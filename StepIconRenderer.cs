using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace frm_CAMLogCleaner
{
    /// <summary>Ve icon trang thai buoc (pending/running/success/failed) bang GDI+, khong can file anh.</summary>
    internal static class StepIconRenderer
    {
        private static readonly Color ColorPending = Color.FromArgb(0xB4, 0xB2, 0xA9);
        private static readonly Color ColorRunning = Color.FromArgb(0x37, 0x8A, 0xDD);
        private static readonly Color ColorSuccess = Color.FromArgb(0x1D, 0x9E, 0x75);
        private static readonly Color ColorFailed = Color.FromArgb(0xD9, 0x53, 0x4F);
        private static readonly Color ColorTrack = Color.FromArgb(0xE4, 0xE7, 0xEC);

        public static Bitmap Render(CleanupStepStatus status, float spinAngle, int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                float pad = size * 0.1f;
                RectangleF rect = new RectangleF(pad, pad, size - pad * 2, size - pad * 2);

                switch (status)
                {
                    case CleanupStepStatus.Pending:
                        using (Pen pen = new Pen(ColorPending, size * 0.09f))
                            g.DrawEllipse(pen, rect);
                        break;

                    case CleanupStepStatus.Running:
                        using (Pen track = new Pen(ColorTrack, size * 0.12f))
                            g.DrawEllipse(track, rect);
                        using (Pen arc = new Pen(ColorRunning, size * 0.12f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                            g.DrawArc(arc, rect, spinAngle, 100f);
                        break;

                    case CleanupStepStatus.Success:
                        using (SolidBrush brush = new SolidBrush(ColorSuccess))
                            g.FillEllipse(brush, rect);
                        using (Pen check = new Pen(Color.White, size * 0.11f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                        {
                            PointF p1 = new PointF(size * 0.28f, size * 0.52f);
                            PointF p2 = new PointF(size * 0.44f, size * 0.68f);
                            PointF p3 = new PointF(size * 0.74f, size * 0.32f);
                            g.DrawLines(check, new[] { p1, p2, p3 });
                        }
                        break;

                    case CleanupStepStatus.Failed:
                        using (SolidBrush brush = new SolidBrush(ColorFailed))
                            g.FillEllipse(brush, rect);
                        using (Pen cross = new Pen(Color.White, size * 0.11f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                        {
                            float m = size * 0.3f;
                            g.DrawLine(cross, m, m, size - m, size - m);
                            g.DrawLine(cross, size - m, m, m, size - m);
                        }
                        break;
                }
            }
            return bmp;
        }
    }
}
