using System.Runtime.InteropServices;

using System.Drawing.Drawing2D;

namespace CtrlCPopup {
    public partial class PopupForm : Form {

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        private int TargetY;
        private const int SlideSpeed = 12;
        private const int CornerRadius = 20;

        private System.Windows.Forms.Timer timerSlide = new System.Windows.Forms.Timer(),
                                          timerClose = new System.Windows.Forms.Timer();

        private static PopupForm CurrentPopup;
        private Label lblMessage;

        public PopupForm() {
            InitializeForm();
        }

        private void InitializeForm() {
            if (Environment.OSVersion.Version.Major >= 6) {
                SetProcessDPIAware();
            }

            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = SystemAccentColor.GetAccentColor();
            this.Size = new Size(180, 100);
            this.TopMost = true;
            this.ShowInTaskbar = false;

            lblMessage = new Label();
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI", 18, FontStyle.Regular);
            lblMessage.ForeColor = Color.White;
            lblMessage.Location = new Point(33, 32);
            this.Controls.Add(lblMessage);

            timerSlide.Interval = 10;
            timerSlide.Tick += TimerSlide_Tick;

            ApplyRoundedCorners();
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            base.OnPaint(e);

            // Optional: draw a smooth border around the form
            using (GraphicsPath path = GetRoundedRectPath(new Rectangle(0, 0, this.Width, this.Height), CornerRadius)) {
                using (Pen pen = new Pen(this.BackColor, 1)) {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius) {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            // top-left arc
            path.AddArc(arc, 180, 90);

            // top-right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom-right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom-left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void ApplyRoundedCorners() {
            using (GraphicsPath path = new GraphicsPath()) {
                Rectangle bounds = new Rectangle(0, 0, this.Width, this.Height);

                path.StartFigure();
                path.AddArc(bounds.X, bounds.Y, CornerRadius * 2, CornerRadius * 2, 180, 90);
                path.AddArc(bounds.Right - CornerRadius * 2, bounds.Y, CornerRadius * 2, CornerRadius * 2, 270, 90);
                path.AddArc(bounds.Right - CornerRadius * 2, bounds.Bottom - CornerRadius * 2, CornerRadius * 2, CornerRadius * 2, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - CornerRadius * 2, CornerRadius * 2, CornerRadius * 2, 90, 90);
                path.CloseFigure();

                this.Region = new Region(path);
            }

            // Force high-quality rendering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.UpdateStyles();
        }

        public void ShowPopup(string message, int AutoCloseTime = 1) {
            if (CurrentPopup != null && !CurrentPopup.IsDisposed) {
                CurrentPopup.Close();
            }

            lblMessage.Text = message;

            timerClose.Interval = AutoCloseTime * 1000;
            timerClose.Tick += TimerClose_Tick;

            // Measure the text size using the label's font
            Size textSize = TextRenderer.MeasureText(message, lblMessage.Font);

            // Define padding for the form (adjust these values as needed)
            int horizontalPadding = 40; // 20 pixels on each side
            int verticalPadding = 40;   // 20 pixels top and bottom

            // Set the form size based on the text size plus padding
            this.Size = new Size(textSize.Width + horizontalPadding, textSize.Height + verticalPadding);

            // Center the label within the form
            lblMessage.Location = new Point(
                (this.Width - textSize.Width) / 2,
                (this.Height - textSize.Height) / 2
            );

            // Reapply the rounded corners to adjust to the new size
            ApplyRoundedCorners();

            // Position the popup at the bottom-center of the screen
            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height;
            TargetY = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 20;

            this.Show();
            timerSlide.Start();
            timerClose.Start();

            CurrentPopup = this;
        }

        private void TimerSlide_Tick(object sender, EventArgs e) {
            if (this.Top > TargetY) {
                this.Top -= SlideSpeed;
            }
            else {
                timerSlide.Stop();
            }
        }

        private void TimerClose_Tick(object sender, EventArgs e) {
            this.Close();
        }
    }
}
