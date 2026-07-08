namespace frm_CAMLogCleaner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // Cac control cua 5 buoc don dep duoc gom thanh mang de de dang cap nhat trang thai
        // tu CleanupEngine (pnlStep[i] / picStepIcon[i] / lblStep[i]). Vi dung mang thay vi
        // field rieng cho tung control, phan nay se KHONG hien thi duoc tren WinForms Designer
        // (canvas) - can chinh sua bang code.
        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Label lblMinimize;
        private System.Windows.Forms.Label lblMaximize;
        private System.Windows.Forms.Label lblClose;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;

        private Guna.UI2.WinForms.Guna2Panel pnlStatusDot;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblServiceStatus;

        private System.Windows.Forms.Panel[] pnlStep;
        private System.Windows.Forms.PictureBox[] picStepIcon;
        private System.Windows.Forms.Label[] lblStep;

        private Guna.UI2.WinForms.Guna2ProgressBar progressBar;

        private Guna.UI2.WinForms.Guna2Panel pnlLog;
        private System.Windows.Forms.RichTextBox rtbLog;

        private Guna.UI2.WinForms.Guna2Button btnClose;
        private Guna.UI2.WinForms.Guna2Button btnStart;
        private System.Windows.Forms.LinkLabel lnkFeedback;
        private Guna.UI2.WinForms.Guna2ToggleSwitch toggleTheme;
        private System.Windows.Forms.ToolTip toolTip1;

        private System.Windows.Forms.Timer spinTimer;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.lblIcon = new System.Windows.Forms.Label();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.lblMinimize = new System.Windows.Forms.Label();
            this.lblMaximize = new System.Windows.Forms.Label();
            this.lblClose = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlStatusDot = new Guna.UI2.WinForms.Guna2Panel();
            this.lblServiceStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.progressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            this.pnlLog = new Guna.UI2.WinForms.Guna2Panel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.btnStart = new Guna.UI2.WinForms.Guna2Button();
            this.lnkFeedback = new System.Windows.Forms.LinkLabel();
            this.toggleTheme = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.spinTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlHeader.SuspendLayout();
            this.pnlLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHeader.Controls.Add(this.lblIcon);
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Controls.Add(this.lblMinimize);
            this.pnlHeader.Controls.Add(this.lblMaximize);
            this.pnlHeader.Controls.Add(this.lblClose);
            this.pnlHeader.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(39)))));
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(520, 36);
            this.pnlHeader.TabIndex = 8;
            this.pnlHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeaderDrag_MouseDown);
            // 
            // lblIcon
            // 
            this.lblIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblIcon.Font = new System.Drawing.Font("Segoe UI Emoji", 10F);
            this.lblIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(202)))), ((int)(((byte)(165)))));
            this.lblIcon.Location = new System.Drawing.Point(12, 6);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(22, 22);
            this.lblIcon.TabIndex = 0;
            this.lblIcon.Text = "🛡";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeaderDrag_MouseDown);
            // 
            // lblAppTitle
            // 
            this.lblAppTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(36, 9);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(160, 18);
            this.lblAppTitle.TabIndex = 1;
            this.lblAppTitle.Text = "CamCleaner";
            this.lblAppTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeaderDrag_MouseDown);
            // 
            // lblMinimize
            // 
            this.lblMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMinimize.BackColor = System.Drawing.Color.Transparent;
            this.lblMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblMinimize.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblMinimize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(160)))), ((int)(((byte)(172)))));
            this.lblMinimize.Location = new System.Drawing.Point(428, 6);
            this.lblMinimize.Name = "lblMinimize";
            this.lblMinimize.Size = new System.Drawing.Size(22, 22);
            this.lblMinimize.TabIndex = 2;
            this.lblMinimize.Text = "–";
            this.lblMinimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMinimize.Click += new System.EventHandler(this.lblMinimize_Click);
            // 
            // lblMaximize
            // 
            this.lblMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMaximize.BackColor = System.Drawing.Color.Transparent;
            this.lblMaximize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblMaximize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMaximize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(160)))), ((int)(((byte)(172)))));
            this.lblMaximize.Location = new System.Drawing.Point(458, 6);
            this.lblMaximize.Name = "lblMaximize";
            this.lblMaximize.Size = new System.Drawing.Size(22, 22);
            this.lblMaximize.TabIndex = 3;
            this.lblMaximize.Text = "□";
            this.lblMaximize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMaximize.Click += new System.EventHandler(this.lblMaximize_Click);
            // 
            // lblClose
            // 
            this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClose.BackColor = System.Drawing.Color.Transparent;
            this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClose.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(160)))), ((int)(((byte)(172)))));
            this.lblClose.Location = new System.Drawing.Point(488, 6);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(22, 22);
            this.lblClose.TabIndex = 4;
            this.lblClose.Text = "✕";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblClose.Click += new System.EventHandler(this.lblClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(31)))), ((int)(((byte)(39)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 52);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(480, 22);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Clean up CapabilityAccessManager";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(143)))), ((int)(((byte)(152)))));
            this.lblSubtitle.Location = new System.Drawing.Point(20, 76);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(480, 16);
            this.lblSubtitle.TabIndex = 0;
            this.lblSubtitle.Text = "camsvc · C:\\ProgramData\\...\\Settings";
            // 
            // pnlStatusDot
            // 
            this.pnlStatusDot.BorderRadius = 4;
            this.pnlStatusDot.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.pnlStatusDot.Location = new System.Drawing.Point(20, 101);
            this.pnlStatusDot.Name = "pnlStatusDot";
            this.pnlStatusDot.Size = new System.Drawing.Size(8, 8);
            this.pnlStatusDot.TabIndex = 2;
            // 
            // lblServiceStatus
            // 
            this.lblServiceStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblServiceStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblServiceStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(101)))), ((int)(((byte)(112)))));
            this.lblServiceStatus.Location = new System.Drawing.Point(34, 97);
            this.lblServiceStatus.Name = "lblServiceStatus";
            this.lblServiceStatus.Size = new System.Drawing.Size(129, 15);
            this.lblServiceStatus.TabIndex = 3;
            this.lblServiceStatus.Text = "Service: <b>Checking...</b>";
            // 
            // progressBar
            // 
            this.progressBar.BorderRadius = 3;
            this.progressBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
            this.progressBar.Location = new System.Drawing.Point(20, 308);
            this.progressBar.Name = "progressBar";
            this.progressBar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(158)))), ((int)(((byte)(117)))));
            this.progressBar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(158)))), ((int)(((byte)(117)))));
            this.progressBar.Size = new System.Drawing.Size(480, 6);
            this.progressBar.TabIndex = 4;
            this.progressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // pnlLog
            // 
            this.pnlLog.BorderRadius = 8;
            this.pnlLog.Controls.Add(this.rtbLog);
            this.pnlLog.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(21)))), ((int)(((byte)(28)))));
            this.pnlLog.Location = new System.Drawing.Point(20, 326);
            this.pnlLog.Name = "pnlLog";
            this.pnlLog.Size = new System.Drawing.Size(480, 220);
            this.pnlLog.TabIndex = 5;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(21)))), ((int)(((byte)(28)))));
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(183)))), ((int)(((byte)(192)))), ((int)(((byte)(206)))));
            this.rtbLog.Location = new System.Drawing.Point(13, 13);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(454, 194);
            this.rtbLog.TabIndex = 0;
            this.rtbLog.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(223)))), ((int)(((byte)(229)))));
            this.btnClose.BorderRadius = 8;
            this.btnClose.BorderThickness = 1;
            this.btnClose.FillColor = System.Drawing.Color.White;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(101)))), ((int)(((byte)(112)))));
            this.btnClose.Location = new System.Drawing.Point(240, 566);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 38);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.BorderRadius = 8;
            this.btnStart.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(158)))), ((int)(((byte)(117)))));
            this.btnStart.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(340, 566);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(160, 38);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start Cleanup";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // lnkFeedback
            //
            this.lnkFeedback.AutoSize = true;
            this.lnkFeedback.BackColor = System.Drawing.Color.Transparent;
            this.lnkFeedback.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lnkFeedback.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(138)))), ((int)(((byte)(221)))));
            this.lnkFeedback.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkFeedback.Location = new System.Drawing.Point(20, 578);
            this.lnkFeedback.Name = "lnkFeedback";
            this.lnkFeedback.Size = new System.Drawing.Size(120, 15);
            this.lnkFeedback.TabIndex = 9;
            this.lnkFeedback.TabStop = true;
            this.lnkFeedback.Text = "Report a bug / feedback";
            this.lnkFeedback.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkFeedback_LinkClicked);
            //
            // toggleTheme
            //
            this.toggleTheme.Location = new System.Drawing.Point(194, 576);
            this.toggleTheme.Name = "toggleTheme";
            this.toggleTheme.Size = new System.Drawing.Size(36, 18);
            this.toggleTheme.TabIndex = 10;
            this.toolTip1.SetToolTip(this.toggleTheme, "Toggle light / dark theme");
            this.toggleTheme.CheckedChanged += new System.EventHandler(this.toggleTheme_CheckedChanged);
            //
            // spinTimer
            // 
            this.spinTimer.Interval = 60;
            this.spinTimer.Tick += new System.EventHandler(this.spinTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(520, 624);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlStatusDot);
            this.Controls.Add(this.lblServiceStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.pnlLog);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lnkFeedback);
            this.Controls.Add(this.toggleTheme);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CamCleaner";
            this.pnlHeader.ResumeLayout(false);
            this.pnlLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
