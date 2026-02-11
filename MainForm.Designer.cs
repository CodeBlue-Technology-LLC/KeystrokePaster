using System.Drawing;
using System.Windows.Forms;

namespace KeystrokePaster
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtPasteBox;
        private Label lblStatus;
        private Button btnSettings;
        private NotifyIcon trayIcon;
        internal Label lblInstructions;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                hotkey?.Dispose();
                trayIcon?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtPasteBox = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPasteBox
            // 
            this.txtPasteBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtPasteBox.Location = new System.Drawing.Point(-1, -2);
            this.txtPasteBox.Multiline = true;
            this.txtPasteBox.Name = "txtPasteBox";
            this.txtPasteBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPasteBox.Size = new System.Drawing.Size(435, 180);
            this.txtPasteBox.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(5, 203);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(300, 20);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status: Waiting";
            // 
            // btnSettings
            // 
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Location = new System.Drawing.Point(388, 181);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(43, 37);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "⚙";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // lblInstructions
            // 
            this.lblInstructions.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblInstructions.ForeColor = System.Drawing.Color.Gray;
            this.lblInstructions.Location = new System.Drawing.Point(5, 185);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(383, 17);
            this.lblInstructions.TabIndex = 3;
            this.lblInstructions.Text = "Paste text above, then press Ctrl+F1 in target window";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(435, 223);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtPasteBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Keystroke Paster";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}