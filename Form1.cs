using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;
using Microsoft.Win32;

namespace frm_CAMLogCleaner
{
    public partial class Form1 : Form
    {
        private readonly BackgroundWorker _worker;
        private float _spinAngle;
        private int _runningStepIndex = -1;
        private bool _isCleaning;
        private bool _isDarkTheme;
        private CleanupStepStatus[] _stepStatuses;

        private Color _stepRowBg;
        private Color _stepRowHighlight;
        private Color _stepPendingText;
        private Color _stepActiveText;

        private static readonly string[] StepLabels = new string[]
        {
            "Check Admin rights & service",
            "Stop camsvc service",
            "Set permissions & backup",
            "Clean up temp log files (*.db-wal, *.db-shm)",
            "Restart & verify service"
        };

        public Form1()
        {
            InitializeComponent();

            // Chong nhay hinh (flicker) khi spinTimer lien tuc doi Image cua icon dang chay.
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            Icon = LoadEmbeddedIcon() ?? Icon;

            BuildStepRows();
            ApplyButtonHoverColors();
            InitializeTheme();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            Load += Form1_Load;
        }

        // CamCleaner.ico duoc nhung vao assembly (EmbeddedResource) thay vi doc tu file ngoai,
        // de icon runtime (taskbar/Alt-Tab) luon di kem theo .exe du deploy o dau.
        private static Icon LoadEmbeddedIcon()
        {
            Assembly asm = typeof(Form1).Assembly;
            string resourceName = Array.Find(asm.GetManifestResourceNames(),
                n => n.EndsWith("CamCleaner.ico", StringComparison.OrdinalIgnoreCase));
            if (resourceName == null) return null;

            using (Stream stream = asm.GetManifestResourceStream(resourceName))
                return stream != null ? new Icon(stream) : null;
        }

        // Dung 5 dong buoc dong (khong dat trong InitializeComponent vi WinForms Designer
        // khong parse duoc vong lap/mang trong do - chi chap nhan mot tap con khai bao tuan tu).
        private void BuildStepRows()
        {
            const int stepStartY = 128;
            const int stepPitch = 34;
            const int stepHeight = 30;

            pnlStep = new Panel[StepLabels.Length];
            picStepIcon = new PictureBox[StepLabels.Length];
            lblStep = new Label[StepLabels.Length];
            _stepStatuses = new CleanupStepStatus[StepLabels.Length];

            for (int i = 0; i < StepLabels.Length; i++)
            {
                Panel row = new Panel();
                row.Location = new Point(12, stepStartY + i * stepPitch);
                row.Size = new Size(496, stepHeight);
                row.BackColor = Color.White;
                EnableDoubleBuffer(row);

                PictureBox icon = new PictureBox();
                icon.Location = new Point(8, 6);
                icon.Size = new Size(18, 18);
                icon.SizeMode = PictureBoxSizeMode.CenterImage;
                icon.BackColor = Color.Transparent;

                Label text = new Label();
                text.Location = new Point(34, 6);
                text.Size = new Size(450, 18);
                text.Font = new Font("Segoe UI", 9F);
                text.ForeColor = Color.FromArgb(0x8A, 0x8F, 0x98);
                text.BackColor = Color.Transparent;
                text.Text = StepLabels[i];

                row.Controls.Add(icon);
                row.Controls.Add(text);

                pnlStep[i] = row;
                picStepIcon[i] = icon;
                lblStep[i] = text;

                Controls.Add(row);
            }
        }

