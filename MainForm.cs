using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace KeystrokePaster
{
    public partial class MainForm : Form
    {
        private GlobalHotkey hotkey;
        private GlobalHotkey clipboardHotkey;
        private KeystrokeSender keystrokeSender;
        private bool isTyping = false;

        // Settings
        public Keys HotkeyKey { get; set; } = Keys.F1;
        public Keys HotkeyModifier { get; set; } = Keys.Control;
        public Keys ClipboardHotkeyKey { get; set; } = Keys.F2;
        public Keys ClipboardHotkeyModifier { get; set; } = Keys.Control;
        public int KeystrokeDelay { get; set; } = 10; // milliseconds
        public bool LaunchOnStartup { get; set; } = false;
        private const int HOTKEY_RELEASE_DELAY = 200; // ms to wait after hotkey
        private const string SETTINGS_KEY = @"SOFTWARE\KeystrokePaster";

        public MainForm()
        {
            InitializeComponent();

            // Load icon from the executable itself
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            catch
            {
                // If icon fails to load, just use default
            }

            InitializeTrayIcon();
            keystrokeSender = new KeystrokeSender();
            LoadSettings();
            LoadStartupSetting();
            RegisterHotkey();
            UpdateInstructionsText();
        }

        private void LoadSettings()
        {
            try
            {
                using (Microsoft.Win32.RegistryKey key =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SETTINGS_KEY, false))
                {
                    // No key yet means this is a first run - the defaults stand
                    if (key == null)
                        return;

                    Keys boxModifier = ReadKey(key, "HotkeyModifier", HotkeyModifier);
                    Keys boxKey = ReadKey(key, "HotkeyKey", HotkeyKey);
                    Keys clipModifier = ReadKey(key, "ClipboardHotkeyModifier", ClipboardHotkeyModifier);
                    Keys clipKey = ReadKey(key, "ClipboardHotkeyKey", ClipboardHotkeyKey);

                    // Only take stored hotkeys if they're still combinations the
                    // settings dialog can produce, and the two don't collide -
                    // a hand-edited registry shouldn't leave the app with no hotkeys
                    if (IsValidHotkey(boxModifier, boxKey) &&
                        IsValidHotkey(clipModifier, clipKey) &&
                        !(boxModifier == clipModifier && boxKey == clipKey))
                    {
                        HotkeyModifier = boxModifier;
                        HotkeyKey = boxKey;
                        ClipboardHotkeyModifier = clipModifier;
                        ClipboardHotkeyKey = clipKey;
                    }

                    if (key.GetValue("KeystrokeDelay") is int delay && delay >= 0 && delay <= 1000)
                        KeystrokeDelay = delay;
                }
            }
            catch
            {
                // Unreadable settings - carry on with the defaults
            }
        }

        private Keys ReadKey(Microsoft.Win32.RegistryKey key, string name, Keys fallback)
        {
            return (key.GetValue(name) is int value) ? (Keys)value : fallback;
        }

        private bool IsValidHotkey(Keys modifier, Keys key)
        {
            if (key < Keys.F1 || key > Keys.F12)
                return false;

            return modifier == Keys.Control
                || modifier == Keys.Alt
                || modifier == Keys.Shift
                || modifier == (Keys.Control | Keys.Alt)
                || modifier == (Keys.Control | Keys.Shift);
        }

        public void SaveSettings()
        {
            try
            {
                using (Microsoft.Win32.RegistryKey key =
                    Microsoft.Win32.Registry.CurrentUser.CreateSubKey(SETTINGS_KEY))
                {
                    key.SetValue("HotkeyModifier", (int)HotkeyModifier, Microsoft.Win32.RegistryValueKind.DWord);
                    key.SetValue("HotkeyKey", (int)HotkeyKey, Microsoft.Win32.RegistryValueKind.DWord);
                    key.SetValue("ClipboardHotkeyModifier", (int)ClipboardHotkeyModifier, Microsoft.Win32.RegistryValueKind.DWord);
                    key.SetValue("ClipboardHotkeyKey", (int)ClipboardHotkeyKey, Microsoft.Win32.RegistryValueKind.DWord);
                    key.SetValue("KeystrokeDelay", KeystrokeDelay, Microsoft.Win32.RegistryValueKind.DWord);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}\n\nThey'll apply now but won't survive a restart.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateInstructionsText()
        {
            string boxCombo = $"{GetModifierName(HotkeyModifier)}+{HotkeyKey}";
            string clipCombo = $"{GetModifierName(ClipboardHotkeyModifier)}+{ClipboardHotkeyKey}";
            lblInstructions.Text =
                $"{boxCombo} in target window types the box above" + Environment.NewLine +
                $"{clipCombo} types the clipboard directly";
        }

        private string GetModifierName(Keys modifier)
        {
            if (modifier == Keys.Control)
                return "Ctrl";
            else if (modifier == Keys.Alt)
                return "Alt";
            else if (modifier == Keys.Shift)
                return "Shift";
            else if (modifier == (Keys.Control | Keys.Alt))
                return "Ctrl+Alt";
            else if (modifier == (Keys.Control | Keys.Shift))
                return "Ctrl+Shift";
            else
                return modifier.ToString();
        }

        private void LoadStartupSetting()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);

                object value = key?.GetValue("KeystrokePaster");
                LaunchOnStartup = (value != null);
                key?.Close();
            }
            catch
            {
                LaunchOnStartup = false;
            }
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = this.Icon, // Use the form's icon (which should be set from embedded resource)
                Text = "Keystroke Paster",
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) =>
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Activate();
            };

            // Context menu for tray icon
            ContextMenuStrip trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Show", null, (s, e) =>
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Activate();
            });
            trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
            trayIcon.ContextMenuStrip = trayMenu;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Don't hide - just minimize to keep window handle active
                this.ShowInTaskbar = false;
                // Balloon tip removed - no notification when minimizing
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {
            // Empty event handler - added by designer
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            // Temporarily disable TopMost so settings dialog shows on top
            this.TopMost = false;

            using (SettingsForm settingsForm = new SettingsForm(this))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // Remember the new settings for next launch
                    SaveSettings();
                    // Re-register hotkey with new settings
                    RegisterHotkey();
                    // Update the instructions text to show new hotkey
                    UpdateInstructionsText();
                }
            }

            // Re-enable TopMost
            this.TopMost = true;
        }

        private void RegisterHotkey()
        {
            // Unregister old hotkeys first
            ReleaseHotkey(ref hotkey);
            ReleaseHotkey(ref clipboardHotkey);

            // Small delay to ensure cleanup
            System.Threading.Thread.Sleep(100);

            // Register new hotkeys
            try
            {
                hotkey = new GlobalHotkey(HotkeyModifier, HotkeyKey, this, GlobalHotkey.HOTKEY_ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register text box hotkey: {ex.Message}\n\nTry a different hotkey combination.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                clipboardHotkey = new GlobalHotkey(ClipboardHotkeyModifier, ClipboardHotkeyKey, this, GlobalHotkey.HOTKEY_ID_CLIPBOARD);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register clipboard hotkey: {ex.Message}\n\nTry a different hotkey combination.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReleaseHotkey(ref GlobalHotkey target)
        {
            if (target == null)
                return;

            try
            {
                target.Unregister();
                target.Dispose();
            }
            catch { }

            target = null;
        }

        private void Hotkey_Pressed(object sender, EventArgs e)
        {
            TypeText(txtPasteBox.Text);
        }

        private void ClipboardHotkey_Pressed(object sender, EventArgs e)
        {
            if (isTyping)
                return;

            string clipboardText = GetClipboardText();

            if (string.IsNullOrEmpty(clipboardText))
            {
                UpdateStatus("Clipboard is empty (or has no text)", Color.Red);
                return;
            }

            TypeText(clipboardText);
        }

        private string GetClipboardText()
        {
            // The clipboard is a shared resource - another app may have it locked,
            // so give it a couple of tries before giving up.
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    return Clipboard.ContainsText() ? Clipboard.GetText() : null;
                }
                catch
                {
                    System.Threading.Thread.Sleep(50);
                }
            }

            return null;
        }

        private async void TypeText(string text)
        {
            if (isTyping || string.IsNullOrEmpty(text))
                return;

            isTyping = true;
            UpdateStatus("Waiting for hotkey release...", Color.Orange);

            // Wait for hotkey to be released
            await Task.Delay(HOTKEY_RELEASE_DELAY);

            UpdateStatus("Typing...", Color.Green);

            try
            {
                await Task.Run(() =>
                {
                    keystrokeSender.SendText(text, KeystrokeDelay);
                });

                UpdateStatus("Done!", Color.Blue);
                await Task.Delay(2000);
                UpdateStatus("Waiting", Color.Black);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error: {ex.Message}", Color.Red);
            }
            finally
            {
                isTyping = false;
            }
        }

        private void UpdateStatus(string message, Color color)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() =>
                {
                    lblStatus.Text = $"Status: {message}";
                    lblStatus.ForeColor = color;
                }));
            }
            else
            {
                lblStatus.Text = $"Status: {message}";
                lblStatus.ForeColor = color;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Re-register hotkey with new handle
            if (hotkey != null)
            {
                RegisterHotkey();
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
            {
                // WParam carries the id we registered the hotkey under
                int id = m.WParam.ToInt32();

                if (id == GlobalHotkey.HOTKEY_ID)
                    Hotkey_Pressed(this, EventArgs.Empty);
                else if (id == GlobalHotkey.HOTKEY_ID_CLIPBOARD)
                    ClipboardHotkey_Pressed(this, EventArgs.Empty);
            }

            base.WndProc(ref m);
        }
    }
}