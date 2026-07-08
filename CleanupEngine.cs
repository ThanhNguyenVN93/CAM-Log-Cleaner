using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;

namespace frm_CAMLogCleaner
{
    public enum CleanupStepStatus
    {
        Pending,
        Running,
        Success,
        Failed
    }

    /// <summary>Goi tin bao progress duoc dong goi de gui qua BackgroundWorker.ReportProgress.</summary>
    public sealed class CleanupProgress
    {
        public string LogLine;
        public int? StepIndex;
        public CleanupStepStatus? StepStatus;
    }

    public sealed class CleanupResult
    {
        public bool Success;
        public string Message;
        public string BackupDir;
    }

    /// <summary>
    /// Logic don dep CapabilityAccessManager - port tu CamCleaner_v3.cs (console),
    /// giu nguyen thu tu cac buoc va cach xu ly loi, chi thay Console.WriteLine
    /// bang cac goi Log()/SetStep() bao ve UI thread qua BackgroundWorker.
    /// </summary>
    public sealed class CleanupEngine
    {
        public const string ServiceName = "camsvc";
        public const string SettingsPath = @"C:\ProgramData\Microsoft\Windows\CapabilityAccessManager\Settings";
        private static readonly TimeSpan ServiceTimeout = TimeSpan.FromSeconds(10);
        private const int MaxFileRetries = 3;
        private const int RetryDelayMs = 200;

        private readonly BackgroundWorker _worker;

        public CleanupEngine(BackgroundWorker worker)
        {
            _worker = worker;
        }

        private void Log(string line)
        {
            _worker.ReportProgress(0, new CleanupProgress { LogLine = line });
        }

        private void SetStep(int index, CleanupStepStatus status)
        {
            _worker.ReportProgress(0, new CleanupProgress { StepIndex = index, StepStatus = status });
        }

        private void SetProgress(int percent)
        {
            _worker.ReportProgress(percent, null);
        }

        public CleanupResult Run()
        {
            Log("=== CapabilityAccessManager Cleanup Tool ===");

            // Step 0: Check Admin rights & service
            SetStep(0, CleanupStepStatus.Running);

            if (!IsAdministrator())
            {
                Log("[ERROR] Please run this program as Administrator.");
                SetStep(0, CleanupStepStatus.Failed);
                return Fail("Missing Administrator rights.");
            }

            using (ServiceController service = new ServiceController(ServiceName))
            {
                bool serviceWasStopped = false;

                try
                {
                    service.Refresh();
                    ServiceControllerStatus probe = service.Status; // force a read to detect a missing service
                }
                catch (InvalidOperationException ex)
                {
                    Log("[ERROR] Service " + ServiceName + " not found: " + ex.Message);
                    SetStep(0, CleanupStepStatus.Failed);
                    return Fail("Service " + ServiceName + " not found.");
                }

                SetStep(0, CleanupStepStatus.Success);
                SetProgress(10);

                // Windows creates the Settings folder lazily, the first time any app is granted
                // or denied a capability (camera/mic/...). A brand-new or barely-used machine may
                // genuinely not have it yet - that's not an error, there's simply nothing to clean.
                if (!Directory.Exists(SettingsPath))
                {
                    Log("[*] No CapabilityAccessManager settings found on this system yet - nothing to clean.");
                    SetStep(1, CleanupStepStatus.Success);
                    SetStep(2, CleanupStepStatus.Success);
                    SetStep(3, CleanupStepStatus.Success);
                    SetProgress(75);

                    SetStep(4, CleanupStepStatus.Running);
                    if (StartAndVerify(service))
                    {
                        SetStep(4, CleanupStepStatus.Success);
                        SetProgress(100);
                        Log("[SUCCESS] Nothing to clean up, service is running normally.");
                        return Ok("Nothing to clean up.", null);
                    }

                    SetStep(4, CleanupStepStatus.Failed);
                    Log("[WARNING] The service could not be verified as running.");
                    return Fail("Service could not be verified as running.");
                }

                // Step 1: Stop the service if it isn't already Stopped
                SetStep(1, CleanupStepStatus.Running);
                try
                {
                    service.Refresh();
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        Log("[*] Stopping service " + ServiceName + "...");
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, ServiceTimeout);
                    }
                    serviceWasStopped = true;
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    Log("[ERROR] Could not stop the service within 10 seconds. Files may be locked.");
                    SetStep(1, CleanupStepStatus.Failed);
                    return Fail("Could not stop the service within the allowed time.");
                }
                catch (InvalidOperationException ex)
                {
                    Log("[ERROR] Service " + ServiceName + " not found or could not be controlled: " + ex.Message);
                    SetStep(1, CleanupStepStatus.Failed);
                    return Fail("Failed to control the service.");
                }

