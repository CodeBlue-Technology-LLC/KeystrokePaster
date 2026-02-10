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
        private Label lblInstructions;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtPasteBox = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPasteBox
            // 
            this.txtPasteBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtPasteBox.Location = new System.Drawing.Point(1, 1);
            this.txtPasteBox.Multiline = true;
            this.txtPasteBox.Name = "txtPasteBox";
            this.txtPasteBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPasteBox.Size = new System.Drawing.Size(414, 180);
            this.txtPasteBox.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(4, 204);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(300, 20);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status: Waiting";
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettings.Location = new System.Drawing.Point(377, 184);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(38, 38);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "⚙";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // lblInstructions
            // 
            this.lblInstructions.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblInstructions.ForeColor = System.Drawing.Color.Gray;
            this.lblInstructions.Location = new System.Drawing.Point(3, 184);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(365, 19);
            this.lblInstructions.TabIndex = 3;
            this.lblInstructions.Text = "Paste text above, then press Ctrl+F1 in target window";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(419, 225);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtPasteBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
