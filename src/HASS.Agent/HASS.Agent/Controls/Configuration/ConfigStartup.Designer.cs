using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Controls.Configuration
{
    partial class ConfigStartup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigStartup));
            this.LblInfo1 = new System.Windows.Forms.Label();
            this.BtnSetStartOnLogin = new Syncfusion.WinForms.Controls.SfButton();
            this.LblStartOnLoginStatus = new System.Windows.Forms.Label();
            this.LblStartOnLoginStatusInfo = new System.Windows.Forms.Label();
            this.PbLine1 = new System.Windows.Forms.PictureBox();
            this.LblInfo2 = new System.Windows.Forms.Label();
            this.CbDisableVirtualDesktopInitalization = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.PbLine1)).BeginInit();
            this.SuspendLayout();
            // 
            // LblInfo1
            // 
            this.LblInfo1.AccessibleDescription = "Startup information.";
            this.LblInfo1.AccessibleName = "Information";
            this.LblInfo1.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblInfo1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblInfo1.Location = new System.Drawing.Point(70, 36);
            this.LblInfo1.Name = "LblInfo1";
            this.LblInfo1.Size = new System.Drawing.Size(575, 111);
            this.LblInfo1.TabIndex = 15;
            this.LblInfo1.Text = Languages.ConfigStartup_LblInfo1;
            // 
            // BtnSetStartOnLogin
            // 
            this.BtnSetStartOnLogin.AccessibleDescription = "Toggle starting HASS.Agent when logging in to your Windows account.";
            this.BtnSetStartOnLogin.AccessibleName = "Toggle status";
            this.BtnSetStartOnLogin.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnSetStartOnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnSetStartOnLogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnSetStartOnLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnSetStartOnLogin.Location = new System.Drawing.Point(222, 204);
            this.BtnSetStartOnLogin.Name = "BtnSetStartOnLogin";
            this.BtnSetStartOnLogin.Size = new System.Drawing.Size(295, 31);
            this.BtnSetStartOnLogin.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnSetStartOnLogin.Style.FocusedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnSetStartOnLogin.Style.FocusedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnSetStartOnLogin.Style.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnSetStartOnLogin.Style.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnSetStartOnLogin.Style.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnSetStartOnLogin.Style.PressedForeColor = System.Drawing.Color.Black;
            this.BtnSetStartOnLogin.TabIndex = 0;
            this.BtnSetStartOnLogin.Text = global::HASS.Agent.Resources.Localization.Languages.ConfigStartup_BtnSetStartOnLogin;
            this.BtnSetStartOnLogin.UseVisualStyleBackColor = false;
            this.BtnSetStartOnLogin.Click += new System.EventHandler(this.BtnSetStartOnLogin_Click);
            // 
            // LblStartOnLoginStatus
            // 
            this.LblStartOnLoginStatus.AccessibleDescription = "Current start on login status.";
            this.LblStartOnLoginStatus.AccessibleName = "Status";
            this.LblStartOnLoginStatus.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblStartOnLoginStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblStartOnLoginStatus.Location = new System.Drawing.Point(398, 145);
            this.LblStartOnLoginStatus.Name = "LblStartOnLoginStatus";
            this.LblStartOnLoginStatus.Size = new System.Drawing.Size(119, 19);
            this.LblStartOnLoginStatus.TabIndex = 13;
            this.LblStartOnLoginStatus.Text = "-";
            this.LblStartOnLoginStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblStartOnLoginStatusInfo
            // 
            this.LblStartOnLoginStatusInfo.AccessibleDescription = "Start on login status description.";
            this.LblStartOnLoginStatusInfo.AccessibleName = "Status description info";
            this.LblStartOnLoginStatusInfo.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblStartOnLoginStatusInfo.AutoSize = true;
            this.LblStartOnLoginStatusInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblStartOnLoginStatusInfo.Location = new System.Drawing.Point(222, 145);
            this.LblStartOnLoginStatusInfo.Name = "LblStartOnLoginStatusInfo";
            this.LblStartOnLoginStatusInfo.Size = new System.Drawing.Size(139, 19);
            this.LblStartOnLoginStatusInfo.TabIndex = 12;
            this.LblStartOnLoginStatusInfo.Text = Languages.ConfigStartup_LblStartOnLoginStatusInfo;
            this.LblStartOnLoginStatusInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PbLine1
            // 
            this.PbLine1.AccessibleDescription = "Seperator line.";
            this.PbLine1.AccessibleName = "Seperator";
            this.PbLine1.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.PbLine1.Image = global::HASS.Agent.Properties.Resources.line;
            this.PbLine1.Location = new System.Drawing.Point(73, 264);
            this.PbLine1.Name = "PbLine1";
            this.PbLine1.Size = new System.Drawing.Size(576, 1);
            this.PbLine1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PbLine1.TabIndex = 16;
            this.PbLine1.TabStop = false;
            // 
            // LblInfo2
            // 
            this.LblInfo2.AccessibleDescription = "Virtual Desktop library initialization options";
            this.LblInfo2.AccessibleName = "Virtual Desktop";
            this.LblInfo2.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this.LblInfo2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LblInfo2.Location = new System.Drawing.Point(70, 294);
            this.LblInfo2.Name = "LblInfo2";
            this.LblInfo2.Size = new System.Drawing.Size(575, 60);
            this.LblInfo2.TabIndex = 17;
            this.LblInfo2.Text = "Virtual Desktop Library will cause issues on unsupprted systems, enable this option to prevent HASS.Agent from crashing.";
            // 
            // CbDisableVirtualDesktopInitalization
            // 
            this.CbDisableVirtualDesktopInitalization.AccessibleDescription = "Disable Virtual Desktop library initialization.";
            this.CbDisableVirtualDesktopInitalization.AccessibleName = "Disable initialization";
            this.CbDisableVirtualDesktopInitalization.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
            this.CbDisableVirtualDesktopInitalization.AutoSize = true;
            this.CbDisableVirtualDesktopInitalization.Checked = true;
            this.CbDisableVirtualDesktopInitalization.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.CbDisableVirtualDesktopInitalization.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CbDisableVirtualDesktopInitalization.Location = new System.Drawing.Point(73, 364);
            this.CbDisableVirtualDesktopInitalization.Name = "CbDisableVirtualDesktopInitalization";
            this.CbDisableVirtualDesktopInitalization.Size = new System.Drawing.Size(212, 23);
            this.CbDisableVirtualDesktopInitalization.TabIndex = 18;
            this.CbDisableVirtualDesktopInitalization.Text = "Disable Virtual Desktop library initialization";
            this.CbDisableVirtualDesktopInitalization.UseVisualStyleBackColor = true;
            // 
            // ConfigStartup
            // 
            this.AccessibleDescription = "Panel containing the startup configuration.";
            this.AccessibleName = "Startup";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Pane;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.LblInfo1);
            this.Controls.Add(this.BtnSetStartOnLogin);
            this.Controls.Add(this.LblStartOnLoginStatus);
            this.Controls.Add(this.LblStartOnLoginStatusInfo);
            this.Controls.Add(this.PbLine1);
            this.Controls.Add(this.LblInfo2);
            this.Controls.Add(this.CbDisableVirtualDesktopInitalization);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConfigStartup";
            this.Size = new System.Drawing.Size(700, 544);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblInfo1;
        private System.Windows.Forms.Label LblStartOnLoginStatusInfo;
        internal Syncfusion.WinForms.Controls.SfButton BtnSetStartOnLogin;
        internal System.Windows.Forms.Label LblStartOnLoginStatus;
        private PictureBox PbLine1;
        private System.Windows.Forms.Label LblInfo2;
        internal CheckBox CbDisableVirtualDesktopInitalization;
    }
}
