using System.Runtime.InteropServices;

public class CtrlCListener : Form {
    [DllImport("user32.dll")]
    private static extern bool AddClipboardFormatListener(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private const int WM_CLIPBOARDUPDATE = 0x031D;
    private const int VK_CONTROL = 0x11;
    private const int VK_C = 0x43;
    private string lastClipboardText = "";
    private bool FirstFunnyMessage;
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
        "Copy... again? Bold move",
        "Trust me or else .."
    };

    public CtrlCListener() {
        this.Load += (s, e) => AddClipboardFormatListener(this.Handle);
        this.FormClosing += (s, e) => RemoveClipboardFormatListener(this.Handle);
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }

    protected override void WndProc(ref Message m) {
        if (m.Msg == WM_CLIPBOARDUPDATE) {
            _HandleCopying();
        }

        base.WndProc(ref m);
    }

    private void _HandleCopying() {
        if (_IsCtrlCPressed()) {
            string CurrentText = Clipboard.GetText();
            string RandomMessage = FunnyMessages[Rand.Next(FunnyMessages.Length)];

            if (CurrentText != lastClipboardText) {
                lastClipboardText = CurrentText;
                PopupForm.ShowPopup("Copied!");
                FirstFunnyMessage = true;
            }
            else if (CurrentText == lastClipboardText && FirstFunnyMessage) {
                PopupForm.ShowPopup(RandomMessage);
                FirstFunnyMessage = false;
            }
        }
    }

    private bool _IsCtrlCPressed() {
        return (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 &&
               (GetAsyncKeyState(VK_C) & 0x8000) != 0;
    }
}
