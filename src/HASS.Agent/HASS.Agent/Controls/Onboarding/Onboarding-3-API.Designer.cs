using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Controls.Onboarding
{
    partial class OnboardingApi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnboardingApi));
            PbHassAgentLogo = new PictureBox();
            TbHassApiToken = new TextBox();
            TbHassIp = new TextBox();
            LblApiToken = new Label();
            LblServerUri = new Label();
            LblInfo1 = new Label();
            BtnTest = new Syncfusion.WinForms.Controls.SfButton();
            LblTip1 = new Label();
            ((System.ComponentModel.ISupportInitialize)PbHassAgentLogo).BeginInit();
            SuspendLayout();
            // 
            // PbHassAgentLogo
            // 
            PbHassAgentLogo.AccessibleDescription = "HASS Agent logo image.";
            PbHassAgentLogo.AccessibleName = "HASS Agent logo";
            PbHassAgentLogo.AccessibleRole = AccessibleRole.Graphic;
            PbHassAgentLogo.Cursor = Cursors.Hand;
            PbHassAgentLogo.Image = Properties.Resources.logo_128;
            PbHassAgentLogo.Location = new Point(24, 20);
            PbHassAgentLogo.Name = "PbHassAgentLogo";
            PbHassAgentLogo.Size = new Size(128, 128);
            PbHassAgentLogo.SizeMode = PictureBoxSizeMode.AutoSize;
            PbHassAgentLogo.TabIndex = 2;
            PbHassAgentLogo.TabStop = false;
            // 
            // TbHassApiToken
            // 
            TbHassApiToken.AccessibleDescription = "The API token to use when connecting to your Home Assistant instance.";
            TbHassApiToken.AccessibleName = "API token";
            TbHassApiToken.AccessibleRole = AccessibleRole.Text;
            TbHassApiToken.BackColor = Color.FromArgb(63, 63, 70);
            TbHassApiToken.BorderStyle = BorderStyle.FixedSingle;
            TbHassApiToken.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            TbHassApiToken.ForeColor = Color.FromArgb(241, 241, 241);
            TbHassApiToken.Location = new Point(180, 331);
            TbHassApiToken.Name = "TbHassApiToken";
            TbHassApiToken.Size = new Size(392, 25);
            TbHassApiToken.TabIndex = 1;
            // 
            // TbHassIp
            // 
            TbHassIp.AccessibleDescription = "The URI of your Home Assistant instance. The default should be okay.";
            TbHassIp.AccessibleName = "HA URI";
            TbHassIp.AccessibleRole = AccessibleRole.Text;
            TbHassIp.BackColor = Color.FromArgb(63, 63, 70);
            TbHassIp.BorderStyle = BorderStyle.FixedSingle;
            TbHassIp.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            TbHassIp.ForeColor = Color.FromArgb(241, 241, 241);
            TbHassIp.Location = new Point(180, 243);
            TbHassIp.Name = "TbHassIp";
            TbHassIp.Size = new Size(392, 25);
            TbHassIp.TabIndex = 0;
            TbHassIp.Text = "http://hass.local:8123";
            // 
            // LblApiToken
            // 
            LblApiToken.AccessibleDescription = "API token textbox description.";
            LblApiToken.AccessibleName = "API token info";
            LblApiToken.AccessibleRole = AccessibleRole.StaticText;
            LblApiToken.AutoSize = true;
            LblApiToken.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblApiToken.Location = new Point(180, 309);
            LblApiToken.Name = "LblApiToken";
            LblApiToken.Size = new Size(70, 19);
            LblApiToken.TabIndex = 15;
            LblApiToken.Text = "API &Token";
            // 
            // LblServerUri
            // 
            LblServerUri.AccessibleDescription = "Home Assistant server URI textbox label";
            LblServerUri.AccessibleName = "URI info";
            LblServerUri.AccessibleRole = AccessibleRole.StaticText;
            LblServerUri.AutoSize = true;
            LblServerUri.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblServerUri.Location = new Point(180, 221);
            LblServerUri.Name = "LblServerUri";
            LblServerUri.Size = new Size(214, 19);
            LblServerUri.TabIndex = 14;
            LblServerUri.Text = "Server &URI (should be ok like this)";
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "Home Assistant API information.";
            LblInfo1.AccessibleName = "Information";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo1.Location = new Point(180, 20);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(584, 184);
            LblInfo1.TabIndex = 13;
            LblInfo1.Text = resources.GetString("LblInfo1.Text");
            // 
            // BtnTest
            // 
            BtnTest.AccessibleDescription = "Perform a test connection with your Home Assistant instance.";
            BtnTest.AccessibleName = "Test connection";
            BtnTest.AccessibleRole = AccessibleRole.PushButton;
            BtnTest.BackColor = Color.FromArgb(63, 63, 70);
            BtnTest.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BtnTest.ForeColor = Color.FromArgb(241, 241, 241);
            BtnTest.Location = new Point(392, 362);
            BtnTest.Name = "BtnTest";
            BtnTest.Size = new Size(180, 23);
            BtnTest.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnTest.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnTest.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnTest.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnTest.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnTest.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnTest.Style.PressedForeColor = Color.Black;
            BtnTest.TabIndex = 2;
            BtnTest.Text = Languages.OnboardingApi_BtnTest;
            BtnTest.UseVisualStyleBackColor = false;
            BtnTest.Click += BtnTest_Click;
            // 
            // LblTip1
            // 
            LblTip1.AccessibleDescription = "Contains a configuration tip.";
            LblTip1.AccessibleName = "Configuration tip";
            LblTip1.AccessibleRole = AccessibleRole.StaticText;
            LblTip1.AutoSize = true;
            LblTip1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            LblTip1.Location = new Point(180, 416);
            LblTip1.Name = "LblTip1";
            LblTip1.Size = new Size(361, 13);
            LblTip1.TabIndex = 36;
            LblTip1.Text = "Tip: Specialized settings can be found in the Configuration Window.";
            // 
            // OnboardingApi
            // 
            AccessibleDescription = "Panel containing the onboarding Home Assistant API configuration.";
            AccessibleName = "Home Assistant API";
            AccessibleRole = AccessibleRole.Pane;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            Controls.Add(LblTip1);
            Controls.Add(BtnTest);
            Controls.Add(TbHassApiToken);
            Controls.Add(TbHassIp);
            Controls.Add(LblApiToken);
            Controls.Add(LblServerUri);
            Controls.Add(LblInfo1);
            Controls.Add(PbHassAgentLogo);
            ForeColor = Color.FromArgb(241, 241, 241);
            Margin = new Padding(4);
            Name = "OnboardingApi";
            Size = new Size(803, 457);
            Load += OnboardingApi_Load;
            ((System.ComponentModel.ISupportInitialize)PbHassAgentLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.PictureBox PbHassAgentLogo;
        private System.Windows.Forms.TextBox TbHassApiToken;
        private System.Windows.Forms.TextBox TbHassIp;
        private System.Windows.Forms.Label LblApiToken;
        private System.Windows.Forms.Label LblServerUri;
        private System.Windows.Forms.Label LblInfo1;
        private Syncfusion.WinForms.Controls.SfButton BtnTest;
        private System.Windows.Forms.Label LblTip1;
    }
}
