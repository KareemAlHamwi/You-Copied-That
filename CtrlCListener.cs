using System.Runtime.InteropServices;

public class CtrlCListener : Form {
    [DllImport("user32.dll")]
    private static extern bool AddClipboardFormatListener(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
    private const int WM_CLIPBOARDUPDATE = 0x031D;
    private string LastClipboardText = "";

    private static readonly Random Rand = new Random();
    private static readonly string[] FunnyMessages =
    {
        "You already copied that, genius",
        "Trying to break the Matrix?",
        "Copycat strikes again!",
        "Still the same, buddy",
        "Déjà vu vibes ✨",
        "Ctrl+C fan club president?",
        "Nice try, it's still there",
        "Paste it already!",
        "You're obsessed with this one huh",
        "Copy... again? Bold move"
    };

    public CtrlCListener() {
        this.Load += (s, e) => AddClipboardFormatListener(this.Handle);
        this.FormClosing += (s, e) => RemoveClipboardFormatListener(this.Handle);
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }

    protected override void WndProc(ref Message m) {
        if (m.Msg == WM_CLIPBOARDUPDATE) {
            HandleClipboardChange();
        }

        base.WndProc(ref m);
    }

    private async void HandleClipboardChange() {
        await Task.Delay(100);

        if (Clipboard.ContainsText()) {
            string currentText = Clipboard.GetText();
            if (currentText != LastClipboardText) {
                LastClipboardText = currentText;

                this.BeginInvoke((Action)(() => {
                    PopupForm.ShowPopup("Copied!");
                }));
            }
            else {
                try {
                    string randomMessage = FunnyMessages[Rand.Next(FunnyMessages.Length)];

                    this.BeginInvoke((Action)(() => {
                        PopupForm.ShowPopup(randomMessage);
                    }));
                }
                catch (Exception) {
                    throw;
                }
            }
        }
    }

    // private bool IsCtrlCPressed() {
    //     return (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 &&
    //            (GetAsyncKeyState(VK_C) & 0x8000) != 0;
    // }
}
