using System.Drawing.Drawing2D;

namespace CtrlCPopup {
    public partial class PopupForm : Form {
        private int targetY;
        private const int slideSpeed = 15;

        private System.Windows.Forms.Timer timerSlide;
        private System.Windows.Forms.Timer timerClose;

        private const int cornerRadius = 20;

        private static PopupForm currentPopup;

        public PopupForm() {
            InitializeForm();
        }

        private void InitializeForm() {
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = SystemAccentColorHelper.GetAccentColor();
            this.Size = new Size(180, 100);
            this.TopMost = true;
            this.ShowInTaskbar = false;

            Label lblMessage = new Label();
            lblMessage.Text = "Copied!";
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI", 20, FontStyle.Regular);
            lblMessage.ForeColor = Color.White;
            lblMessage.Location = new Point(33, 32);
            this.Controls.Add(lblMessage);

            timerSlide = new System.Windows.Forms.Timer();
            timerSlide.Interval = 10;
            timerSlide.Tick += TimerSlide_Tick;

            timerClose = new System.Windows.Forms.Timer();
            timerClose.Interval = 3000; //* 3 seconds auto-close
            timerClose.Tick += TimerClose_Tick;

            ApplyRoundedCorners();
        }

        private void ApplyRoundedCorners() {
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = new Rectangle(0, 0, this.Width, this.Height);

            path.AddArc(bounds.X, bounds.Y, cornerRadius * 2, cornerRadius * 2, 180, 90); // Top-left
            path.AddArc(bounds.Right - cornerRadius * 2, bounds.Y, cornerRadius * 2, cornerRadius * 2, 270, 90); // Top-right
            path.AddArc(bounds.Right - cornerRadius * 2, bounds.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90); // Bottom-right
            path.AddArc(bounds.X, bounds.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90); // Bottom-left
            path.CloseFigure();

            this.Region = new Region(path);
        }

        public void ShowPopup() {
            //! Close any existing popup
            if (currentPopup != null && !currentPopup.IsDisposed) {
                currentPopup.Close();
            }

            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height;
            targetY = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 20; // 20 pixels from the bottom

            this.Show();
            timerSlide.Start();
            timerClose.Start();

            currentPopup = this;
        }

        private void TimerSlide_Tick(object sender, EventArgs e) {
            if (this.Top > targetY) {
                this.Top -= slideSpeed;
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
