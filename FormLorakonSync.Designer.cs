namespace LorakonSync
{
    partial class FormLorakonSync
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLorakonSync));
            this.status = new System.Windows.Forms.StatusStrip();
            this.tabs = new System.Windows.Forms.TabControl();
            this.pageAbout = new System.Windows.Forms.TabPage();
            this.pageSettings = new System.Windows.Forms.TabPage();
            this.pageLog = new System.Windows.Forms.TabPage();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.tabs.SuspendLayout();
            this.pageLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(0, 550);
            this.status.Name = "status";
            this.status.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.status.Size = new System.Drawing.Size(820, 22);
            this.status.TabIndex = 0;
            this.status.Text = "statusStrip1";
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.pageAbout);
            this.tabs.Controls.Add(this.pageSettings);
            this.tabs.Controls.Add(this.pageLog);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(820, 550);
            this.tabs.TabIndex = 3;
            // 
            // pageAbout
            // 
            this.pageAbout.Location = new System.Drawing.Point(4, 25);
            this.pageAbout.Name = "pageAbout";
            this.pageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.pageAbout.Size = new System.Drawing.Size(812, 522);
            this.pageAbout.TabIndex = 0;
            this.pageAbout.Text = "Lorakon Sync";
            this.pageAbout.UseVisualStyleBackColor = true;
            // 
            // pageSettings
            // 
            this.pageSettings.Location = new System.Drawing.Point(4, 25);
            this.pageSettings.Name = "pageSettings";
            this.pageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.pageSettings.Size = new System.Drawing.Size(812, 522);
            this.pageSettings.TabIndex = 1;
            this.pageSettings.Text = "Innstillinger";
            this.pageSettings.UseVisualStyleBackColor = true;
            // 
            // pageLog
            // 
            this.pageLog.Controls.Add(this.lbLog);
            this.pageLog.Location = new System.Drawing.Point(4, 24);
            this.pageLog.Name = "pageLog";
            this.pageLog.Padding = new System.Windows.Forms.Padding(3);
            this.pageLog.Size = new System.Drawing.Size(812, 522);
            this.pageLog.TabIndex = 2;
            this.pageLog.Text = "Logg";
            this.pageLog.UseVisualStyleBackColor = true;
            // 
            // lbLog
            // 
            this.lbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 15;
            this.lbLog.Location = new System.Drawing.Point(3, 3);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(806, 516);
            this.lbLog.TabIndex = 0;
            // 
            // FormLorakonSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 572);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.status);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormLorakonSync";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lorakon Sync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLorakonSync_FormClosing);
            this.Load += new System.EventHandler(this.FormLorakonSync_Load);
            this.tabs.ResumeLayout(false);
            this.pageLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage pageAbout;
        private System.Windows.Forms.TabPage pageSettings;
        private System.Windows.Forms.TabPage pageLog;
        private System.Windows.Forms.ListBox lbLog;
    }
}