        // Panel.DoubleBuffered la protected, phai bat qua reflection. Giam nhay hinh
        // o vung icon khi spinTimer doi Image lien tuc trong luc dong buoc dang chay.
        private static void EnableDoubleBuffer(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        // Gan ngoai InitializeComponent() vi WinForms Designer khong parse duoc kieu gan
        // property long 2 cap (control.HoverState.FillColor = ...) tren Guna2Button.
        private void ApplyButtonHoverColors()
        {
            btnStart.HoverState.FillColor = Color.FromArgb(0x17, 0x8C, 0x67);
        }

        // === Light / dark theme ===

        private void InitializeTheme()
        {
            _isDarkTheme = DetectSystemDarkTheme();
            toggleTheme.Checked = _isDarkTheme;
            ApplyTheme(_isDarkTheme);
        }

        // Doc gia tri AppsUseLightTheme tu registry de lay theme mac dinh cua Windows.
        // Neu khong doc duoc (may cu, quyen bi chan...) thi mac dinh ve theme sang.
        private static bool DetectSystemDarkTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    object value = key != null ? key.GetValue("AppsUseLightTheme") : null;
                    if (value is int)
                        return (int)value == 0;
                }
            }
            catch
            {
                // Bo qua, dung theme sang mac dinh.
            }
            return false;
        }

        private void toggleTheme_CheckedChanged(object sender, EventArgs e)
        {
            _isDarkTheme = toggleTheme.Checked;
            ApplyTheme(_isDarkTheme);
        }

        private void ApplyTheme(bool dark)
        {
            Color formBack = dark ? Color.FromArgb(0x17, 0x1B, 0x23) : Color.White;
            Color titleText = dark ? Color.FromArgb(0xF2, 0xF4, 0xF7) : Color.FromArgb(0x1B, 0x1F, 0x27);
            Color subtitleText = dark ? Color.FromArgb(0x9A, 0xA3, 0xAF) : Color.FromArgb(0x8A, 0x8F, 0x98);
            Color serviceText = dark ? Color.FromArgb(0xB7, 0xC0, 0xCE) : Color.FromArgb(0x5F, 0x65, 0x70);
            Color progressTrack = dark ? Color.FromArgb(0x2A, 0x30, 0x3C) : Color.FromArgb(0xED, 0xEF, 0xF2);
            Color btnCloseFill = dark ? Color.FromArgb(0x23, 0x29, 0x35) : Color.White;
            Color btnCloseBorder = dark ? Color.FromArgb(0x3A, 0x41, 0x50) : Color.FromArgb(0xDA, 0xDF, 0xE5);
            Color btnCloseText = dark ? Color.FromArgb(0xC7, 0xCD, 0xD6) : Color.FromArgb(0x5F, 0x65, 0x70);
            Color btnCloseHover = dark ? Color.FromArgb(0x2C, 0x33, 0x41) : Color.FromArgb(0xF6, 0xF7, 0xF8);

            BackColor = formBack;
            lblTitle.ForeColor = titleText;
            lblSubtitle.ForeColor = subtitleText;
            lblServiceStatus.ForeColor = serviceText;
            progressBar.FillColor = progressTrack;

            btnClose.FillColor = btnCloseFill;
            btnClose.BorderColor = btnCloseBorder;
            btnClose.ForeColor = btnCloseText;
            btnClose.HoverState.FillColor = btnCloseHover;

            _stepRowBg = formBack;
            _stepRowHighlight = dark ? Color.FromArgb(0x1B, 0x2A, 0x3D) : Color.FromArgb(0xF3, 0xF7, 0xFE);
            _stepPendingText = dark ? Color.FromArgb(0x6B, 0x72, 0x80) : Color.FromArgb(0x8A, 0x8F, 0x98);
            _stepActiveText = titleText;

            if (pnlStep != null)
            {
                for (int i = 0; i < pnlStep.Length; i++)
                    RenderStepVisual(i);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!CheckPlatformCompatibility())
                return;

            RefreshServiceStatusLabel();

            if (!CleanupEngine.IsAdministrator())
            {
                AppendLog("[ERROR] This application must run as Administrator to perform cleanup.");
                btnStart.Enabled = false;
            }
        }

        // Camsvc chi ton tai tu Windows 10 (~1803) tro len - day la dieu kien thuc su
        // gioi han boi phien ban OS. Rieng thu muc CapabilityAccessManager\Settings duoc
        // Windows tao "lazy" (chi sinh ra khi co App dau tien xin quyen Camera/Mic), nen
        // thieu thu muc nay KHONG co nghia la may khong tuong thich - chi la chua co gi
        // de don. Truong hop do de CleanupEngine tu bao loi ro rang khi bam Start.
        private bool CheckPlatformCompatibility()
        {
            bool serviceExists = ServiceController.GetServices()
                .Any(s => string.Equals(s.ServiceName, CleanupEngine.ServiceName, StringComparison.OrdinalIgnoreCase));

            if (serviceExists)
                return true;

            lblServiceStatus.Text = "Service: <b>Not available</b>";
            pnlStatusDot.FillColor = Color.FromArgb(0xD9, 0x53, 0x4F);

            AppendLog("[ERROR] This feature requires Windows 10 (version 1803 or later) or Windows 11.");
            AppendLog("[ERROR] The camsvc service was not found on this system.");

            btnStart.Enabled = false;
            return false;
        }

        private void RefreshServiceStatusLabel()
        {
            try
            {
                using (ServiceController sc = new ServiceController(CleanupEngine.ServiceName))
                {
                    sc.Refresh();
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Running:
                            lblServiceStatus.Text = "Service: <b>Running</b>";
                            pnlStatusDot.FillColor = Color.FromArgb(0x3B, 0x8B, 0xD4);
                            break;
                        case ServiceControllerStatus.Stopped:
                            lblServiceStatus.Text = "Service: <b>Stopped</b>";
                            pnlStatusDot.FillColor = Color.FromArgb(0xB4, 0xB2, 0xA9);
                            break;
                        default:
                            lblServiceStatus.Text = "Service: <b>" + sc.Status + "</b>";
                            pnlStatusDot.FillColor = Color.FromArgb(0xB4, 0xB2, 0xA9);
                            break;
                    }
                }
            }
            catch
            {
                lblServiceStatus.Text = "Service: <b>Unknown</b>";
                pnlStatusDot.FillColor = Color.FromArgb(0xB4, 0xB2, 0xA9);
            }
        }

        // === Bat dau don dep ===

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_isCleaning) return;

            if (!CleanupEngine.IsAdministrator())
            {
                AppendLog("[ERROR] This application must run as Administrator to perform cleanup.");
                return;
            }

            _isCleaning = true;
            btnStart.Enabled = false;
            btnStart.Text = "Cleaning...";
            rtbLog.Clear();
            progressBar.Value = 0;
            for (int i = 0; i < pnlStep.Length; i++)
                SetStepStatus(i, CleanupStepStatus.Pending);

            _worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            CleanupEngine engine = new CleanupEngine(_worker);
            e.Result = engine.Run();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CleanupProgress payload = e.UserState as CleanupProgress;
            if (payload != null)
            {
                if (payload.LogLine != null)
                    AppendLog(payload.LogLine);

                if (payload.StepIndex.HasValue && payload.StepStatus.HasValue)
                    SetStepStatus(payload.StepIndex.Value, payload.StepStatus.Value);
            }
            else
            {
                progressBar.Value = Math.Max(progressBar.Minimum, Math.Min(progressBar.Maximum, e.ProgressPercentage));
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _isCleaning = false;
            btnStart.Enabled = true;
            btnStart.Text = "Start Cleanup";

            if (e.Error != null)
            {
                AppendLog("[CRITICAL] Unexpected error: " + e.Error.Message);
            }

            RefreshServiceStatusLabel();
        }

        // === Cap nhat trang thai tung buoc / icon ===

        private void SetStepStatus(int index, CleanupStepStatus status)
        {
            if (index < 0 || index >= pnlStep.Length) return;

            _stepStatuses[index] = status;
            RenderStepVisual(index);

            bool isRunning = status == CleanupStepStatus.Running;
            if (isRunning)
            {
                _runningStepIndex = index;
                spinTimer.Enabled = true;
            }
            else if (_runningStepIndex == index)
            {
                _runningStepIndex = -1;
                spinTimer.Enabled = false;
            }
        }

        // Ve lai mot dong buoc theo trang thai da luu (_stepStatuses) va bang mau theme hien tai.
        // Duoc goi lai cho toan bo cac buoc moi khi doi theme.
        private void RenderStepVisual(int index)
        {
            CleanupStepStatus status = _stepStatuses[index];
            picStepIcon[index].Image = StepIconRenderer.Render(status, _spinAngle, picStepIcon[index].Width);

            bool isRunning = status == CleanupStepStatus.Running;
            pnlStep[index].BackColor = isRunning ? _stepRowHighlight : _stepRowBg;

            switch (status)
            {
                case CleanupStepStatus.Pending:
                    lblStep[index].ForeColor = _stepPendingText;
                    lblStep[index].Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                    break;
                case CleanupStepStatus.Running:
                    lblStep[index].ForeColor = _stepActiveText;
                    lblStep[index].Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    break;
                case CleanupStepStatus.Success:
                    lblStep[index].ForeColor = _stepActiveText;
                    lblStep[index].Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                    break;
                case CleanupStepStatus.Failed:
                    lblStep[index].ForeColor = Color.FromArgb(0xD9, 0x53, 0x4F);
                    lblStep[index].Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    break;
            }
        }

        private void spinTimer_Tick(object sender, EventArgs e)
        {
            _spinAngle = (_spinAngle + 24f) % 360f;
            if (_runningStepIndex >= 0 && _runningStepIndex < picStepIcon.Length)
            {
                picStepIcon[_runningStepIndex].Image = StepIconRenderer.Render(
                    CleanupStepStatus.Running, _spinAngle, picStepIcon[_runningStepIndex].Width);
            }
        }

        // === Nhat ky mau theo muc do ===

        private void AppendLog(string line)
        {
            Color color;
            if (line.IndexOf("[ERROR", StringComparison.OrdinalIgnoreCase) >= 0 ||
                line.IndexOf("[CRITICAL", StringComparison.OrdinalIgnoreCase) >= 0)
                color = Color.FromArgb(0xE0, 0x6C, 0x6C);
            else if (line.IndexOf("[WARNING", StringComparison.OrdinalIgnoreCase) >= 0)
                color = Color.FromArgb(0xE0, 0xB4, 0x5A);
            else if (line.IndexOf("[SUCCESS", StringComparison.OrdinalIgnoreCase) >= 0)
                color = Color.FromArgb(0x5D, 0xCA, 0xA5);
            else
                color = Color.FromArgb(0xB7, 0xC0, 0xCE);

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(line + Environment.NewLine);
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        }

        // === Nut dong ===

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lnkFeedback_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://www.facebook.com/PycCodeTools");
            }
            catch
            {
                // Ignore if no browser/handler is available for the URL.
            }
        }

        // === Title bar gia lap: keo tha / thu nho / phong to / dong ===

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private void HeaderDrag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void lblMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void lblMaximize_Click(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
