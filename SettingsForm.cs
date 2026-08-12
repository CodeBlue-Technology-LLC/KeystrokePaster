using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeystrokePaster
{
    public partial class SettingsForm : Form
    {
        private MainForm mainForm;
        private ComboBox cmbModifier;
        private ComboBox cmbKey;
        private ComboBox cmbClipModifier;
        private ComboBox cmbClipKey;
        private NumericUpDown numDelay;
        private CheckBox chkLaunchOnStartup;
        private Button btnOK;
        private Button btnCancel;

        public SettingsForm(MainForm parent)
        {
            mainForm = parent;
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            // ClientSize, not Size - control positions below are client coordinates,
            // and the title bar height varies enough to clip the buttons otherwise
            this.ClientSize = new Size(334, 272);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Hotkey label
            Label lblHotkey = new Label
            {
                Text = "Text box:",
                Location = new Point(20, 25),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblHotkey);

            // Modifier dropdown
            cmbModifier = new ComboBox
            {
                Location = new Point(100, 23),
                Size = new Size(90, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbModifier.Items.AddRange(new object[] { "Ctrl", "Alt", "Shift", "Ctrl+Alt", "Ctrl+Shift" });
            this.Controls.Add(cmbModifier);

            // Plus label
            Label lblPlus = new Label
            {
                Text = "+",
                Location = new Point(195, 25),
                Size = new Size(15, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            this.Controls.Add(lblPlus);

            // Key dropdown
            cmbKey = new ComboBox
            {
                Location = new Point(215, 23),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbKey.Items.AddRange(new object[] {
                "F1", "F2", "F3", "F4", "F5", "F6",
                "F7", "F8", "F9", "F10", "F11", "F12"
            });
            this.Controls.Add(cmbKey);

            // Clipboard hotkey label
            Label lblClipHotkey = new Label
            {
                Text = "Clipboard:",
                Location = new Point(20, 60),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblClipHotkey);

            // Clipboard modifier dropdown
            cmbClipModifier = new ComboBox
            {
                Location = new Point(100, 58),
                Size = new Size(90, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbClipModifier.Items.AddRange(new object[] { "Ctrl", "Alt", "Shift", "Ctrl+Alt", "Ctrl+Shift" });
            this.Controls.Add(cmbClipModifier);

            // Plus label
            Label lblClipPlus = new Label
            {
                Text = "+",
                Location = new Point(195, 60),
                Size = new Size(15, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            this.Controls.Add(lblClipPlus);

            // Clipboard key dropdown
            cmbClipKey = new ComboBox
            {
                Location = new Point(215, 58),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbClipKey.Items.AddRange(new object[] {
                "F1", "F2", "F3", "F4", "F5", "F6",
                "F7", "F8", "F9", "F10", "F11", "F12"
            });
            this.Controls.Add(cmbClipKey);

            // Clipboard description
            Label lblClipDesc = new Label
            {
                Text = "Types the clipboard directly - nothing to paste in the box first",
                Location = new Point(20, 85),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 7.5F),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblClipDesc);

            // Delay label
            Label lblDelay = new Label
            {
                Text = "Keystroke Delay (ms):",
                Location = new Point(20, 122),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblDelay);

            // Delay numeric input
            numDelay = new NumericUpDown
            {
                Location = new Point(165, 120),
                Size = new Size(80, 25),
                Minimum = 0,
                Maximum = 1000,
                Increment = 5,
                Value = 10
            };
            this.Controls.Add(numDelay);

            // Delay description
            Label lblDelayDesc = new Label
            {
                Text = "Time to wait between each keystroke\n(0 = fastest, 50+ = safer for slow systems)",
                Location = new Point(20, 152),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 7.5F),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblDelayDesc);

            // Launch on startup checkbox
            chkLaunchOnStartup = new CheckBox
            {
                Text = "Launch on Windows startup",
                Location = new Point(20, 197),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(chkLaunchOnStartup);

            // OK button
            btnOK = new Button
            {
                Text = "OK",
                Location = new Point(150, 227),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // Cancel button
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(240, 227),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadSettings()
        {
            // Load hotkeys
            LoadHotkey(cmbModifier, cmbKey, mainForm.HotkeyModifier, mainForm.HotkeyKey);
            LoadHotkey(cmbClipModifier, cmbClipKey, mainForm.ClipboardHotkeyModifier, mainForm.ClipboardHotkeyKey);

            // Load delay
            numDelay.Value = mainForm.KeystrokeDelay;

            // Load launch on startup
            chkLaunchOnStartup.Checked = mainForm.LaunchOnStartup;
        }

        private void LoadHotkey(ComboBox modifierBox, ComboBox keyBox, Keys modifier, Keys key)
        {
            // Load modifier
            if (modifier == Keys.Control)
                modifierBox.SelectedIndex = 0; // Ctrl
            else if (modifier == Keys.Alt)
                modifierBox.SelectedIndex = 1; // Alt
            else if (modifier == Keys.Shift)
                modifierBox.SelectedIndex = 2; // Shift
            else if (modifier == (Keys.Control | Keys.Alt))
                modifierBox.SelectedIndex = 3; // Ctrl+Alt
            else if (modifier == (Keys.Control | Keys.Shift))
                modifierBox.SelectedIndex = 4; // Ctrl+Shift
            else
                modifierBox.SelectedIndex = 0; // Default to Ctrl

            // Load key
            if (key >= Keys.F1 && key <= Keys.F12)
            {
                int fKeyNumber = key - Keys.F1;
                keyBox.SelectedIndex = fKeyNumber;
            }
            else
            {
                keyBox.SelectedIndex = 0; // Default to F1
            }
        }

        private Keys GetSelectedModifier(ComboBox modifierBox)
        {
            switch (modifierBox.SelectedIndex)
            {
                case 1: return Keys.Alt;
                case 2: return Keys.Shift;
                case 3: return Keys.Control | Keys.Alt;
                case 4: return Keys.Control | Keys.Shift;
                default: return Keys.Control;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Both hotkeys have to be distinct - Windows won't register the same
            // combination twice, so catch it here rather than failing silently later
            if (cmbModifier.SelectedIndex == cmbClipModifier.SelectedIndex &&
                cmbKey.SelectedIndex == cmbClipKey.SelectedIndex)
            {
                MessageBox.Show("The text box hotkey and the clipboard hotkey must be different.",
                    "Duplicate Hotkey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Save hotkeys
            mainForm.HotkeyModifier = GetSelectedModifier(cmbModifier);
            mainForm.HotkeyKey = Keys.F1 + cmbKey.SelectedIndex;
            mainForm.ClipboardHotkeyModifier = GetSelectedModifier(cmbClipModifier);
            mainForm.ClipboardHotkeyKey = Keys.F1 + cmbClipKey.SelectedIndex;

            // Save delay
            mainForm.KeystrokeDelay = (int)numDelay.Value;

            // Save launch on startup
            bool wasEnabled = mainForm.LaunchOnStartup;
            mainForm.LaunchOnStartup = chkLaunchOnStartup.Checked;

            // Update Windows registry for startup
            if (mainForm.LaunchOnStartup != wasEnabled)
            {
                SetStartup(mainForm.LaunchOnStartup);
            }
        }

        private void SetStartup(bool enable)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (enable)
                {
                    // Add to startup
                    string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    key.SetValue("KeystrokePaster", exePath);
                }
                else
                {
                    // Remove from startup
                    key.DeleteValue("KeystrokePaster", false);
                }

                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update startup settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}