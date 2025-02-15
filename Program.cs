using System.Runtime.InteropServices;
using CtrlCPopup;

namespace ClipboardMonitor {
    static class Program {
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClipboardListenerForm());
        }
    }

    public class ClipboardListenerForm : Form {
        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private string lastClipboardText = "";

        public ClipboardListenerForm() {
            this.Load += (s, e) => AddClipboardFormatListener(this.Handle);
            this.FormClosing += (s, e) => RemoveClipboardFormatListener(this.Handle);
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_CLIPBOARDUPDATE) {
                if (Clipboard.ContainsText()) {
                    string currentText = Clipboard.GetText();

                    // TODO: it works on ctrl + c and ctrl + x for sure .. fix it
                    if (currentText != lastClipboardText) {
                        lastClipboardText = currentText;
                        new PopupForm().ShowPopup();
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}
