using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeystrokePaster
{
    public class GlobalHotkey : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;

        private IntPtr handle;
        private bool registered = false;

        public event EventHandler HotkeyPressed;

        // Modifier key constants
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        public GlobalHotkey(Keys modifiers, Keys key, Form form)
        {
            handle = form.Handle;

            // Convert modifiers
            uint mod = 0;
            if ((modifiers & Keys.Alt) == Keys.Alt)
                mod |= MOD_ALT;
            if ((modifiers & Keys.Control) == Keys.Control)
                mod |= MOD_CONTROL;
            if ((modifiers & Keys.Shift) == Keys.Shift)
                mod |= MOD_SHIFT;

            // Register the hotkey
            registered = RegisterHotKey(handle, HOTKEY_ID, mod, (uint)key);
            
            if (!registered)
                throw new InvalidOperationException("Failed to register hotkey. It may already be in use.");

            // Hook into the window message handler
            form.Load += (s, e) =>
            {
                NativeWindow window = new HotkeyWindow(this);
                window.AssignHandle(handle);
            };
        }

        internal void OnHotkeyPressed()
        {
            HotkeyPressed?.Invoke(this, EventArgs.Empty);
        }

        public void Unregister()
        {
            if (registered)
            {
                UnregisterHotKey(handle, HOTKEY_ID);
                registered = false;
            }
        }

        public void Dispose()
        {
            Unregister();
        }

        // Internal class to handle Windows messages
        private class HotkeyWindow : NativeWindow
        {
            private GlobalHotkey hotkey;

            public HotkeyWindow(GlobalHotkey hotkey)
            {
                this.hotkey = hotkey;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    hotkey.OnHotkeyPressed();
                }
                base.WndProc(ref m);
            }
        }
    }
}
