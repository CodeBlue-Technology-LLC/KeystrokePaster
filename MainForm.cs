using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace KeystrokePaster
{
    public partial class MainForm : Form
    {
        private GlobalHotkey hotkey;
        private KeystrokeSender keystrokeSender;
        private bool isTyping = false;

        // Settings
        public Keys HotkeyKey { get; set; } = Keys.F1;
        public Keys HotkeyModifier { get; set; } = Keys.Control;
        public int KeystrokeDelay { get; set; } = 10; // milliseconds
        public bool LaunchOnStartup { get; set; } = false;
        private const int HOTKEY_RELEASE_DELAY = 200; // ms to wait after hotkey

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
            LoadStartupSetting();
            RegisterHotkey();
            UpdateInstructionsText();
        }

        private void UpdateInstructionsText()
        {
            string modifier = GetModifierName(HotkeyModifier);
            string key = HotkeyKey.ToString();
            lblInstructions.Text = $"Paste text above, then press {modifier}+{key} in target window";
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
            // Unregister old hotkey first
            if (hotkey != null)
            {
                try
                {
                    hotkey.Unregister();
                    hotkey.Dispose();
                    hotkey = null;
                }
                catch { }
            }

            // Small delay to ensure cleanup
            System.Threading.Thread.Sleep(100);

            // Register new hotkey
            try
            {
                hotkey = new GlobalHotkey(HotkeyModifier, HotkeyKey, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register hotkey: {ex.Message}\n\nTry a different hotkey combination.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Hotkey_Pressed(object sender, EventArgs e)
        {
            if (isTyping || string.IsNullOrEmpty(txtPasteBox.Text))
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
                    keystrokeSender.SendText(txtPasteBox.Text, KeystrokeDelay);
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
                // Hotkey was pressed
                Hotkey_Pressed(this, EventArgs.Empty);
            }

            base.WndProc(ref m);
        }
    }
}