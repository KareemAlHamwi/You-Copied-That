using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

public partial class PopupForm : Form {

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();
    private int TargetY;
    private const int SlideSpeed = 12;
    private const int CornerRadius = 20;
    private const double FadeStep = 0.05;
    private System.Windows.Forms.Timer TimerSlide = new System.Windows.Forms.Timer(),
                                      TimerClose = new System.Windows.Forms.Timer(),
                                      TadeTimer = new System.Windows.Forms.Timer();
    private bool FadingIn = true;
    private static PopupForm CurrentPopup;
    private Label lblMessage;

    //? Prevent the popup from stealing focus and hide it from Alt+Tab
    protected override CreateParams CreateParams {
        get {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
            cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
            return cp;
        }
    }

    public PopupForm() {
        InitializeForm();

        TadeTimer.Interval = 15;
        TadeTimer.Tick += FadeTimer_Tick;
    }

    private void InitializeForm() {
        if (Environment.OSVersion.Version.Major >= 6) {
            SetProcessDPIAware();
        }

        this.StartPosition = FormStartPosition.Manual;
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = SystemAccentColor.GetAccentColor();
        // Get the working area of the primary screen
        Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

        // Set the form size to 25% of the screen's width and 10% of the screen's height (adjust as needed)
        this.Size = new Size(workingArea.Width / 4, workingArea.Height / 10);
        this.TopMost = true;
        this.ShowInTaskbar = false;
        this.Opacity = 0;

        lblMessage = new Label();
        lblMessage.AutoSize = true;
        lblMessage.Font = new Font("Segoe UI", 18, FontStyle.Regular);
        lblMessage.ForeColor = Color.White;
        lblMessage.Location = new Point(33, 32);
        this.Controls.Add(lblMessage);

        TimerSlide.Interval = 10;
        TimerSlide.Tick += TimerSlide_Tick;

        TimerClose.Tick += TimerClose_Tick;

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

    private GraphicsPath GetRoundedRectPath(Rectangle Bounds, int Radius) {
        GraphicsPath path = new GraphicsPath();
        int diameter = Radius * 2;
        Rectangle arc = new Rectangle(Bounds.Location, new Size(diameter, diameter));

        path.AddArc(arc, 180, 90);

        arc.X = Bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        arc.Y = Bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        arc.X = Bounds.Left;
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

        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        this.UpdateStyles();
    }

    public void ShowPopup(string Message, int AutoCloseTimeInSeconds = 1) {
        if (CurrentPopup != null && !CurrentPopup.IsDisposed) {
            CurrentPopup.Close();
        }

        lblMessage.Text = Message;
        TimerClose.Interval = AutoCloseTimeInSeconds * 1000;

        Size TextSize = TextRenderer.MeasureText(Message, lblMessage.Font);

        int HorizontalPadding = 20;
        int verticalPadding = 30;

        this.Size = new Size(TextSize.Width + HorizontalPadding, TextSize.Height + verticalPadding);

        lblMessage.Location = new Point(
            (this.Width - TextSize.Width) / 2,
            (this.Height - TextSize.Height) / 2
        );

        ApplyRoundedCorners();

        this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
        this.Top = Screen.PrimaryScreen.WorkingArea.Height;
        TargetY = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 20;

        FadingIn = true;
        this.Opacity = 0;

        this.Show();
        TimerSlide.Start();
        TimerClose.Start();
        TadeTimer.Start();

        CurrentPopup = this;
    }

    private void FadeTimer_Tick(object sender, EventArgs e) {
        if (FadingIn) {
            if (this.Opacity < 1)
                this.Opacity += FadeStep;
            else
                TadeTimer.Stop();
        }
        else {
            if (this.Opacity > 0)
                this.Opacity -= FadeStep;
            else {
                TadeTimer.Stop();
                this.Close();
            }
        }
    }


    private void TimerSlide_Tick(object sender, EventArgs e) {
        if (this.Top > TargetY) {
            this.Top -= SlideSpeed;
        }
        else {
            TimerSlide.Stop();
        }
    }

    // Instead of immediately closing, start fade-out
    private void TimerClose_Tick(object sender, EventArgs e) {
        TimerClose.Stop();
        FadingIn = false;
        TadeTimer.Start();
    }
}
