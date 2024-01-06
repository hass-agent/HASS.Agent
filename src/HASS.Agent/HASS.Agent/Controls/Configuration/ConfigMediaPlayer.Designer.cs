using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Controls.Configuration
{
    partial class ConfigMediaPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigMediaPlayer));
            LblInfo2 = new Label();
            LblInfo1 = new Label();
            BtnMediaPlayerReadme = new Syncfusion.WinForms.Controls.SfButton();
            CbEnableMediaPlayer = new CheckBox();
            LblConnectivityDisabled = new Label();
            CbOptOutWhatsPlaying = new CheckBox();
            SuspendLayout();
            // 
            // LblInfo2
            // 
            LblInfo2.AccessibleDescription = "Debugging info in case the media player doesn't work.";
            LblInfo2.AccessibleName = "Debugging info";
            LblInfo2.AccessibleRole = AccessibleRole.StaticText;
            LblInfo2.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo2.Location = new Point(37, 336);
            LblInfo2.Name = "LblInfo2";
            LblInfo2.Size = new Size(643, 192);
            LblInfo2.TabIndex = 37;
            LblInfo2.Text = resources.GetString("LblInfo2.Text");
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "Media player information.";
            LblInfo1.AccessibleName = "Information";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo1.Location = new Point(70, 36);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(575, 104);
            LblInfo1.TabIndex = 36;
            LblInfo1.Text = resources.GetString("LblInfo1.Text");
            // 
            // BtnMediaPlayerReadme
            // 
            BtnMediaPlayerReadme.AccessibleDescription = "Launches the media player documentation webpage.";
            BtnMediaPlayerReadme.AccessibleName = "Open documentation";
            BtnMediaPlayerReadme.AccessibleRole = AccessibleRole.PushButton;
            BtnMediaPlayerReadme.BackColor = Color.FromArgb(63, 63, 70);
            BtnMediaPlayerReadme.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BtnMediaPlayerReadme.ForeColor = Color.FromArgb(241, 241, 241);
            BtnMediaPlayerReadme.Location = new Point(452, 497);
            BtnMediaPlayerReadme.Name = "BtnMediaPlayerReadme";
            BtnMediaPlayerReadme.Size = new Size(228, 31);
            BtnMediaPlayerReadme.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnMediaPlayerReadme.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnMediaPlayerReadme.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnMediaPlayerReadme.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnMediaPlayerReadme.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnMediaPlayerReadme.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnMediaPlayerReadme.Style.PressedForeColor = Color.Black;
            BtnMediaPlayerReadme.TabIndex = 1;
            BtnMediaPlayerReadme.Text = Languages.ConfigMediaPlayer_BtnMediaPlayerReadme;
            BtnMediaPlayerReadme.UseVisualStyleBackColor = false;
            BtnMediaPlayerReadme.Click += BtnNotificationsReadme_Click;
            // 
            // CbEnableMediaPlayer
            // 
            CbEnableMediaPlayer.AccessibleDescription = "Enable the MediaPlayer functionality.";
            CbEnableMediaPlayer.AccessibleName = "Enable MediaPlayer";
            CbEnableMediaPlayer.AccessibleRole = AccessibleRole.CheckButton;
            CbEnableMediaPlayer.AutoSize = true;
            CbEnableMediaPlayer.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CbEnableMediaPlayer.Location = new Point(232, 174);
            CbEnableMediaPlayer.Name = "CbEnableMediaPlayer";
            CbEnableMediaPlayer.Size = new Size(233, 23);
            CbEnableMediaPlayer.TabIndex = 0;
            CbEnableMediaPlayer.Text = Languages.ConfigMediaPlayer_CbEnableMediaPlayer;
            CbEnableMediaPlayer.UseVisualStyleBackColor = true;
            // 
            // LblConnectivityDisabled
            // 
            LblConnectivityDisabled.AccessibleDescription = "Warns that the local api or mqtt needs to be enabled for this to work.";
            LblConnectivityDisabled.AccessibleName = "Connectivity warning";
            LblConnectivityDisabled.AccessibleRole = AccessibleRole.StaticText;
            LblConnectivityDisabled.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblConnectivityDisabled.ForeColor = Color.OrangeRed;
            LblConnectivityDisabled.Location = new Point(17, 263);
            LblConnectivityDisabled.Name = "LblConnectivityDisabled";
            LblConnectivityDisabled.Size = new Size(663, 54);
            LblConnectivityDisabled.TabIndex = 62;
            LblConnectivityDisabled.Text = "both the local API and MQTT are disabled, but the integration needs at least one for it to work";
            LblConnectivityDisabled.TextAlign = ContentAlignment.TopCenter;
            LblConnectivityDisabled.Visible = false;
            // 
            // CbOptOutWhatsPlaying
            // 
            CbOptOutWhatsPlaying.AccessibleDescription = "Enable the MediaPlayer functionality.";
            CbOptOutWhatsPlaying.AccessibleName = "Enable MediaPlayer";
            CbOptOutWhatsPlaying.AccessibleRole = AccessibleRole.CheckButton;
            CbOptOutWhatsPlaying.AutoSize = true;
            CbOptOutWhatsPlaying.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CbOptOutWhatsPlaying.Location = new Point(232, 217);
            CbOptOutWhatsPlaying.Name = "CbOptOutWhatsPlaying";
            CbOptOutWhatsPlaying.Size = new Size(242, 23);
            CbOptOutWhatsPlaying.TabIndex = 0;
            CbOptOutWhatsPlaying.Text = "&Opt-Out of What's Playing Feature";
            CbOptOutWhatsPlaying.UseVisualStyleBackColor = true;
            // 
            // ConfigMediaPlayer
            // 
            AccessibleDescription = "Panel containing the media player integration's configuration.";
            AccessibleName = "Media player";
            AccessibleRole = AccessibleRole.Pane;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            Controls.Add(LblConnectivityDisabled);
            Controls.Add(LblInfo1);
            Controls.Add(BtnMediaPlayerReadme);
            Controls.Add(CbOptOutWhatsPlaying);
            Controls.Add(CbEnableMediaPlayer);
            Controls.Add(LblInfo2);
            ForeColor = Color.FromArgb(241, 241, 241);
            Margin = new Padding(4);
            Name = "ConfigMediaPlayer";
            Size = new Size(700, 544);
            Load += ConfigMediaPlayer_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label LblInfo2;
        private System.Windows.Forms.Label LblInfo1;
        internal Syncfusion.WinForms.Controls.SfButton BtnMediaPlayerReadme;
        internal System.Windows.Forms.CheckBox CbEnableMediaPlayer;
        private Label LblConnectivityDisabled;
        internal CheckBox CbOptOutWhatsPlaying;
    }
}
