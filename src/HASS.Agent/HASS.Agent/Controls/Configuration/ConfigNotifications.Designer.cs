using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Controls.Configuration
{
    partial class ConfigNotifications
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigNotifications));
            LblInfo2 = new Label();
            LblInfo1 = new Label();
            BtnNotificationsReadme = new Syncfusion.WinForms.Controls.SfButton();
            CbAcceptNotifications = new CheckBox();
            BtnSendTestNotification = new Syncfusion.WinForms.Controls.SfButton();
            CbNotificationsIgnoreImageCertErrors = new CheckBox();
            LblConnectivityDisabled = new Label();
            CbNotificationsOpenActionUri = new CheckBox();
            SuspendLayout();
            // 
            // LblInfo2
            // 
            LblInfo2.AccessibleDescription = "Debugging info in case the notifications don't work.";
            LblInfo2.AccessibleName = "Debugging info";
            LblInfo2.AccessibleRole = AccessibleRole.StaticText;
            LblInfo2.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo2.Location = new Point(37, 354);
            LblInfo2.Name = "LblInfo2";
            LblInfo2.Size = new Size(643, 153);
            LblInfo2.TabIndex = 37;
            LblInfo2.Text = resources.GetString("LblInfo2.Text");
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "Notifications information.";
            LblInfo1.AccessibleName = "Information";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo1.Location = new Point(70, 36);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(575, 68);
            LblInfo1.TabIndex = 36;
            LblInfo1.Text = resources.GetString("LblInfo1.Text");
            // 
            // BtnNotificationsReadme
            // 
            BtnNotificationsReadme.AccessibleDescription = "Launches the notifications documentation webpage.";
            BtnNotificationsReadme.AccessibleName = "Open documentation";
            BtnNotificationsReadme.AccessibleRole = AccessibleRole.PushButton;
            BtnNotificationsReadme.BackColor = Color.FromArgb(63, 63, 70);
            BtnNotificationsReadme.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BtnNotificationsReadme.ForeColor = Color.FromArgb(241, 241, 241);
            BtnNotificationsReadme.Location = new Point(452, 497);
            BtnNotificationsReadme.Name = "BtnNotificationsReadme";
            BtnNotificationsReadme.Size = new Size(228, 31);
            BtnNotificationsReadme.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnNotificationsReadme.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnNotificationsReadme.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnNotificationsReadme.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnNotificationsReadme.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnNotificationsReadme.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnNotificationsReadme.Style.PressedForeColor = Color.Black;
            BtnNotificationsReadme.TabIndex = 3;
            BtnNotificationsReadme.Text = Languages.ConfigNotifications_BtnNotificationsReadme;
            BtnNotificationsReadme.UseVisualStyleBackColor = false;
            BtnNotificationsReadme.Click += BtnNotificationsReadme_Click;
            // 
            // CbAcceptNotifications
            // 
            CbAcceptNotifications.AccessibleDescription = "Enable the notifications functionality.";
            CbAcceptNotifications.AccessibleName = "Enable notifications";
            CbAcceptNotifications.AccessibleRole = AccessibleRole.CheckButton;
            CbAcceptNotifications.AutoSize = true;
            CbAcceptNotifications.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CbAcceptNotifications.Location = new Point(148, 125);
            CbAcceptNotifications.Name = "CbAcceptNotifications";
            CbAcceptNotifications.Size = new Size(149, 23);
            CbAcceptNotifications.TabIndex = 0;
            CbAcceptNotifications.Text = Languages.ConfigNotifications_CbAcceptNotifications;
            CbAcceptNotifications.UseVisualStyleBackColor = true;
            // 
            // BtnSendTestNotification
            // 
            BtnSendTestNotification.AccessibleDescription = "Show a test notification.";
            BtnSendTestNotification.AccessibleName = "Test notification";
            BtnSendTestNotification.AccessibleRole = AccessibleRole.PushButton;
            BtnSendTestNotification.BackColor = Color.FromArgb(63, 63, 70);
            BtnSendTestNotification.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BtnSendTestNotification.ForeColor = Color.FromArgb(241, 241, 241);
            BtnSendTestNotification.Location = new Point(220, 230);
            BtnSendTestNotification.Name = "BtnSendTestNotification";
            BtnSendTestNotification.Size = new Size(301, 31);
            BtnSendTestNotification.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnSendTestNotification.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnSendTestNotification.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnSendTestNotification.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnSendTestNotification.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnSendTestNotification.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnSendTestNotification.Style.PressedForeColor = Color.Black;
            BtnSendTestNotification.TabIndex = 2;
            BtnSendTestNotification.Text = Languages.ConfigNotifications_BtnSendTestNotification;
            BtnSendTestNotification.UseVisualStyleBackColor = false;
            BtnSendTestNotification.Click += BtnSendTestNotification_Click;
            // 
            // CbNotificationsIgnoreImageCertErrors
            // 
            CbNotificationsIgnoreImageCertErrors.AccessibleDescription = "Download notification images, even when there are certificate errors.";
            CbNotificationsIgnoreImageCertErrors.AccessibleName = "Ignore certificate";
            CbNotificationsIgnoreImageCertErrors.AccessibleRole = AccessibleRole.CheckButton;
            CbNotificationsIgnoreImageCertErrors.AutoSize = true;
            CbNotificationsIgnoreImageCertErrors.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CbNotificationsIgnoreImageCertErrors.Location = new Point(148, 154);
            CbNotificationsIgnoreImageCertErrors.Name = "CbNotificationsIgnoreImageCertErrors";
            CbNotificationsIgnoreImageCertErrors.Size = new Size(238, 23);
            CbNotificationsIgnoreImageCertErrors.TabIndex = 1;
            CbNotificationsIgnoreImageCertErrors.Text = Languages.ConfigNotifications_CbNotificationsIgnoreImageCertErrors;
            CbNotificationsIgnoreImageCertErrors.UseVisualStyleBackColor = true;
            // 
            // LblConnectivityDisabled
            // 
            LblConnectivityDisabled.AccessibleDescription = "Warns that the local api or mqtt needs to be enabled for this to work.";
            LblConnectivityDisabled.AccessibleName = "Connectivity warning";
            LblConnectivityDisabled.AccessibleRole = AccessibleRole.StaticText;
            LblConnectivityDisabled.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblConnectivityDisabled.ForeColor = Color.OrangeRed;
            LblConnectivityDisabled.Location = new Point(19, 293);
            LblConnectivityDisabled.Name = "LblConnectivityDisabled";
            LblConnectivityDisabled.Size = new Size(663, 54);
            LblConnectivityDisabled.TabIndex = 63;
            LblConnectivityDisabled.Text = "both the local API and MQTT are disabled, but the integration needs at least one for it to work";
            LblConnectivityDisabled.TextAlign = ContentAlignment.TopCenter;
            LblConnectivityDisabled.Visible = false;
            // 
            // CbNotificationsOpenActionUri
            // 
            CbNotificationsOpenActionUri.AccessibleDescription = "Treat URI elements of notification elemtns like Android Compaion App Does (open URI)";
            CbNotificationsOpenActionUri.AccessibleName = "Use Android Companion App logic for URI elements.";
            CbNotificationsOpenActionUri.AccessibleRole = AccessibleRole.CheckButton;
            CbNotificationsOpenActionUri.AutoSize = true;
            CbNotificationsOpenActionUri.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CbNotificationsOpenActionUri.Location = new Point(148, 183);
            CbNotificationsOpenActionUri.Name = "CbNotificationsOpenActionUri";
            CbNotificationsOpenActionUri.Size = new Size(446, 23);
            CbNotificationsOpenActionUri.TabIndex = 64;
            CbNotificationsOpenActionUri.Text = Languages.ConfigNotifications_CbNotificationsOpenActionUri;
            CbNotificationsOpenActionUri.UseVisualStyleBackColor = true;
            // 
            // ConfigNotifications
            // 
            AccessibleDescription = "Panel containing the notification integration's configuration.";
            AccessibleName = "Notifications";
            AccessibleRole = AccessibleRole.Pane;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            Controls.Add(CbNotificationsOpenActionUri);
            Controls.Add(LblConnectivityDisabled);
            Controls.Add(CbNotificationsIgnoreImageCertErrors);
            Controls.Add(BtnSendTestNotification);
            Controls.Add(LblInfo1);
            Controls.Add(BtnNotificationsReadme);
            Controls.Add(CbAcceptNotifications);
            Controls.Add(LblInfo2);
            ForeColor = Color.FromArgb(241, 241, 241);
            Margin = new Padding(4);
            Name = "ConfigNotifications";
            Size = new Size(700, 544);
            Load += ConfigNotifications_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label LblInfo2;
        private System.Windows.Forms.Label LblInfo1;
        internal Syncfusion.WinForms.Controls.SfButton BtnNotificationsReadme;
        internal System.Windows.Forms.CheckBox CbAcceptNotifications;
        internal Syncfusion.WinForms.Controls.SfButton BtnSendTestNotification;
        internal CheckBox CbNotificationsIgnoreImageCertErrors;
        private Label LblConnectivityDisabled;
        internal CheckBox CbNotificationsOpenActionUri;
    }
}