                SetStep(1, CleanupStepStatus.Success);
                SetProgress(25);

                // Step 2: Take ownership, reset ACLs and back up
                SetStep(2, CleanupStepStatus.Running);
                Log("[*] Resetting access permissions...");
                bool takeownOk = RunElevatedCommand("takeown", "/f \"" + SettingsPath + "\" /r /d y");
                bool icaclsOk = RunElevatedCommand("icacls", "\"" + SettingsPath + "\" /grant administrators:(OI)(CI)F /t /q");

                if (!takeownOk || !icaclsOk)
                {
                    Log("[ERROR] Failed to set access permissions. Stopping to keep things safe.");
                    RestoreServiceBeforeExit(service, serviceWasStopped, "permission setup failure");
                    SetStep(2, CleanupStepStatus.Failed);
                    return Fail("Failed to set access permissions.");
                }

                string backupDir = Path.Combine(Path.GetTempPath(), "CAM_Backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Log("[*] Backing up to: " + backupDir);
                int totalFiles = 0;
                int failedCopies = 0;
                bool anyDirUnreadable = false;
                SafeCopyDirectory(SettingsPath, backupDir, ref totalFiles, ref failedCopies, ref anyDirUnreadable);

                if (totalFiles == 0 || failedCopies == totalFiles)
                {
                    Log("[ERROR] Backup failed or the directory is unreadable. Stopping to protect your data.");
                    RestoreServiceBeforeExit(service, serviceWasStopped, "backup failure");
                    SetStep(2, CleanupStepStatus.Failed);
                    return Fail("Data backup failed.");
                }
                if (anyDirUnreadable)
                    Log("[WARNING] Some subfolders could not be read; the backup may be missing data from them.");
                if (failedCopies > 0)
                    Log("[WARNING] Backup completed but " + failedCopies + "/" + totalFiles + " files are missing.");
                else
                    Log("[*] Safely backed up " + totalFiles + " files.");

                SetStep(2, CleanupStepStatus.Success);
                SetProgress(55);

                // Step 3: Delete temporary log files (.db-wal, .db-shm)
                SetStep(3, CleanupStepStatus.Running);
                Log("[*] Cleaning up temporary log files...");
                int failedDeletes = DeleteFilesByPattern(SettingsPath, "*.db-wal");
                failedDeletes += DeleteFilesByPattern(SettingsPath, "*.db-shm");
                if (failedDeletes > 0)
                    Log("[WARNING] " + failedDeletes + " temporary log files are locked and could not be removed.");
                else
                    Log("[*] Deleted .db-wal, .db-shm files.");

                SetStep(3, CleanupStepStatus.Success);
                SetProgress(75);

                // Step 4: Restart and verify
                SetStep(4, CleanupStepStatus.Running);
                if (StartAndVerify(service))
                {
                    SetStep(4, CleanupStepStatus.Success);
                    SetProgress(100);
                    if (failedDeletes == 0)
                    {
                        Log("[SUCCESS] Cleanup completed, app permission settings were preserved.");
                        return Ok("Cleanup completed.", backupDir);
                    }
                    Log("[SUCCESS-PARTIAL] Service is running again, but some log files could not be fully cleaned.");
                    return Ok("Partially completed, some log files are still locked.", backupDir);
                }

                // Step 5 (fallback): Full database reset if the service fails to start
                Log("[WARNING] The service failed to start - the database may be corrupted.");
                Log("[WARNING] Performing a FULL RESET of the database (.db). All apps' Camera/Mic settings will be reset.");

                try
                {
                    service.Refresh();
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, ServiceTimeout);
                    }
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    Thread.Sleep(1000);
                    service.Refresh();
                    if (service.Status != ServiceControllerStatus.Stopped)
                        Log("[WARNING] The service still hasn't responded in time. Deleting .db files may be denied.");
                }
                catch (Exception ex)
                {
                    Log("[WARNING] Could not control the service state in the fallback path: " + ex.Message);
                }

                int failedDbDeletes = DeleteFilesByPattern(SettingsPath, "*.db");
                if (failedDbDeletes > 0)
                    Log("[WARNING] Could not delete " + failedDbDeletes + " original .db files.");

                if (StartAndVerify(service))
                {
                    SetStep(4, CleanupStepStatus.Success);
                    SetProgress(100);
                    Log("[SUCCESS] Database reset completed successfully. The service is back up.");
                    return Ok("Database reset completed successfully.", backupDir);
                }

