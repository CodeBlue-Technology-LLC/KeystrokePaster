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

        public const int HOTKEY_ID = 9000;          // types the text box
        public const int HOTKEY_ID_CLIPBOARD = 9001; // types the clipboard

        private IntPtr handle;
        private int hotkeyId;
        private bool registered = false;

        // Modifier key constants
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN = 0x0008;

        public GlobalHotkey(Keys modifiers, Keys key, Form parentForm)
            : this(modifiers, key, parentForm, HOTKEY_ID)
        {
        }

        public GlobalHotkey(Keys modifiers, Keys key, Form parentForm, int id)
        {
            handle = parentForm.Handle;
            hotkeyId = id;

            // Convert modifiers
            uint mod = 0;
            if ((modifiers & Keys.Alt) == Keys.Alt)
                mod |= MOD_ALT;
            if ((modifiers & Keys.Control) == Keys.Control)
                mod |= MOD_CONTROL;
            if ((modifiers & Keys.Shift) == Keys.Shift)
                mod |= MOD_SHIFT;

            // Register the hotkey
            registered = RegisterHotKey(handle, hotkeyId, mod, (uint)key);

            if (!registered)
                throw new InvalidOperationException("Failed to register hotkey. It may already be in use.");
        }

        public void Unregister()
        {
            if (registered)
            {
                UnregisterHotKey(handle, hotkeyId);
                registered = false;
            }
        }

        public void Dispose()
        {
            Unregister();
        }
    }
}