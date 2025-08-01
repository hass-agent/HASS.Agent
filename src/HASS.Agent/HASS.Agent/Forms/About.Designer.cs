
using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Forms
{
    partial class About
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            BtnClose = new Syncfusion.WinForms.Controls.SfButton();
            LblHassAgentProject = new Label();
            LblInfo1 = new Label();
            LblInfo3 = new Label();
            LblCoreAudio = new Label();
            LblGrapevine = new Label();
            LblHADotNet = new Label();
            LblMicrosoftAppSDK = new Label();
            LblLibreHardwareMonitor = new Label();
            LblHotkeyListener = new Label();
            LblSerilog = new Label();
            LblNewtonsoftJson = new Label();
            LblMQTTnet = new Label();
            LblSyncfusion = new Label();
            LblInfo4 = new Label();
            LblInfo5 = new Label();
            LblInfo2 = new Label();
            LblLab02Research = new Label();
            LblVersion = new Label();
            LblOctokit = new Label();
            LblCliWrap = new Label();
            LblInfo6 = new Label();
            PbHassLogo = new PictureBox();
            PbHassAgentLogo = new PictureBox();
            LblCassia = new Label();
            LblGrpcDotNetNamedPipes = new Label();
            LblGrpc = new Label();
            LblByteSize = new Label();
            PbPayPal = new PictureBox();
            PbKoFi = new PictureBox();
            PbGithubSponsor = new PictureBox();
            PbBMAC = new PictureBox();
            label2 = new Label();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)PbHassLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PbHassAgentLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PbPayPal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PbKoFi).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PbGithubSponsor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PbBMAC).BeginInit();
            SuspendLayout();
            // 
            // BtnClose
            // 
            BtnClose.AccessibleDescription = "Closes the window.";
            BtnClose.AccessibleName = "Close";
            BtnClose.AccessibleRole = AccessibleRole.PushButton;
            BtnClose.BackColor = Color.FromArgb(63, 63, 70);
            BtnClose.Dock = DockStyle.Bottom;
            BtnClose.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BtnClose.ForeColor = Color.FromArgb(241, 241, 241);
            BtnClose.Location = new Point(0, 808);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(747, 37);
            BtnClose.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnClose.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnClose.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnClose.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnClose.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnClose.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnClose.Style.PressedForeColor = Color.Black;
            BtnClose.TabIndex = 0;
            BtnClose.Text = Languages.About_BtnClose;
            BtnClose.UseVisualStyleBackColor = false;
            BtnClose.Click += BtnClose_Click;
            // 
            // LblHassAgentProject
            // 
            LblHassAgentProject.AccessibleDescription = "Application name.";
            LblHassAgentProject.AccessibleName = "Name";
            LblHassAgentProject.AccessibleRole = AccessibleRole.StaticText;
            LblHassAgentProject.AutoSize = true;
            LblHassAgentProject.Cursor = Cursors.Hand;
            LblHassAgentProject.Font = new Font("Segoe UI Semibold", 26.25F, FontStyle.Bold, GraphicsUnit.Point);
            LblHassAgentProject.Location = new Point(166, 12);
            LblHassAgentProject.Name = "LblHassAgentProject";
            LblHassAgentProject.Size = new Size(213, 47);
            LblHassAgentProject.TabIndex = 2;
            LblHassAgentProject.Text = "HASS.Agent";
            LblHassAgentProject.Click += LblHassAgentProject_Click;
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "Application description.";
            LblInfo1.AccessibleName = "Description";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.AutoSize = true;
            LblInfo1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo1.Location = new Point(176, 70);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(360, 19);
            LblInfo1.TabIndex = 3;
            LblInfo1.Text = Resources.Localization.Languages.About_LblInfo1;
            // 
            // LblInfo3
            // 
            LblInfo3.AccessibleDescription = "Used components information.";
            LblInfo3.AccessibleName = "Components info";
            LblInfo3.AccessibleRole = AccessibleRole.StaticText;
            LblInfo3.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo3.Location = new Point(176, 175);
            LblInfo3.Name = "LblInfo3";
            LblInfo3.Size = new Size(536, 38);
            LblInfo3.TabIndex = 4;
            LblInfo3.Text =  Resources.Localization.Languages.About_LblInfo3;
            // 
            // LblCoreAudio
            // 
            LblCoreAudio.AccessibleDescription = "Opens the 'CoreAudio' website.";
            LblCoreAudio.AccessibleName = "CoreAudio";
            LblCoreAudio.AccessibleRole = AccessibleRole.Link;
            LblCoreAudio.AutoSize = true;
            LblCoreAudio.Cursor = Cursors.Hand;
            LblCoreAudio.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblCoreAudio.Location = new Point(176, 227);
            LblCoreAudio.Name = "LblCoreAudio";
            LblCoreAudio.Size = new Size(74, 19);
            LblCoreAudio.TabIndex = 5;
            LblCoreAudio.Text = "CoreAudio";
            LblCoreAudio.Click += LblCoreAudio_Click;
            // 
            // LblGrapevine
            // 
            LblGrapevine.AccessibleDescription = "Opens the 'Grapevine' website.";
            LblGrapevine.AccessibleName = "Grapevine";
            LblGrapevine.AccessibleRole = AccessibleRole.Link;
            LblGrapevine.AutoSize = true;
            LblGrapevine.Cursor = Cursors.Hand;
            LblGrapevine.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblGrapevine.Location = new Point(294, 227);
            LblGrapevine.Name = "LblGrapevine";
            LblGrapevine.Size = new Size(71, 19);
            LblGrapevine.TabIndex = 6;
            LblGrapevine.Text = "Grapevine";
            LblGrapevine.Click += LblGrapevine_Click;
            // 
            // LblHADotNet
            // 
            LblHADotNet.AccessibleDescription = "Opens the 'HADotNet' website.";
            LblHADotNet.AccessibleName = "HADotNet";
            LblHADotNet.AccessibleRole = AccessibleRole.Link;
            LblHADotNet.AutoSize = true;
            LblHADotNet.Cursor = Cursors.Hand;
            LblHADotNet.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblHADotNet.Location = new Point(294, 377);
            LblHADotNet.Name = "LblHADotNet";
            LblHADotNet.Size = new Size(73, 19);
            LblHADotNet.TabIndex = 7;
            LblHADotNet.Text = "HADotNet";
            LblHADotNet.Click += LblHADotNet_Click;
            // 
            // LblMicrosoftAppSDK
            // 
            LblMicrosoftAppSDK.AccessibleDescription = "Opens the 'Microsoft App SDK' website.";
            LblMicrosoftAppSDK.AccessibleName = "MicrosoftAppSDK";
            LblMicrosoftAppSDK.AccessibleRole = AccessibleRole.Link;
            LblMicrosoftAppSDK.AutoSize = true;
            LblMicrosoftAppSDK.Cursor = Cursors.Hand;
            LblMicrosoftAppSDK.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblMicrosoftAppSDK.Location = new Point(455, 227);
            LblMicrosoftAppSDK.Name = "LblMicrosoftAppSDK";
            LblMicrosoftAppSDK.Size = new Size(176, 19);
            LblMicrosoftAppSDK.TabIndex = 10;
            LblMicrosoftAppSDK.Text = "Microsoft.WindowsAppSDK";
            LblMicrosoftAppSDK.Click += LblMicrosoftAppSDK_Click;
            // 
            // LblLibreHardwareMonitor
            // 
            LblLibreHardwareMonitor.AccessibleDescription = "Opens the 'LibreHardwareMonitor' website.";
            LblLibreHardwareMonitor.AccessibleName = "LibreHardwareMonitor";
            LblLibreHardwareMonitor.AccessibleRole = AccessibleRole.Link;
            LblLibreHardwareMonitor.AutoSize = true;
            LblLibreHardwareMonitor.Cursor = Cursors.Hand;
            LblLibreHardwareMonitor.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblLibreHardwareMonitor.Location = new Point(294, 257);
            LblLibreHardwareMonitor.Name = "LblLibreHardwareMonitor";
            LblLibreHardwareMonitor.Size = new Size(148, 19);
            LblLibreHardwareMonitor.TabIndex = 9;
            LblLibreHardwareMonitor.Text = "LibreHardwareMonitor";
            LblLibreHardwareMonitor.Click += LblLibreHardwareMonitor_Click;
            // 
            // LblHotkeyListener
            // 
            LblHotkeyListener.AccessibleDescription = "Opens the 'HotkeyListener' website.";
            LblHotkeyListener.AccessibleName = "HotkeyListener";
            LblHotkeyListener.AccessibleRole = AccessibleRole.Link;
            LblHotkeyListener.AutoSize = true;
            LblHotkeyListener.Cursor = Cursors.Hand;
            LblHotkeyListener.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblHotkeyListener.Location = new Point(176, 257);
            LblHotkeyListener.Name = "LblHotkeyListener";
            LblHotkeyListener.Size = new Size(101, 19);
            LblHotkeyListener.TabIndex = 8;
            LblHotkeyListener.Text = "HotkeyListener";
            LblHotkeyListener.Click += LblHotkeyListener_Click;
            // 
            // LblSerilog
            // 
            LblSerilog.AccessibleDescription = "Opens the 'Serilog' website.";
            LblSerilog.AccessibleName = "Serilog";
            LblSerilog.AccessibleRole = AccessibleRole.Link;
            LblSerilog.AutoSize = true;
            LblSerilog.Cursor = Cursors.Hand;
            LblSerilog.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblSerilog.Location = new Point(294, 317);
            LblSerilog.Name = "LblSerilog";
            LblSerilog.Size = new Size(50, 19);
            LblSerilog.TabIndex = 13;
            LblSerilog.Text = "Serilog";
            LblSerilog.Click += LblSerilog_Click;
            // 
            // LblNewtonsoftJson
            // 
            LblNewtonsoftJson.AccessibleDescription = "Opens the 'NewtonsoftJson' website.";
            LblNewtonsoftJson.AccessibleName = "NewtonsoftJson";
            LblNewtonsoftJson.AccessibleRole = AccessibleRole.Link;
            LblNewtonsoftJson.AutoSize = true;
            LblNewtonsoftJson.Cursor = Cursors.Hand;
            LblNewtonsoftJson.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblNewtonsoftJson.Location = new Point(294, 287);
            LblNewtonsoftJson.Name = "LblNewtonsoftJson";
            LblNewtonsoftJson.Size = new Size(110, 19);
            LblNewtonsoftJson.TabIndex = 12;
            LblNewtonsoftJson.Text = "Newtonsoft.Json";
            LblNewtonsoftJson.Click += LblNewtonsoftJson_Click;
            // 
            // LblMQTTnet
            // 
            LblMQTTnet.AccessibleDescription = "Opens the 'MQTTnet' website.";
            LblMQTTnet.AccessibleName = "MQTTnet";
            LblMQTTnet.AccessibleRole = AccessibleRole.Link;
            LblMQTTnet.AutoSize = true;
            LblMQTTnet.Cursor = Cursors.Hand;
            LblMQTTnet.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblMQTTnet.Location = new Point(176, 287);
            LblMQTTnet.Name = "LblMQTTnet";
            LblMQTTnet.Size = new Size(65, 19);
            LblMQTTnet.TabIndex = 11;
            LblMQTTnet.Text = "MQTTnet";
            LblMQTTnet.Click += LblMQTTnet_Click;
            // 
            // LblSyncfusion
            // 
            LblSyncfusion.AccessibleDescription = "Opens the 'Syncfusion' website.";
            LblSyncfusion.AccessibleName = "Syncfusion";
            LblSyncfusion.AccessibleRole = AccessibleRole.Link;
            LblSyncfusion.AutoSize = true;
            LblSyncfusion.Cursor = Cursors.Hand;
            LblSyncfusion.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblSyncfusion.Location = new Point(176, 317);
            LblSyncfusion.Name = "LblSyncfusion";
            LblSyncfusion.Size = new Size(74, 19);
            LblSyncfusion.TabIndex = 14;
            LblSyncfusion.Text = "Syncfusion";
            LblSyncfusion.Click += LblSyncfusion_Click;
            // 
            // LblInfo4
            // 
            LblInfo4.AccessibleDescription = "Components thanks message.";
            LblInfo4.AccessibleName = "Components thanks";
            LblInfo4.AccessibleRole = AccessibleRole.StaticText;
            LblInfo4.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo4.Location = new Point(176, 418);
            LblInfo4.Name = "LblInfo4";
            LblInfo4.Size = new Size(536, 38);
            LblInfo4.TabIndex = 16;
            LblInfo4.Text = Resources.Localization.Languages.About_LblInfo4;
            // 
            // LblInfo5
            // 
            LblInfo5.AccessibleDescription = "Home Assistant thanks message.";
            LblInfo5.AccessibleName = "HA thanks";
            LblInfo5.AccessibleRole = AccessibleRole.StaticText;
            LblInfo5.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo5.Location = new Point(176, 541);
            LblInfo5.Name = "LblInfo5";
            LblInfo5.Size = new Size(536, 38);
            LblInfo5.TabIndex = 18;
            LblInfo5.Text = Resources.Localization.Languages.About_LblInfo5;
            // 
            // LblInfo2
            // 
            LblInfo2.AccessibleDescription = "Created info.";
            LblInfo2.AccessibleName = "Created info";
            LblInfo2.AccessibleRole = AccessibleRole.StaticText;
            LblInfo2.AutoSize = true;
            LblInfo2.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo2.Location = new Point(176, 135);
            LblInfo2.Name = "LblInfo2";
            LblInfo2.Size = new Size(253, 19);
            LblInfo2.TabIndex = 21;
            LblInfo2.Text = Resources.Localization.Languages.About_LblInfo2;
            // 
            // LblLab02Research
            // 
            LblLab02Research.AccessibleDescription = "Created by link. Opens the LAB02 Research webpage.";
            LblLab02Research.AccessibleName = "Created by";
            LblLab02Research.AccessibleRole = AccessibleRole.Link;
            LblLab02Research.AutoSize = true;
            LblLab02Research.Cursor = Cursors.Hand;
            LblLab02Research.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblLab02Research.Location = new Point(547, 135);
            LblLab02Research.Name = "LblLab02Research";
            LblLab02Research.Size = new Size(107, 19);
            LblLab02Research.TabIndex = 22;
            LblLab02Research.Text = "LAB02 Research";
            LblLab02Research.Click += LblLab02Research_Click;
            // 
            // LblVersion
            // 
            LblVersion.AccessibleDescription = "HASS.Agent's current version.";
            LblVersion.AccessibleName = "Version";
            LblVersion.AccessibleRole = AccessibleRole.StaticText;
            LblVersion.AutoSize = true;
            LblVersion.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LblVersion.Location = new Point(12, 154);
            LblVersion.Name = "LblVersion";
            LblVersion.Size = new Size(12, 15);
            LblVersion.TabIndex = 24;
            LblVersion.Text = "-";
            // 
            // LblOctokit
            // 
            LblOctokit.AccessibleDescription = "Opens the 'Octokit' website.";
            LblOctokit.AccessibleName = "Octokit";
            LblOctokit.AccessibleRole = AccessibleRole.Link;
            LblOctokit.AutoSize = true;
            LblOctokit.Cursor = Cursors.Hand;
            LblOctokit.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblOctokit.Location = new Point(175, 347);
            LblOctokit.Name = "LblOctokit";
            LblOctokit.Size = new Size(54, 19);
            LblOctokit.TabIndex = 26;
            LblOctokit.Text = "Octokit";
            LblOctokit.Click += LblOctokit_Click;
            // 
            // LblCliWrap
            // 
            LblCliWrap.AccessibleDescription = "Opens the 'CliWrap' website.";
            LblCliWrap.AccessibleName = "CliWrap";
            LblCliWrap.AccessibleRole = AccessibleRole.Link;
            LblCliWrap.AutoSize = true;
            LblCliWrap.Cursor = Cursors.Hand;
            LblCliWrap.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblCliWrap.Location = new Point(294, 347);
            LblCliWrap.Name = "LblCliWrap";
            LblCliWrap.Size = new Size(57, 19);
            LblCliWrap.TabIndex = 27;
            LblCliWrap.Text = "CliWrap";
            LblCliWrap.Click += LblCliWrap_Click;
            // 
            // LblInfo6
            // 
            LblInfo6.AccessibleDescription = "Donating for HASS.Agent message.";
            LblInfo6.AccessibleName = "Donating info";
            LblInfo6.AccessibleRole = AccessibleRole.StaticText;
            LblInfo6.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo6.Location = new Point(173, 580);
            LblInfo6.Name = "LblInfo6";
            LblInfo6.Size = new Size(562, 91);
            LblInfo6.TabIndex = 28;
            LblInfo6.Text = Resources.Localization.Languages.About_LblInfo6;
            LblInfo6.TextAlign = ContentAlignment.BottomLeft;
            // 
            // PbHassLogo
            // 
            PbHassLogo.AccessibleDescription = "Home Assistant logo.";
            PbHassLogo.AccessibleName = "HA logo";
            PbHassLogo.AccessibleRole = AccessibleRole.Graphic;
            PbHassLogo.Cursor = Cursors.Hand;
            PbHassLogo.Image = (Image)resources.GetObject("PbHassLogo.Image");
            PbHassLogo.Location = new Point(12, 499);
            PbHassLogo.Name = "PbHassLogo";
            PbHassLogo.Size = new Size(128, 128);
            PbHassLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            PbHassLogo.TabIndex = 17;
            PbHassLogo.TabStop = false;
            PbHassLogo.Click += PbHassLogo_Click;
            // 
            // PbHassAgentLogo
            // 
            PbHassAgentLogo.AccessibleDescription = "HASS.Agent logo.";
            PbHassAgentLogo.AccessibleName = "HASS.Agent logo";
            PbHassAgentLogo.AccessibleRole = AccessibleRole.Graphic;
            PbHassAgentLogo.Cursor = Cursors.Hand;
            PbHassAgentLogo.Image = (Image)resources.GetObject("PbHassAgentLogo.Image");
            PbHassAgentLogo.Location = new Point(12, 23);
            PbHassAgentLogo.Name = "PbHassAgentLogo";
            PbHassAgentLogo.Size = new Size(128, 128);
            PbHassAgentLogo.SizeMode = PictureBoxSizeMode.AutoSize;
            PbHassAgentLogo.TabIndex = 1;
            PbHassAgentLogo.TabStop = false;
            PbHassAgentLogo.Click += PbHassAgentLogo_Click;
            // 
            // LblCassia
            // 
            LblCassia.AccessibleDescription = "Opens the 'Cassia' website.";
            LblCassia.AccessibleName = "Cassia";
            LblCassia.AccessibleRole = AccessibleRole.Link;
            LblCassia.AutoSize = true;
            LblCassia.Cursor = Cursors.Hand;
            LblCassia.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblCassia.Location = new Point(176, 377);
            LblCassia.Name = "LblCassia";
            LblCassia.Size = new Size(47, 19);
            LblCassia.TabIndex = 30;
            LblCassia.Text = "Cassia";
            LblCassia.Click += LblCassia_Click;
            // 
            // LblGrpcDotNetNamedPipes
            // 
            LblGrpcDotNetNamedPipes.AccessibleDescription = "Opens the 'GrpcDotNetNamedPipes' website.";
            LblGrpcDotNetNamedPipes.AccessibleName = "GrpcDotNetNamedPipes";
            LblGrpcDotNetNamedPipes.AccessibleRole = AccessibleRole.Link;
            LblGrpcDotNetNamedPipes.AutoSize = true;
            LblGrpcDotNetNamedPipes.Cursor = Cursors.Hand;
            LblGrpcDotNetNamedPipes.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblGrpcDotNetNamedPipes.Location = new Point(455, 257);
            LblGrpcDotNetNamedPipes.Name = "LblGrpcDotNetNamedPipes";
            LblGrpcDotNetNamedPipes.Size = new Size(159, 19);
            LblGrpcDotNetNamedPipes.TabIndex = 31;
            LblGrpcDotNetNamedPipes.Text = "GrpcDotNetNamedPipes";
            LblGrpcDotNetNamedPipes.Click += LblGrpcDotNetNamedPipes_Click;
            // 
            // LblGrpc
            // 
            LblGrpc.AccessibleDescription = "Opens the 'gRPC' website.";
            LblGrpc.AccessibleName = "Grpc";
            LblGrpc.AccessibleRole = AccessibleRole.Link;
            LblGrpc.AutoSize = true;
            LblGrpc.Cursor = Cursors.Hand;
            LblGrpc.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblGrpc.Location = new Point(455, 287);
            LblGrpc.Name = "LblGrpc";
            LblGrpc.Size = new Size(42, 19);
            LblGrpc.TabIndex = 34;
            LblGrpc.Text = "gRPC";
            LblGrpc.Click += LblGrpc_Click;
            // 
            // LblByteSize
            // 
            LblByteSize.AccessibleDescription = "Opens the 'ByteSize' website.";
            LblByteSize.AccessibleName = "ByteSize";
            LblByteSize.AccessibleRole = AccessibleRole.Link;
            LblByteSize.AutoSize = true;
            LblByteSize.Cursor = Cursors.Hand;
            LblByteSize.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            LblByteSize.Location = new Point(455, 317);
            LblByteSize.Name = "LblByteSize";
            LblByteSize.Size = new Size(59, 19);
            LblByteSize.TabIndex = 35;
            LblByteSize.Text = "ByteSize";
            LblByteSize.Click += LblByteSize_Click;
            // 
            // PbPayPal
            // 
            PbPayPal.AccessibleDescription = "Opens the 'paypal' donation website.";
            PbPayPal.AccessibleName = "PayPal donation";
            PbPayPal.AccessibleRole = AccessibleRole.PushButton;
            PbPayPal.Cursor = Cursors.Hand;
            PbPayPal.Image = (Image)resources.GetObject("PbPayPal.Image");
            PbPayPal.Location = new Point(359, 684);
            PbPayPal.Name = "PbPayPal";
            PbPayPal.Size = new Size(152, 43);
            PbPayPal.SizeMode = PictureBoxSizeMode.AutoSize;
            PbPayPal.TabIndex = 43;
            PbPayPal.TabStop = false;
            PbPayPal.Click += PbPayPal_Click;
            // 
            // PbKoFi
            // 
            PbKoFi.AccessibleDescription = "Opens the 'ko-fi' donation website.";
            PbKoFi.AccessibleName = "Kofi donation";
            PbKoFi.AccessibleRole = AccessibleRole.PushButton;
            PbKoFi.Cursor = Cursors.Hand;
            PbKoFi.Image = (Image)resources.GetObject("PbKoFi.Image");
            PbKoFi.Location = new Point(541, 684);
            PbKoFi.Name = "PbKoFi";
            PbKoFi.Size = new Size(171, 43);
            PbKoFi.SizeMode = PictureBoxSizeMode.AutoSize;
            PbKoFi.TabIndex = 42;
            PbKoFi.TabStop = false;
            PbKoFi.Click += PbKoFi_Click;
            // 
            // PbGithubSponsor
            // 
            PbGithubSponsor.AccessibleDescription = "Opens the 'sponsor on gituhb' donation website.";
            PbGithubSponsor.AccessibleName = "GitHub donation";
            PbGithubSponsor.AccessibleRole = AccessibleRole.PushButton;
            PbGithubSponsor.Cursor = Cursors.Hand;
            PbGithubSponsor.Image = (Image)resources.GetObject("PbGithubSponsor.Image");
            PbGithubSponsor.Location = new Point(176, 748);
            PbGithubSponsor.Name = "PbGithubSponsor";
            PbGithubSponsor.Size = new Size(235, 43);
            PbGithubSponsor.SizeMode = PictureBoxSizeMode.AutoSize;
            PbGithubSponsor.TabIndex = 41;
            PbGithubSponsor.TabStop = false;
            PbGithubSponsor.Click += PbGithubSponsor_Click;
            // 
            // PbBMAC
            // 
            PbBMAC.AccessibleDescription = "Opens the 'buy me a coffee' donation website.";
            PbBMAC.AccessibleName = "BMAC donation";
            PbBMAC.AccessibleRole = AccessibleRole.PushButton;
            PbBMAC.Cursor = Cursors.Hand;
            PbBMAC.Image = (Image)resources.GetObject("PbBMAC.Image");
            PbBMAC.Location = new Point(176, 684);
            PbBMAC.Name = "PbBMAC";
            PbBMAC.Size = new Size(153, 43);
            PbBMAC.SizeMode = PictureBoxSizeMode.AutoSize;
            PbBMAC.TabIndex = 40;
            PbBMAC.TabStop = false;
            PbBMAC.Click += PbBMAC_Click;
            // 
            // label2
            // 
            label2.AccessibleDescription = "Created by link. Opens the unofficial github webpage.";
            label2.AccessibleName = "Created by";
            label2.AccessibleRole = AccessibleRole.Link;
            label2.AutoSize = true;
            label2.Cursor = Cursors.Hand;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            label2.Location = new Point(536, 112);
            label2.Name = "label2";
            label2.Size = new Size(118, 19);
            label2.TabIndex = 45;
            label2.Text = "HASS.Agent Team";
            label2.Click += label2_Click;
            // 
            // label1
            // 
            label1.AccessibleDescription = "Created info.";
            label1.AccessibleName = "Created info";
            label1.AccessibleRole = AccessibleRole.StaticText;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(176, 112);
            label1.Name = "label1";
            label1.Size = new Size(180, 19);
            label1.TabIndex = 46;
            label1.Text = Resources.Localization.Languages.About_LblInfo2_1;
            // 
            // label3
            // 
            label3.AccessibleDescription = "Components thanks message.";
            label3.AccessibleName = "Components thanks";
            label3.AccessibleRole = AccessibleRole.StaticText;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(176, 477);
            label3.Name = "label3";
            label3.Size = new Size(536, 38);
            label3.TabIndex = 47;
            label3.Text = Resources.Localization.Languages.About_LblInfo4_1;
            // 
            // label4
            // 
            label4.AccessibleDescription = "Opens the 'ByteSize' website.";
            label4.AccessibleName = "ByteSize";
            label4.AccessibleRole = AccessibleRole.Link;
            label4.AutoSize = true;
            label4.Cursor = Cursors.Hand;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
            label4.Location = new Point(455, 347);
            label4.Name = "label4";
            label4.Size = new Size(100, 19);
            label4.TabIndex = 48;
            label4.Text = "VirtualDesktop";
            label4.Click += label4_Click;
            // 
            // About
            // 
            AccessibleDescription = "General info about HASS.Agent.";
            AccessibleName = "About";
            AccessibleRole = AccessibleRole.Window;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            CaptionBarColor = Color.FromArgb(63, 63, 70);
            CaptionFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CaptionForeColor = Color.FromArgb(241, 241, 241);
            ClientSize = new Size(747, 845);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(PbPayPal);
            Controls.Add(PbKoFi);
            Controls.Add(PbGithubSponsor);
            Controls.Add(PbBMAC);
            Controls.Add(LblByteSize);
            Controls.Add(LblGrpc);
            Controls.Add(LblGrpcDotNetNamedPipes);
            Controls.Add(LblCassia);
            Controls.Add(LblInfo6);
            Controls.Add(LblCliWrap);
            Controls.Add(LblOctokit);
            Controls.Add(LblVersion);
            Controls.Add(LblLab02Research);
            Controls.Add(LblInfo2);
            Controls.Add(LblInfo5);
            Controls.Add(PbHassLogo);
            Controls.Add(LblInfo4);
            Controls.Add(LblSyncfusion);
            Controls.Add(LblSerilog);
            Controls.Add(LblNewtonsoftJson);
            Controls.Add(LblMQTTnet);
            Controls.Add(LblMicrosoftAppSDK);
            Controls.Add(LblLibreHardwareMonitor);
            Controls.Add(LblHotkeyListener);
            Controls.Add(LblHADotNet);
            Controls.Add(LblGrapevine);
            Controls.Add(LblCoreAudio);
            Controls.Add(LblInfo3);
            Controls.Add(LblInfo1);
            Controls.Add(LblHassAgentProject);
            Controls.Add(PbHassAgentLogo);
            Controls.Add(BtnClose);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(241, 241, 241);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MetroColor = Color.FromArgb(63, 63, 70);
            Name = "About";
            ShowMaximizeBox = false;
            ShowMinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "About";
            Load += About_Load;
            ResizeEnd += About_ResizeEnd;
            KeyUp += About_KeyUp;
            ((System.ComponentModel.ISupportInitialize)PbHassLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)PbHassAgentLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)PbPayPal).EndInit();
            ((System.ComponentModel.ISupportInitialize)PbKoFi).EndInit();
            ((System.ComponentModel.ISupportInitialize)PbGithubSponsor).EndInit();
            ((System.ComponentModel.ISupportInitialize)PbBMAC).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Syncfusion.WinForms.Controls.SfButton BtnClose;
        private System.Windows.Forms.PictureBox PbHassAgentLogo;
        private System.Windows.Forms.Label LblHassAgentProject;
        private System.Windows.Forms.Label LblInfo1;
        private System.Windows.Forms.Label LblInfo3;
        private System.Windows.Forms.Label LblCoreAudio;
        private System.Windows.Forms.Label LblGrapevine;
        private System.Windows.Forms.Label LblHADotNet;
        private System.Windows.Forms.Label LblMicrosoftAppSDK;
        private System.Windows.Forms.Label LblLibreHardwareMonitor;
        private System.Windows.Forms.Label LblHotkeyListener;
        private System.Windows.Forms.Label LblSerilog;
        private System.Windows.Forms.Label LblNewtonsoftJson;
        private System.Windows.Forms.Label LblMQTTnet;
        private System.Windows.Forms.Label LblSyncfusion;
        private System.Windows.Forms.Label LblInfo4;
        private System.Windows.Forms.PictureBox PbHassLogo;
        private System.Windows.Forms.Label LblInfo5;
        private System.Windows.Forms.Label LblInfo2;
        private System.Windows.Forms.Label LblLab02Research;
        private System.Windows.Forms.Label LblVersion;
        private System.Windows.Forms.Label LblOctokit;
        private System.Windows.Forms.Label LblCliWrap;
        private System.Windows.Forms.Label LblInfo6;
        private System.Windows.Forms.Label LblCassia;
        private Label LblGrpcDotNetNamedPipes;
        private Label LblGrpc;
        private Label LblByteSize;
        private PictureBox PbPayPal;
        private PictureBox PbKoFi;
        private PictureBox PbGithubSponsor;
        private PictureBox PbBMAC;
        private Label label2;
        private Label label1;
        private Label label3;
        private Label label4;
    }
}