                Log("[CRITICAL] RESET FAILED! The service still cannot start.");
                Log("[INFO] Your original backup is still safe at: " + backupDir);
                SetStep(4, CleanupStepStatus.Failed);
                return Fail("Reset failed, the service could not start. Backup at: " + backupDir);
            }
        }

        private static CleanupResult Ok(string message, string backupDir)
        {
            return new CleanupResult { Success = true, Message = message, BackupDir = backupDir };
        }

        private static CleanupResult Fail(string message)
        {
            return new CleanupResult { Success = false, Message = message };
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // Called before any exit path that happens AFTER the service was stopped in Step 1.
        // Never leave the user with camsvc stuck Stopped without knowing about it.
        private void RestoreServiceBeforeExit(ServiceController service, bool serviceWasStopped, string context)
        {
            if (!serviceWasStopped) return;
            try
            {
                service.Refresh();
                if (service.Status != ServiceControllerStatus.Running)
                {
                    Log("[*] Restarting service before exit (" + context + ")...");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, ServiceTimeout);
                    Log("[*] Service restarted, system back to normal.");
                }
            }
            catch (Exception ex)
            {
                Log("[WARNING] Could not restart the service after " + context + ": " + ex.Message);
                Log("[WARNING] IMPORTANT: camsvc is currently STOPPED. Camera/Mic will not work for any app until you manually restart the service (services.msc).");
            }
        }

        private bool RunElevatedCommand(string fileName, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(fileName, arguments);
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        Log("[WARNING] " + fileName + " returned error code: " + p.ExitCode);
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log("[WARNING] Could not run " + fileName + ": " + ex.Message);
                return false;
            }
        }

        // Walk each directory level; a failure in one branch must NEVER block sibling branches.
        // anyDirUnreadable is just a status flag, it does not interrupt the tree walk.
        private void SafeCopyDirectory(string sourceDir, string destDir, ref int totalFiles, ref int failedFiles, ref bool anyDirUnreadable)
        {
            try
            {
                Directory.CreateDirectory(destDir);
            }
            catch (Exception ex)
            {
                Log("[WARNING] Could not create backup directory " + destDir + ": " + ex.Message);
                anyDirUnreadable = true;
                return;
            }

            string[] files;
            try
            {
                files = Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
                Log("[WARNING] Could not read files in " + sourceDir + ": " + ex.Message);
                anyDirUnreadable = true;
                files = new string[0];
            }

            foreach (string file in files)
            {
                totalFiles++;
                bool copied = false;
                for (int attempt = 1; attempt <= MaxFileRetries && !copied; attempt++)
                {
                    try
                    {
                        string destFile = Path.Combine(destDir, Path.GetFileName(file));
                        File.Copy(file, destFile, true);
                        copied = true;
                    }
                    catch (Exception ex)
                    {
                        if (attempt == MaxFileRetries)
                            Log("[WARNING] Could not copy " + file + ": " + ex.Message);
                        else
                            Thread.Sleep(RetryDelayMs);
                    }
                }
                if (!copied) failedFiles++;
            }

            string[] subDirs;
            try
            {
                subDirs = Directory.GetDirectories(sourceDir);
            }
            catch (Exception ex)
            {
                Log("[WARNING] Could not list subfolders of " + sourceDir + ": " + ex.Message);
                anyDirUnreadable = true;
                subDirs = new string[0];
            }

            // Do not return early here: a failure in one branch must not block sibling branches.
            foreach (string subDir in subDirs)
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                SafeCopyDirectory(subDir, destSubDir, ref totalFiles, ref failedFiles, ref anyDirUnreadable);
            }
        }

        private int DeleteFilesByPattern(string dir, string pattern)
        {
            int failed = 0;
            try
            {
                string[] files = Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    bool deleted = false;
                    for (int attempt = 1; attempt <= MaxFileRetries && !deleted; attempt++)
                    {
                        try
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                            deleted = true;
                        }
                        catch (Exception ex)
                        {
                            if (attempt == MaxFileRetries)
                                Log("[WARNING] Could not delete " + file + " after " + MaxFileRetries + " attempts: " + ex.Message);
                            else
                                Thread.Sleep(RetryDelayMs);
                        }
                    }
                    if (!deleted) failed++;
                }

                string[] subDirs = Directory.GetDirectories(dir);
                foreach (string subDir in subDirs)
                {
                    failed += DeleteFilesByPattern(subDir, pattern);
                }
            }
            catch (Exception ex)
            {
                Log("[WARNING] Could not scan directory " + dir + ": " + ex.Message);
                failed++;
            }
            return failed;
        }

        private bool StartAndVerify(ServiceController service)
        {
            try
            {
                Log("[*] Restarting the service...");
                service.Refresh();
                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, ServiceTimeout);
                }
                return true;
            }
            catch (System.ServiceProcess.TimeoutException)
            {
                Log("[WARNING] The service did not reach RUNNING status within " + ServiceTimeout.TotalSeconds + " seconds.");
                return false;
            }
            catch (Exception ex)
            {
                Log("[WARNING] System error while restarting the service: " + ex.Message);
                return false;
            }
        }
    }
}
