# CAM Log Cleaner (CamCleaner)

A small Windows utility that fixes broken Camera / Microphone permissions caused by a
corrupted **Capability Access Manager** (`camsvc`) database, without wiping every app's
privacy settings unless it's truly necessary.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4)
![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6)
![License](https://img.shields.io/badge/license-MIT-green)

## What it does

Windows stores per-app Camera/Microphone/Location consent decisions in
`C:\ProgramData\Microsoft\Windows\CapabilityAccessManager\Settings`. When those SQLite-backed
files get corrupted, apps can lose access to the camera or mic with no clear error message.
This tool safely repairs that state:

1. Checks Administrator rights and that the `camsvc` service exists
2. Stops the `camsvc` service
3. Takes ownership, resets ACLs, and **backs up** the settings folder before touching anything
4. Removes stale `*.db-wal` / `*.db-shm` journal files (the usual source of corruption)
5. Restarts the service and verifies it comes back up
6. **Fallback only if needed:** if the service still won't start, performs a full reset of the
   `.db` files (this resets every app's Camera/Mic permission back to default) — your original
   backup is kept either way

If the settings folder doesn't exist yet (a fresh machine that has never prompted for
camera/mic access), the tool reports there's simply nothing to clean instead of treating it
as an error.

## Requirements

- Windows 10 (version 1803+) or Windows 11 — earlier versions don't have `camsvc`
- .NET Framework 4.7.2 runtime
- Administrator privileges (the app requests elevation on launch)

## Building

1. Open `frm_CAMLogCleaner.slnx` in Visual Studio (2022+ recommended)
2. Let NuGet restore the packages listed in `packages.config` (`Guna.UI2.WinForms`, `ILRepack`)
   — Visual Studio does this automatically on build if "Allow NuGet to download missing
   packages" is enabled (default)
3. Build:
   - **Debug** — produces `CAM Log Cleaner.exe` next to `Guna.UI2.dll` in `bin\Debug\`
   - **Release** — merges `Guna.UI2.dll` into the executable (via ILRepack) so
     `bin\Release\` contains a single standalone `CAM Log Cleaner.exe`

## Safety notes

- A full backup of the settings folder is always taken before any destructive step, to
  `%TEMP%\CAM_Backup_<timestamp>`
- The service is always restarted before the tool exits, even on failure paths — it never
  leaves `camsvc` stopped
- The full `.db` reset (which clears every app's Camera/Mic permission) only runs as a last
  resort, and only if the normal cleanup path fails to bring the service back up

## Feedback

Found a bug or have a suggestion? [Report it here](https://www.facebook.com/PycCodeTools),
or use the "Report a bug / feedback" link inside the app.

## License

MIT — see [LICENSE](LICENSE).
