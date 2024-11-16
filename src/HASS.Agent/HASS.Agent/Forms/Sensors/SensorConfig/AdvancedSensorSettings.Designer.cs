
using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Forms.Commands.CommandConfig
{
    partial class AdvancedSensorSettings
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSensorSettings));
            BtnSave = new Syncfusion.WinForms.Controls.SfButton();
            LblInfo1 = new Label();
            LblDeviceClass = new Label();
            TbDeviceClass = new TextBox();
            LblUnitOfMeasurement = new Label();
            TbUnitOfMeasurement = new TextBox();
            LblStateClass = new Label();
            TbStateClass = new TextBox();
            SuspendLayout();
            // 
            // BtnSave
            // 
            BtnSave.AccessibleDescription = "Saves and closes the window.";
            BtnSave.AccessibleName = "Save";
            BtnSave.AccessibleRole = AccessibleRole.PushButton;
            BtnSave.BackColor = Color.FromArgb(63, 63, 70);
            BtnSave.Dock = DockStyle.Bottom;
            BtnSave.Font = new Font("Segoe UI", 10F);
            BtnSave.ForeColor = Color.FromArgb(241, 241, 241);
            BtnSave.Location = new Point(0, 299);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(349, 37);
            BtnSave.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnSave.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnSave.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnSave.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnSave.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnSave.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnSave.Style.PressedForeColor = Color.Black;
            BtnSave.TabIndex = 8;
            BtnSave.Text = Languages.WebViewCommandConfig_BtnSave;
            BtnSave.UseVisualStyleBackColor = false;
            BtnSave.Click += BtnSave_Click;
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "Warning regarding advanced settings";
            LblInfo1.AccessibleName = "Advanced settings warning";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.Font = new Font("Segoe UI", 10F);
            LblInfo1.Location = new Point(12, 13);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(325, 45);
            LblInfo1.TabIndex = 3;
            LblInfo1.Text = Languages.AdvancedSensorConfig_Warning;
            LblInfo1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LblDeviceClass
            // 
            LblDeviceClass.AccessibleDescription = "Device class textbox";
            LblDeviceClass.AccessibleName = "Device class";
            LblDeviceClass.AccessibleRole = AccessibleRole.StaticText;
            LblDeviceClass.AutoSize = true;
            LblDeviceClass.Font = new Font("Segoe UI", 10F);
            LblDeviceClass.Location = new Point(34, 81);
            LblDeviceClass.Name = "LblDeviceClass";
            LblDeviceClass.Size = new Size(81, 19);
            LblDeviceClass.TabIndex = 5;
            LblDeviceClass.Text = Languages.AdvancedSensorConfig_LblDeviceClass;
            // 
            // TbDeviceClass
            // 
            TbDeviceClass.AccessibleDescription = "Device class of the sensor";
            TbDeviceClass.AccessibleName = "URL";
            TbDeviceClass.AccessibleRole = AccessibleRole.Text;
            TbDeviceClass.BackColor = Color.FromArgb(63, 63, 70);
            TbDeviceClass.BorderStyle = BorderStyle.FixedSingle;
            TbDeviceClass.Font = new Font("Segoe UI", 10F);
            TbDeviceClass.ForeColor = Color.FromArgb(241, 241, 241);
            TbDeviceClass.Location = new Point(34, 103);
            TbDeviceClass.Name = "TbDeviceClass";
            TbDeviceClass.Size = new Size(282, 25);
            TbDeviceClass.TabIndex = 0;
            // 
            // LblUnitOfMeasurement
            // 
            LblUnitOfMeasurement.AccessibleDescription = "Unit of measurement textbox";
            LblUnitOfMeasurement.AccessibleName = "Unit of measurement";
            LblUnitOfMeasurement.AccessibleRole = AccessibleRole.StaticText;
            LblUnitOfMeasurement.AutoSize = true;
            LblUnitOfMeasurement.Font = new Font("Segoe UI", 10F);
            LblUnitOfMeasurement.Location = new Point(34, 143);
            LblUnitOfMeasurement.Name = "LblUnitOfMeasurement";
            LblUnitOfMeasurement.Size = new Size(139, 19);
            LblUnitOfMeasurement.TabIndex = 10;
            LblUnitOfMeasurement.Text = Languages.AdvancedSensorConfig_LblUnitOfMeasurement;
            // 
            // TbUnitOfMeasurement
            // 
            TbUnitOfMeasurement.AccessibleDescription = "Device class of the sensor";
            TbUnitOfMeasurement.AccessibleName = "URL";
            TbUnitOfMeasurement.AccessibleRole = AccessibleRole.Text;
            TbUnitOfMeasurement.BackColor = Color.FromArgb(63, 63, 70);
            TbUnitOfMeasurement.BorderStyle = BorderStyle.FixedSingle;
            TbUnitOfMeasurement.Font = new Font("Segoe UI", 10F);
            TbUnitOfMeasurement.ForeColor = Color.FromArgb(241, 241, 241);
            TbUnitOfMeasurement.Location = new Point(34, 165);
            TbUnitOfMeasurement.Name = "TbUnitOfMeasurement";
            TbUnitOfMeasurement.Size = new Size(282, 25);
            TbUnitOfMeasurement.TabIndex = 9;
            // 
            // LblStateClass
            // 
            LblStateClass.AccessibleDescription = "State class textbox";
            LblStateClass.AccessibleName = "State class";
            LblStateClass.AccessibleRole = AccessibleRole.StaticText;
            LblStateClass.AutoSize = true;
            LblStateClass.Font = new Font("Segoe UI", 10F);
            LblStateClass.Location = new Point(34, 206);
            LblStateClass.Name = "LblStateClass";
            LblStateClass.Size = new Size(72, 19);
            LblStateClass.TabIndex = 12;
            LblStateClass.Text = Languages.AdvancedSensorConfig_LblStateClass;
            // 
            // TbStateClass
            // 
            TbStateClass.AccessibleDescription = "State class of the sensor";
            TbStateClass.AccessibleName = "URL";
            TbStateClass.AccessibleRole = AccessibleRole.Text;
            TbStateClass.BackColor = Color.FromArgb(63, 63, 70);
            TbStateClass.BorderStyle = BorderStyle.FixedSingle;
            TbStateClass.Font = new Font("Segoe UI", 10F);
            TbStateClass.ForeColor = Color.FromArgb(241, 241, 241);
            TbStateClass.Location = new Point(34, 228);
            TbStateClass.Name = "TbStateClass";
            TbStateClass.Size = new Size(282, 25);
            TbStateClass.TabIndex = 11;
            // 
            // AdvancedSensorConfig
            // 
            AccessibleDescription = "Configuration for the webview command, like position and url.";
            AccessibleName = "Webview configuration";
            AccessibleRole = AccessibleRole.Window;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            CaptionBarColor = Color.FromArgb(63, 63, 70);
            CaptionFont = new Font("Segoe UI", 10F);
            CaptionForeColor = Color.FromArgb(241, 241, 241);
            ClientSize = new Size(349, 336);
            Controls.Add(LblStateClass);
            Controls.Add(TbStateClass);
            Controls.Add(LblUnitOfMeasurement);
            Controls.Add(TbUnitOfMeasurement);
            Controls.Add(BtnSave);
            Controls.Add(LblDeviceClass);
            Controls.Add(TbDeviceClass);
            Controls.Add(LblInfo1);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(241, 241, 241);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MetroColor = Color.FromArgb(63, 63, 70);
            Name = "AdvancedSensorConfig";
            ShowMaximizeBox = false;
            ShowMinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = Languages.AdvancedSensorConfig_Title;
            Load += AdvancedSensorConfig_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Syncfusion.WinForms.Controls.SfButton BtnSave;
        private System.Windows.Forms.Label LblInfo1;
        private Label LblDeviceClass;
        private TextBox TbDeviceClass;
        private Label LblUnitOfMeasurement;
        private TextBox TbUnitOfMeasurement;
        private Label LblStateClass;
        private TextBox TbStateClass;
    }
}

