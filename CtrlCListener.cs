using System.Runtime.InteropServices;
using CtrlCPopup;

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

    public CtrlCListener() {
        this.Load += (s, e) => AddClipboardFormatListener(this.Handle);
        this.FormClosing += (s, e) => RemoveClipboardFormatListener(this.Handle);
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }

    protected override void WndProc(ref Message m) {
        if (m.Msg == WM_CLIPBOARDUPDATE) {
            if (IsCtrlCPressed()) {
                string currentText = Clipboard.GetText();

                if (currentText != lastClipboardText) {
                    lastClipboardText = currentText;
                    new PopupForm().ShowPopup("Copied!");
                }
                //! Might figure it out later
                //? Like a random funny stuff to say if user is paranoid
                // else if (currentText == lastClipboardText) {
                //     new PopupForm().ShowPopup("Dude chillax");
                // }
            }
        }

        base.WndProc(ref m);
    }

    private bool IsCtrlCPressed() {
        return (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 &&
               (GetAsyncKeyState(VK_C) & 0x8000) != 0;
    }
}
