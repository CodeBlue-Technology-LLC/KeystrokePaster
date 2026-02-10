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
        private const int HOTKEY_RELEASE_DELAY = 200; // ms to wait after hotkey

        public MainForm()
        {
            InitializeComponent();
            InitializeTrayIcon();
            keystrokeSender = new KeystrokeSender();
            RegisterHotkey();
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "Keystroke Paster",
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            };

            // Context menu for tray icon
            ContextMenuStrip trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Show", null, (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            });
            trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
            trayIcon.ContextMenuStrip = trayMenu;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                trayIcon.BalloonTipTitle = "Keystroke Paster";
                trayIcon.BalloonTipText = "Application minimized to tray";
                trayIcon.ShowBalloonTip(1000);
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
                }
            }
            
            // Re-enable TopMost
            this.TopMost = true;
        }

        private void RegisterHotkey()
        {
            // Unregister old hotkey
            if (hotkey != null)
            {
                hotkey.Unregister();
                hotkey.Dispose();
            }

            // Register new hotkey
            try
            {
                hotkey = new GlobalHotkey(HotkeyModifier, HotkeyKey, this);
                hotkey.HotkeyPressed += Hotkey_Pressed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register hotkey: {ex.Message}", "Error", 
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

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
