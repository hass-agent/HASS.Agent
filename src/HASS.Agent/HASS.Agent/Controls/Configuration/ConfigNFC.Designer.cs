using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Controls.Configuration
{
    partial class ConfigNFC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigNFC));
            LblInfo1 = new Label();
            LblSelectedScanner = new Label();
            CbNfcScanner = new ComboBox();
            PbLine1 = new PictureBox();
            CbEnableNfc = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)PbLine1).BeginInit();
            SuspendLayout();
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "NFC scanning ability description";
            LblInfo1.AccessibleName = "NFC information";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.AutoEllipsis = true;
            LblInfo1.Font = new Font("Segoe UI", 10F);
            LblInfo1.Location = new Point(70, 36);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(583, 74);
            LblInfo1.TabIndex = 57;
            LblInfo1.Text = resources.GetString("LblInfo1.Text");
            // 
            // LblSelectedScanner
            // 
            LblSelectedScanner.AccessibleDescription = "NFC Scanner";
            LblSelectedScanner.AccessibleName = "NFC Scanner";
            LblSelectedScanner.AccessibleRole = AccessibleRole.StaticText;
            LblSelectedScanner.AutoSize = true;
            LblSelectedScanner.Font = new Font("Segoe UI", 10F);
            LblSelectedScanner.Location = new Point(70, 188);
            LblSelectedScanner.Name = "LblSelectedScanner";
            LblSelectedScanner.Size = new Size(87, 19);
            LblSelectedScanner.TabIndex = 65;
            LblSelectedScanner.Text = "NFC &Scanner";
            // 
            // CbNfcScanner
            // 
            CbNfcScanner.AccessibleDescription = "Selected NFC scanner";
            CbNfcScanner.AccessibleName = "Selected NFC scanner";
            CbNfcScanner.AccessibleRole = AccessibleRole.DropList;
            CbNfcScanner.BackColor = Color.FromArgb(63, 63, 70);
            CbNfcScanner.DrawMode = DrawMode.OwnerDrawFixed;
            CbNfcScanner.DropDownHeight = 300;
            CbNfcScanner.DropDownStyle = ComboBoxStyle.DropDownList;
            CbNfcScanner.Font = new Font("Segoe UI", 9.75F);
            CbNfcScanner.ForeColor = Color.FromArgb(241, 241, 241);
            CbNfcScanner.FormattingEnabled = true;
            CbNfcScanner.IntegralHeight = false;
            CbNfcScanner.Location = new Point(73, 210);
            CbNfcScanner.Name = "CbNfcScanner";
            CbNfcScanner.Size = new Size(358, 26);
            CbNfcScanner.Sorted = true;
            CbNfcScanner.TabIndex = 0;
            // 
            // PbLine1
            // 
            PbLine1.AccessibleDescription = "Seperator line.";
            PbLine1.AccessibleName = "Seperator";
            PbLine1.AccessibleRole = AccessibleRole.Graphic;
            PbLine1.Image = Properties.Resources.line;
            PbLine1.Location = new Point(73, 130);
            PbLine1.Name = "PbLine1";
            PbLine1.Size = new Size(576, 1);
            PbLine1.SizeMode = PictureBoxSizeMode.AutoSize;
            PbLine1.TabIndex = 67;
            PbLine1.TabStop = false;
            // 
            // CbEnableNfc
            // 
            CbEnableNfc.AccessibleDescription = "Enable NFC tag scanning";
            CbEnableNfc.AccessibleName = "Enable NFC";
            CbEnableNfc.AccessibleRole = AccessibleRole.CheckButton;
            CbEnableNfc.AutoSize = true;
            CbEnableNfc.Font = new Font("Segoe UI", 10F);
            CbEnableNfc.Location = new Point(73, 150);
            CbEnableNfc.Name = "CbEnableNfc";
            CbEnableNfc.Size = new Size(180, 23);
            CbEnableNfc.TabIndex = 72;
            CbEnableNfc.Text = "Enable &NFC tag scanning";
            CbEnableNfc.UseVisualStyleBackColor = true;
            // 
            // ConfigNFC
            // 
            AccessibleDescription = "Panel containing general configuration options.";
            AccessibleName = "General configuration";
            AccessibleRole = AccessibleRole.Pane;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            Controls.Add(CbEnableNfc);
            Controls.Add(PbLine1);
            Controls.Add(CbNfcScanner);
            Controls.Add(LblSelectedScanner);
            Controls.Add(LblInfo1);
            ForeColor = Color.FromArgb(241, 241, 241);
            Margin = new Padding(4);
            Name = "ConfigNFC";
            Size = new Size(700, 275);
            Load += ConfigNFC_Load;
            ((System.ComponentModel.ISupportInitialize)PbLine1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label LblInfo1;
        private Label LblSelectedScanner;
        internal ComboBox CbNfcScanner;
        private PictureBox PbLine1;
        internal CheckBox CbEnableNfc;
    }
}
