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
        private NumericUpDown numDelay;
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
            this.Size = new Size(350, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Hotkey label
            Label lblHotkey = new Label
            {
                Text = "Hotkey:",
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

            // Delay label
            Label lblDelay = new Label
            {
                Text = "Keystroke Delay (ms):",
                Location = new Point(20, 65),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 9F)
            };
            this.Controls.Add(lblDelay);

            // Delay numeric input
            numDelay = new NumericUpDown
            {
                Location = new Point(165, 63),
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
                Location = new Point(20, 95),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 7.5F),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblDelayDesc);

            // OK button
            btnOK = new Button
            {
                Text = "OK",
                Location = new Point(150, 145),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // Cancel button
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(240, 145),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadSettings()
        {
            // Load modifier
            Keys modifier = mainForm.HotkeyModifier;
            if (modifier == Keys.Control)
                cmbModifier.SelectedIndex = 0; // Ctrl
            else if (modifier == Keys.Alt)
                cmbModifier.SelectedIndex = 1; // Alt
            else if (modifier == Keys.Shift)
                cmbModifier.SelectedIndex = 2; // Shift
            else if (modifier == (Keys.Control | Keys.Alt))
                cmbModifier.SelectedIndex = 3; // Ctrl+Alt
            else if (modifier == (Keys.Control | Keys.Shift))
                cmbModifier.SelectedIndex = 4; // Ctrl+Shift
            else
                cmbModifier.SelectedIndex = 0; // Default to Ctrl

            // Load key
            Keys key = mainForm.HotkeyKey;
            if (key >= Keys.F1 && key <= Keys.F12)
            {
                int fKeyNumber = key - Keys.F1;
                cmbKey.SelectedIndex = fKeyNumber;
            }
            else
            {
                cmbKey.SelectedIndex = 0; // Default to F1
            }

            // Load delay
            numDelay.Value = mainForm.KeystrokeDelay;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Save modifier
            switch (cmbModifier.SelectedIndex)
            {
                case 0: mainForm.HotkeyModifier = Keys.Control; break;
                case 1: mainForm.HotkeyModifier = Keys.Alt; break;
                case 2: mainForm.HotkeyModifier = Keys.Shift; break;
                case 3: mainForm.HotkeyModifier = Keys.Control | Keys.Alt; break;
                case 4: mainForm.HotkeyModifier = Keys.Control | Keys.Shift; break;
            }

            // Save key
            mainForm.HotkeyKey = Keys.F1 + cmbKey.SelectedIndex;

            // Save delay
            mainForm.KeystrokeDelay = (int)numDelay.Value;
        }
    }
}
