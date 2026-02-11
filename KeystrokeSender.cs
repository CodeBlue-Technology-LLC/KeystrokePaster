using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace KeystrokePaster
{
    public class KeystrokeSender
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        public void SendText(string text, int delayMs)
        {
            foreach (char c in text)
            {
                SendChar(c);
                if (delayMs > 0)
                    Thread.Sleep(delayMs);
            }
        }

        private void SendChar(char c)
        {
            // Handle special characters that need specific key codes
            if (c == '\r' || c == '\n')
            {
                if (c == '\r')
                {
                    // Send Enter key
                    SendKeyPress((byte)Keys.Return);
                }
                return;
            }
            else if (c == '\t')
            {
                // Send Tab key
                SendKeyPress((byte)Keys.Tab);
                return;
            }

            // For regular characters, use VkKeyScan to get the virtual key code
            short vkKeyScan = VkKeyScan(c);

            if (vkKeyScan == -1)
            {
                // Character not in keyboard layout, use Unicode method
                SendUnicodeChar(c);
            }
            else
            {
                byte vk = (byte)(vkKeyScan & 0xFF);
                byte shiftState = (byte)(vkKeyScan >> 8);

                // Press shift if needed
                if ((shiftState & 1) != 0)
                {
                    keybd_event((byte)Keys.ShiftKey, 0, 0, UIntPtr.Zero);
                    Thread.Sleep(10);
                }

                // Press and release the key
                SendKeyPress(vk);

                // Release shift if it was pressed
                if ((shiftState & 1) != 0)
                {
                    Thread.Sleep(10);
                    keybd_event((byte)Keys.ShiftKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
            }
        }

        private void SendUnicodeChar(char c)
        {
            // Send key down
            keybd_event(0, (byte)c, KEYEVENTF_UNICODE, UIntPtr.Zero);
            Thread.Sleep(10);
            // Send key up
            keybd_event(0, (byte)c, KEYEVENTF_UNICODE | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private void SendKeyPress(byte vk)
        {
            // Key down
            keybd_event(vk, 0, 0, UIntPtr.Zero);
            Thread.Sleep(10);
            // Key up
            keybd_event(vk, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}