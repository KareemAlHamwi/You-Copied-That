using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

public sealed class PopupForm : Form {
    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    private const int SlideSpeed = 12;
    private const int CornerRadius = 20;
    private const double FadeStep = 0.05;
    private static PopupForm _currentPopup = new();

    private readonly Label _lblMessage;
    private readonly System.Windows.Forms.Timer _timerSlide;
    private readonly System.Windows.Forms.Timer _timerClose;
    private readonly System.Windows.Forms.Timer _fadeTimer;
    private int _targetY;
    private bool _fadingIn = true;

    // Singleton pattern to ensure only one popup at a time
    private PopupForm() {
        if (Environment.OSVersion.Version.Major >= 6) {
            SetProcessDPIAware();
        }

        InitializeForm();

        _lblMessage = new Label {
            AutoSize = true,
            Font = new Font("Segoe UI", 18, FontStyle.Regular),
            ForeColor = Color.White,
            Location = new Point(33, 32)
        };
        Controls.Add(_lblMessage);

        _timerSlide = new System.Windows.Forms.Timer { Interval = 10 };
        _timerSlide.Tick += TimerSlide_Tick;

        _timerClose = new System.Windows.Forms.Timer();
        _timerClose.Tick += TimerClose_Tick;

        _fadeTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _fadeTimer.Tick += FadeTimer_Tick;
    }

    protected override CreateParams CreateParams {
        get {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
            cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
            return cp;
        }
    }

    public static void ShowPopup(string message = "Message", int autoCloseTimeInSeconds = 3) {
        // Close existing popup if any
        _currentPopup?.Close();

        // Create and show new popup
        _currentPopup = new PopupForm();
        _currentPopup.InitializePopup(message, autoCloseTimeInSeconds);
    }

    private void InitializeForm() {
        StartPosition = FormStartPosition.Manual;
        FormBorderStyle = FormBorderStyle.None;
        BackColor = SystemAccentColor.GetAccentColor();
        TopMost = true;
        ShowInTaskbar = false;
        Opacity = 0;
    }

    private void InitializePopup(string message, int autoCloseTimeInSeconds) {
        _lblMessage.Text = message;
        _timerClose.Interval = autoCloseTimeInSeconds * 1000;

        var textSize = TextRenderer.MeasureText(message, _lblMessage.Font);
        const int horizontalPadding = 20;
        const int verticalPadding = 30;

        Size = new Size(textSize.Width + horizontalPadding, textSize.Height + verticalPadding);

        _lblMessage.Location = new Point(
            (Width - textSize.Width) / 2,
            (Height - textSize.Height) / 2
        );

        ApplyRoundedCorners();

        var workingArea = Screen.PrimaryScreen.WorkingArea;
        Left = (workingArea.Width - Width) / 2;
        Top = workingArea.Height;
        _targetY = workingArea.Height - Height - 20;

        _fadingIn = true;
        Opacity = 0;

        Show();
        _timerSlide.Start();
        _timerClose.Start();
        _fadeTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e) {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        base.OnPaint(e);

        using (var path = GetRoundedRectPath(new Rectangle(0, 0, Width, Height), CornerRadius))
        using (var pen = new Pen(BackColor, 1)) {
            e.Graphics.DrawPath(pen, path);
        }
    }

    private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius) {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();

        return path;
    }

    private void ApplyRoundedCorners() {
        using (var path = new GraphicsPath()) {
            var bounds = new Rectangle(0, 0, Width, Height);

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, CornerRadius * 2, CornerRadius * 2, 180, 90);
            path.AddArc(bounds.Right - CornerRadius * 2, bounds.Y, CornerRadius * 2, CornerRadius * 2, 270, 90);
            path.AddArc(bounds.Right - CornerRadius * 2, bounds.Bottom - CornerRadius * 2, CornerRadius * 2, CornerRadius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - CornerRadius * 2, CornerRadius * 2, CornerRadius * 2, 90, 90);
            path.CloseFigure();

            Region = new Region(path);
        }

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        UpdateStyles();
    }

    private void FadeTimer_Tick(object sender, EventArgs e) {
        if (_fadingIn) {
            if (Opacity < 1)
                Opacity += FadeStep;
            else
                _fadeTimer.Stop();
        }
        else {
            if (Opacity > 0)
                Opacity -= FadeStep;
            else {
                _fadeTimer.Stop();
                Close();
            }
        }
    }

    private void TimerSlide_Tick(object sender, EventArgs e) {
        if (Top > _targetY) {
            Top -= SlideSpeed;
        }
        else {
            _timerSlide.Stop();
        }
    }

    private void TimerClose_Tick(object sender, EventArgs e) {
        _timerClose.Stop();
        _fadingIn = false;
        _fadeTimer.Start();
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            _timerSlide?.Dispose();
            _timerClose?.Dispose();
            _fadeTimer?.Dispose();
            _lblMessage?.Dispose();
        }
        base.Dispose(disposing);
    }
}
