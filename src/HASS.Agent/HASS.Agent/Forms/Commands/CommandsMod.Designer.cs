
using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Forms.Commands
{
	partial class CommandsMod
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandsMod));
			BtnStore = new Syncfusion.WinForms.Controls.SfButton();
			TbSetting = new TextBox();
			TbName = new TextBox();
			LblSetting = new Label();
			LblName = new Label();
			PnlDescription = new Panel();
			TbDescription = new RichTextBox();
			LblDescription = new Label();
			CbRunAsLowIntegrity = new CheckBox();
			LblIntegrityInfo = new Label();
			CbCommandSpecific = new CheckBox();
			LblInfo = new Label();
			LvCommands = new ListView();
			ClmSensorId = new ColumnHeader();
			ClmSensorName = new ColumnHeader();
			ClmAgentCompatible = new ColumnHeader("agent_16_header");
			ClmSatelliteCompatible = new ColumnHeader("service_16_header");
			ClmActionCompatible = new ColumnHeader("action_16_header");
			ClmEmpty = new ColumnHeader();
			ImgLv = new ImageList(components);
			TbSelectedType = new TextBox();
			LblSelectedType = new Label();
			LblService = new Label();
			PbService = new PictureBox();
			LblAgent = new Label();
			PbAgent = new PictureBox();
			LblSpecificClient = new Label();
			CbEntityType = new ComboBox();
			LblEntityType = new Label();
			LblMqttTopic = new Label();
			LblActionInfo = new Label();
			PbActionInfo = new PictureBox();
			BtnConfigureCommand = new Syncfusion.WinForms.Controls.SfButton();
			TbKeyCode = new TextBox();
			LblOptional1 = new Label();
			LblFriendlyName = new Label();
			TbFriendlyName = new TextBox();
			CbConfigDropdown = new ComboBox();
			PnlDescription.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PbService).BeginInit();
			((System.ComponentModel.ISupportInitialize)PbAgent).BeginInit();
			((System.ComponentModel.ISupportInitialize)PbActionInfo).BeginInit();
			SuspendLayout();
			// 
			// BtnStore
			// 
			BtnStore.AccessibleDescription = "Stores the command in the command list. This does not yet activates it.";
			BtnStore.AccessibleName = "Store";
			BtnStore.AccessibleRole = AccessibleRole.PushButton;
			BtnStore.BackColor = Color.FromArgb(63, 63, 70);
			BtnStore.Dock = DockStyle.Bottom;
			BtnStore.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			BtnStore.ForeColor = Color.FromArgb(241, 241, 241);
			BtnStore.Location = new Point(0, 450);
			BtnStore.Name = "BtnStore";
			BtnStore.Size = new Size(1318, 38);
			BtnStore.Style.BackColor = Color.FromArgb(63, 63, 70);
			BtnStore.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
			BtnStore.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
			BtnStore.Style.ForeColor = Color.FromArgb(241, 241, 241);
			BtnStore.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
			BtnStore.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
			BtnStore.Style.PressedForeColor = Color.Black;
			BtnStore.TabIndex = 7;
			BtnStore.Text = Languages.CommandsMod_BtnStore;
			BtnStore.UseVisualStyleBackColor = false;
			BtnStore.Click += BtnStore_Click;
			// 
			// TbSetting
			// 
			TbSetting.AccessibleDescription = "Command specific configuration.";
			TbSetting.AccessibleName = "Command configuration";
			TbSetting.AccessibleRole = AccessibleRole.Text;
			TbSetting.BackColor = Color.FromArgb(63, 63, 70);
			TbSetting.BorderStyle = BorderStyle.FixedSingle;
			TbSetting.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbSetting.ForeColor = Color.FromArgb(241, 241, 241);
			TbSetting.Location = new Point(566, 242);
			TbSetting.Name = "TbSetting";
			TbSetting.Size = new Size(328, 25);
			TbSetting.TabIndex = 3;
			TbSetting.Visible = false;
			// 
			// TbName
			// 
			TbName.AccessibleDescription = "The name as which the command will show up in Home Assistant. This has to be unique!";
			TbName.AccessibleName = "Command name";
			TbName.AccessibleRole = AccessibleRole.Text;
			TbName.BackColor = Color.FromArgb(63, 63, 70);
			TbName.BorderStyle = BorderStyle.FixedSingle;
			TbName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbName.ForeColor = Color.FromArgb(241, 241, 241);
			TbName.Location = new Point(566, 140);
			TbName.Name = "TbName";
			TbName.Size = new Size(328, 25);
			TbName.TabIndex = 2;
			// 
			// LblSetting
			// 
			LblSetting.AccessibleDescription = "Command specific setting description.";
			LblSetting.AccessibleName = "Setting description";
			LblSetting.AccessibleRole = AccessibleRole.StaticText;
			LblSetting.AutoSize = true;
			LblSetting.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblSetting.Location = new Point(566, 220);
			LblSetting.Name = "LblSetting";
			LblSetting.Size = new Size(93, 19);
			LblSetting.TabIndex = 12;
			LblSetting.Text = "&Configuration";
			LblSetting.Visible = false;
			// 
			// LblName
			// 
			LblName.AccessibleDescription = "Command name textbox description";
			LblName.AccessibleName = "Command name description";
			LblName.AccessibleRole = AccessibleRole.StaticText;
			LblName.AutoSize = true;
			LblName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblName.Location = new Point(566, 118);
			LblName.Name = "LblName";
			LblName.Size = new Size(45, 19);
			LblName.TabIndex = 10;
			LblName.Text = "&Name";
			// 
			// PnlDescription
			// 
			PnlDescription.AccessibleDescription = "Contains the description textbox.";
			PnlDescription.AccessibleName = "Description panel";
			PnlDescription.BorderStyle = BorderStyle.FixedSingle;
			PnlDescription.Controls.Add(TbDescription);
			PnlDescription.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			PnlDescription.Location = new Point(955, 39);
			PnlDescription.Name = "PnlDescription";
			PnlDescription.Size = new Size(354, 375);
			PnlDescription.TabIndex = 21;
			// 
			// TbDescription
			// 
			TbDescription.AccessibleDescription = "Contains a description and extra information regarding the selected command.";
			TbDescription.AccessibleName = "Command description";
			TbDescription.AccessibleRole = AccessibleRole.StaticText;
			TbDescription.AutoWordSelection = true;
			TbDescription.BackColor = Color.FromArgb(45, 45, 48);
			TbDescription.BorderStyle = BorderStyle.None;
			TbDescription.Dock = DockStyle.Fill;
			TbDescription.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbDescription.ForeColor = Color.FromArgb(241, 241, 241);
			TbDescription.Location = new Point(0, 0);
			TbDescription.Name = "TbDescription";
			TbDescription.ReadOnly = true;
			TbDescription.Size = new Size(352, 373);
			TbDescription.TabIndex = 18;
			TbDescription.Text = "";
			TbDescription.LinkClicked += TbDescription_LinkClicked;
			// 
			// LblDescription
			// 
			LblDescription.AccessibleDescription = "Command description textbox description.";
			LblDescription.AccessibleName = "Command description description";
			LblDescription.AccessibleRole = AccessibleRole.StaticText;
			LblDescription.AutoSize = true;
			LblDescription.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblDescription.Location = new Point(955, 19);
			LblDescription.Name = "LblDescription";
			LblDescription.Size = new Size(78, 19);
			LblDescription.TabIndex = 20;
			LblDescription.Text = "Description";
			// 
			// CbRunAsLowIntegrity
			// 
			CbRunAsLowIntegrity.AccessibleDescription = "Runs the command as 'low integrity', limiting what it's allowed to do.";
			CbRunAsLowIntegrity.AccessibleName = "Low integrity";
			CbRunAsLowIntegrity.AccessibleRole = AccessibleRole.CheckButton;
			CbRunAsLowIntegrity.AutoSize = true;
			CbRunAsLowIntegrity.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			CbRunAsLowIntegrity.Location = new Point(566, 294);
			CbRunAsLowIntegrity.Name = "CbRunAsLowIntegrity";
			CbRunAsLowIntegrity.Size = new Size(160, 23);
			CbRunAsLowIntegrity.TabIndex = 4;
			CbRunAsLowIntegrity.Text = Languages.CommandsMod_CbRunAsLowIntegrity;
			CbRunAsLowIntegrity.UseVisualStyleBackColor = true;
			CbRunAsLowIntegrity.Visible = false;
			// 
			// LblIntegrityInfo
			// 
			LblIntegrityInfo.AccessibleDescription = "Opens a message box, showing extra info about low integrity commands.";
			LblIntegrityInfo.AccessibleName = "Low integrity info";
			LblIntegrityInfo.AccessibleRole = AccessibleRole.PushButton;
			LblIntegrityInfo.Cursor = Cursors.Hand;
			LblIntegrityInfo.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
			LblIntegrityInfo.Location = new Point(739, 298);
			LblIntegrityInfo.Name = "LblIntegrityInfo";
			LblIntegrityInfo.Size = new Size(155, 15);
			LblIntegrityInfo.TabIndex = 27;
			LblIntegrityInfo.Text = "What's this?";
			LblIntegrityInfo.TextAlign = ContentAlignment.MiddleRight;
			LblIntegrityInfo.Visible = false;
			LblIntegrityInfo.Click += LblIntegrityInfo_Click;
			// 
			// CbCommandSpecific
			// 
			CbCommandSpecific.AccessibleDescription = "Command specific setting.";
			CbCommandSpecific.AccessibleName = "Command setting";
			CbCommandSpecific.AccessibleRole = AccessibleRole.CheckButton;
			CbCommandSpecific.AutoSize = true;
			CbCommandSpecific.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			CbCommandSpecific.Location = new Point(566, 335);
			CbCommandSpecific.Name = "CbCommandSpecific";
			CbCommandSpecific.Size = new Size(34, 23);
			CbCommandSpecific.TabIndex = 5;
			CbCommandSpecific.Text = "-";
			CbCommandSpecific.UseVisualStyleBackColor = true;
			CbCommandSpecific.Visible = false;
			// 
			// LblInfo
			// 
			LblInfo.AccessibleDescription = "Extra info regarding the selected command.";
			LblInfo.AccessibleName = "Command extra info";
			LblInfo.AccessibleRole = AccessibleRole.StaticText;
			LblInfo.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblInfo.Location = new Point(566, 270);
			LblInfo.Name = "LblInfo";
			LblInfo.Size = new Size(328, 168);
			LblInfo.TabIndex = 29;
			LblInfo.Text = "-";
			LblInfo.Visible = false;
			// 
			// LvCommands
			// 
			LvCommands.AccessibleDescription = "List of available command types.";
			LvCommands.AccessibleName = "Command types";
			LvCommands.AccessibleRole = AccessibleRole.Table;
			LvCommands.BackColor = Color.FromArgb(63, 63, 70);
			LvCommands.Columns.AddRange(new ColumnHeader[] { ClmSensorId, ClmSensorName, ClmAgentCompatible, ClmSatelliteCompatible, ClmActionCompatible, ClmEmpty });
			LvCommands.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LvCommands.ForeColor = Color.FromArgb(241, 241, 241);
			LvCommands.FullRowSelect = true;
			LvCommands.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			LvCommands.HideSelection = true;
			LvCommands.LargeImageList = ImgLv;
			LvCommands.Location = new Point(12, 15);
			LvCommands.MultiSelect = false;
			LvCommands.Name = "LvCommands";
			LvCommands.OwnerDraw = true;
			LvCommands.Size = new Size(516, 399);
			LvCommands.SmallImageList = ImgLv;
			LvCommands.TabIndex = 30;
			LvCommands.UseCompatibleStateImageBehavior = false;
			LvCommands.View = View.Details;
			LvCommands.SelectedIndexChanged += LvCommands_SelectedIndexChanged;
			// 
			// ClmSensorId
			// 
			ClmSensorId.Text = "id";
			ClmSensorId.Width = 0;
			// 
			// ClmSensorName
			// 
			ClmSensorName.Text = Languages.CommandsMod_ClmSensorName;
			ClmSensorName.Width = 300;
			// 
			// ClmAgentCompatible
			// 
			ClmAgentCompatible.Tag = "hide";
			ClmAgentCompatible.Text = "agent compatible";
			// 
			// ClmSatelliteCompatible
			// 
			ClmSatelliteCompatible.Tag = "hide";
			ClmSatelliteCompatible.Text = "satellite compatible";
			// 
			// ClmActionCompatible
			// 
			ClmActionCompatible.Tag = "hide";
			ClmActionCompatible.Text = "action compatible";
			// 
			// ClmEmpty
			// 
			ClmEmpty.Tag = "hide";
			ClmEmpty.Text = "filler column";
			ClmEmpty.Width = 500;
			// 
			// ImgLv
			// 
			ImgLv.ColorDepth = ColorDepth.Depth24Bit;
			ImgLv.ImageStream = (ImageListStreamer)resources.GetObject("ImgLv.ImageStream");
			ImgLv.TransparentColor = Color.Transparent;
			ImgLv.Images.SetKeyName(0, "multivalue_16_header");
			ImgLv.Images.SetKeyName(1, "agent_16_header");
			ImgLv.Images.SetKeyName(2, "service_16_header");
			ImgLv.Images.SetKeyName(3, "action_16_header");
			// 
			// TbSelectedType
			// 
			TbSelectedType.AccessibleDescription = "Selected command type.";
			TbSelectedType.AccessibleName = "Selected command";
			TbSelectedType.AccessibleRole = AccessibleRole.StaticText;
			TbSelectedType.BackColor = Color.FromArgb(45, 45, 48);
			TbSelectedType.BorderStyle = BorderStyle.FixedSingle;
			TbSelectedType.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbSelectedType.ForeColor = Color.FromArgb(241, 241, 241);
			TbSelectedType.Location = new Point(566, 40);
			TbSelectedType.Name = "TbSelectedType";
			TbSelectedType.ReadOnly = true;
			TbSelectedType.Size = new Size(328, 25);
			TbSelectedType.TabIndex = 0;
			// 
			// LblSelectedType
			// 
			LblSelectedType.AccessibleDescription = "Selected command type textbox description.";
			LblSelectedType.AccessibleName = "Selected command description";
			LblSelectedType.AccessibleRole = AccessibleRole.StaticText;
			LblSelectedType.AutoSize = true;
			LblSelectedType.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblSelectedType.Location = new Point(566, 18);
			LblSelectedType.Name = "LblSelectedType";
			LblSelectedType.Size = new Size(91, 19);
			LblSelectedType.TabIndex = 31;
			LblSelectedType.Text = "Selected Type";
			// 
			// LblService
			// 
			LblService.AccessibleDescription = "Service column description.";
			LblService.AccessibleName = "Service info";
			LblService.AccessibleRole = AccessibleRole.StaticText;
			LblService.AutoSize = true;
			LblService.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			LblService.Location = new Point(116, 421);
			LblService.Name = "LblService";
			LblService.Size = new Size(44, 15);
			LblService.TabIndex = 37;
			LblService.Text = "Service";
			// 
			// PbService
			// 
			PbService.AccessibleDescription = "Service icon image, as shown in the header of the 'service' column.";
			PbService.AccessibleName = "Service icon";
			PbService.AccessibleRole = AccessibleRole.Graphic;
			PbService.Image = Properties.Resources.service_16;
			PbService.Location = new Point(94, 420);
			PbService.Name = "PbService";
			PbService.Size = new Size(16, 16);
			PbService.SizeMode = PictureBoxSizeMode.AutoSize;
			PbService.TabIndex = 36;
			PbService.TabStop = false;
			// 
			// LblAgent
			// 
			LblAgent.AccessibleDescription = "Agent column description.";
			LblAgent.AccessibleName = "Agent info";
			LblAgent.AccessibleRole = AccessibleRole.StaticText;
			LblAgent.AutoSize = true;
			LblAgent.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			LblAgent.Location = new Point(34, 421);
			LblAgent.Name = "LblAgent";
			LblAgent.Size = new Size(39, 15);
			LblAgent.TabIndex = 35;
			LblAgent.Text = "Agent";
			// 
			// PbAgent
			// 
			PbAgent.AccessibleDescription = "Agent icon image, as shown in the header of the 'agent' column.";
			PbAgent.AccessibleName = "Agent icon";
			PbAgent.AccessibleRole = AccessibleRole.Graphic;
			PbAgent.Image = Properties.Resources.agent_16;
			PbAgent.Location = new Point(12, 420);
			PbAgent.Name = "PbAgent";
			PbAgent.Size = new Size(16, 16);
			PbAgent.SizeMode = PictureBoxSizeMode.AutoSize;
			PbAgent.TabIndex = 34;
			PbAgent.TabStop = false;
			// 
			// LblSpecificClient
			// 
			LblSpecificClient.AccessibleDescription = "Warning message that the selected command is only available for the HASS.Agent, not the satellite service.";
			LblSpecificClient.AccessibleName = "Compatibility warning";
			LblSpecificClient.AccessibleRole = AccessibleRole.StaticText;
			LblSpecificClient.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
			LblSpecificClient.ForeColor = Color.FromArgb(245, 42, 42);
			LblSpecificClient.Location = new Point(739, 18);
			LblSpecificClient.Name = "LblSpecificClient";
			LblSpecificClient.Size = new Size(155, 19);
			LblSpecificClient.TabIndex = 38;
			LblSpecificClient.Text = "HASS.Agent only!";
			LblSpecificClient.TextAlign = ContentAlignment.TopRight;
			LblSpecificClient.Visible = false;
			// 
			// CbEntityType
			// 
			CbEntityType.AccessibleDescription = "List of possible entity types, as which the command will show up in Home Assistant.";
			CbEntityType.AccessibleName = "Entity types";
			CbEntityType.AccessibleRole = AccessibleRole.DropList;
			CbEntityType.BackColor = Color.FromArgb(63, 63, 70);
			CbEntityType.DrawMode = DrawMode.OwnerDrawFixed;
			CbEntityType.DropDownHeight = 300;
			CbEntityType.DropDownStyle = ComboBoxStyle.DropDownList;
			CbEntityType.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
			CbEntityType.ForeColor = Color.FromArgb(241, 241, 241);
			CbEntityType.FormattingEnabled = true;
			CbEntityType.IntegralHeight = false;
			CbEntityType.Location = new Point(566, 90);
			CbEntityType.Name = "CbEntityType";
			CbEntityType.Size = new Size(328, 26);
			CbEntityType.TabIndex = 1;
			CbEntityType.SelectedIndexChanged += CbEntityType_SelectedIndexChanged;
			// 
			// LblEntityType
			// 
			LblEntityType.AccessibleDescription = "Entity type dropdown description.";
			LblEntityType.AccessibleName = "Entity type description";
			LblEntityType.AccessibleRole = AccessibleRole.StaticText;
			LblEntityType.AutoSize = true;
			LblEntityType.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblEntityType.Location = new Point(566, 68);
			LblEntityType.Name = "LblEntityType";
			LblEntityType.Size = new Size(76, 19);
			LblEntityType.TabIndex = 40;
			LblEntityType.Text = "&Entity Type";
			// 
			// LblMqttTopic
			// 
			LblMqttTopic.AccessibleDescription = "Opens a window with the MQTT topic you can use, to send command actions to.";
			LblMqttTopic.AccessibleName = "Mqtt action topic";
			LblMqttTopic.AccessibleRole = AccessibleRole.PushButton;
			LblMqttTopic.AutoSize = true;
			LblMqttTopic.Cursor = Cursors.Hand;
			LblMqttTopic.Font = new Font("Segoe UI", 10F, FontStyle.Underline, GraphicsUnit.Point);
			LblMqttTopic.Location = new Point(955, 421);
			LblMqttTopic.Name = "LblMqttTopic";
			LblMqttTopic.Size = new Size(161, 19);
			LblMqttTopic.TabIndex = 41;
			LblMqttTopic.Text = "Show MQTT Action Topic";
			LblMqttTopic.Visible = false;
			LblMqttTopic.Click += LblMqttTopic_Click;
			// 
			// LblActionInfo
			// 
			LblActionInfo.AccessibleDescription = "Action column description. Click to open the examples webpage.";
			LblActionInfo.AccessibleName = "Action info";
			LblActionInfo.AccessibleRole = AccessibleRole.Link;
			LblActionInfo.AutoSize = true;
			LblActionInfo.Cursor = Cursors.Hand;
			LblActionInfo.Font = new Font("Segoe UI", 9F, FontStyle.Underline, GraphicsUnit.Point);
			LblActionInfo.Location = new Point(197, 421);
			LblActionInfo.Name = "LblActionInfo";
			LblActionInfo.Size = new Size(42, 15);
			LblActionInfo.TabIndex = 43;
			LblActionInfo.Text = "Action";
			LblActionInfo.Click += LblActionInfo_Click;
			// 
			// PbActionInfo
			// 
			PbActionInfo.AccessibleDescription = "Action icon image, as shown in the header of the 'action' column.";
			PbActionInfo.AccessibleName = "Action icon";
			PbActionInfo.AccessibleRole = AccessibleRole.Graphic;
			PbActionInfo.Cursor = Cursors.Hand;
			PbActionInfo.Image = Properties.Resources.action_16;
			PbActionInfo.Location = new Point(175, 420);
			PbActionInfo.Name = "PbActionInfo";
			PbActionInfo.Size = new Size(16, 16);
			PbActionInfo.SizeMode = PictureBoxSizeMode.AutoSize;
			PbActionInfo.TabIndex = 42;
			PbActionInfo.TabStop = false;
			PbActionInfo.Click += PbActionInfo_Click;
			// 
			// BtnConfigureCommand
			// 
			BtnConfigureCommand.AccessibleDescription = "Opens a command specific window with extra settings.";
			BtnConfigureCommand.AccessibleName = "Command settings window";
			BtnConfigureCommand.AccessibleRole = AccessibleRole.PushButton;
			BtnConfigureCommand.BackColor = Color.FromArgb(63, 63, 70);
			BtnConfigureCommand.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			BtnConfigureCommand.ForeColor = Color.FromArgb(241, 241, 241);
			BtnConfigureCommand.Location = new Point(566, 380);
			BtnConfigureCommand.Name = "BtnConfigureCommand";
			BtnConfigureCommand.Size = new Size(328, 34);
			BtnConfigureCommand.Style.BackColor = Color.FromArgb(63, 63, 70);
			BtnConfigureCommand.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
			BtnConfigureCommand.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
			BtnConfigureCommand.Style.ForeColor = Color.FromArgb(241, 241, 241);
			BtnConfigureCommand.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
			BtnConfigureCommand.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
			BtnConfigureCommand.Style.PressedForeColor = Color.Black;
			BtnConfigureCommand.TabIndex = 6;
			BtnConfigureCommand.Text = Languages.CommandsMod_BtnConfigureCommand;
			BtnConfigureCommand.UseVisualStyleBackColor = false;
			BtnConfigureCommand.Visible = false;
			BtnConfigureCommand.Click += BtnConfigureCommand_Click;
			// 
			// TbKeyCode
			// 
			TbKeyCode.AccessibleDescription = "Captures what key is pressed, and converts it to its integer value.";
			TbKeyCode.AccessibleName = "Keycode";
			TbKeyCode.AccessibleRole = AccessibleRole.Text;
			TbKeyCode.BackColor = Color.FromArgb(63, 63, 70);
			TbKeyCode.BorderStyle = BorderStyle.FixedSingle;
			TbKeyCode.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbKeyCode.ForeColor = Color.FromArgb(241, 241, 241);
			TbKeyCode.Location = new Point(566, 242);
			TbKeyCode.Name = "TbKeyCode";
			TbKeyCode.ReadOnly = true;
			TbKeyCode.Size = new Size(328, 25);
			TbKeyCode.TabIndex = 44;
			TbKeyCode.Visible = false;
			TbKeyCode.KeyDown += TbKeyCode_KeyDown;
			// 
			// LblOptional1
			// 
			LblOptional1.AccessibleDescription = "Indicates that the friendly name is optional.";
			LblOptional1.AccessibleName = "Friendly name optional";
			LblOptional1.AccessibleRole = AccessibleRole.StaticText;
			LblOptional1.AutoSize = true;
			LblOptional1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			LblOptional1.Location = new Point(842, 171);
			LblOptional1.Name = "LblOptional1";
			LblOptional1.Size = new Size(51, 15);
			LblOptional1.TabIndex = 49;
			LblOptional1.Text = "optional";
			// 
			// LblFriendlyName
			// 
			LblFriendlyName.AccessibleDescription = "Command friendly name textbox description";
			LblFriendlyName.AccessibleName = "Command friendly name description";
			LblFriendlyName.AccessibleRole = AccessibleRole.StaticText;
			LblFriendlyName.AutoSize = true;
			LblFriendlyName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblFriendlyName.Location = new Point(566, 169);
			LblFriendlyName.Name = "LblFriendlyName";
			LblFriendlyName.Size = new Size(95, 19);
			LblFriendlyName.TabIndex = 48;
			LblFriendlyName.Text = "&Friendly name";
			// 
			// TbFriendlyName
			// 
			TbFriendlyName.AccessibleDescription = "The friendly name as which the command will show up in Home Assistant.";
			TbFriendlyName.AccessibleName = "Command friendly name";
			TbFriendlyName.AccessibleRole = AccessibleRole.Text;
			TbFriendlyName.BackColor = Color.FromArgb(63, 63, 70);
			TbFriendlyName.BorderStyle = BorderStyle.FixedSingle;
			TbFriendlyName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbFriendlyName.ForeColor = Color.FromArgb(241, 241, 241);
			TbFriendlyName.Location = new Point(566, 190);
			TbFriendlyName.Name = "TbFriendlyName";
			TbFriendlyName.Size = new Size(328, 25);
			TbFriendlyName.TabIndex = 47;
			// 
			// CbConfigDropdown
			// 
			CbConfigDropdown.AccessibleDescription = "List of configuration options";
			CbConfigDropdown.AccessibleName = "Configuration combo box";
			CbConfigDropdown.AccessibleRole = AccessibleRole.DropList;
			CbConfigDropdown.BackColor = Color.FromArgb(63, 63, 70);
			CbConfigDropdown.DrawMode = DrawMode.OwnerDrawFixed;
			CbConfigDropdown.DropDownHeight = 300;
			CbConfigDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
			CbConfigDropdown.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
			CbConfigDropdown.ForeColor = Color.FromArgb(241, 241, 241);
			CbConfigDropdown.FormattingEnabled = true;
			CbConfigDropdown.IntegralHeight = false;
			CbConfigDropdown.Location = new Point(566, 242);
			CbConfigDropdown.Name = "CbConfigDropdown";
			CbConfigDropdown.Size = new Size(328, 26);
			CbConfigDropdown.TabIndex = 50;
			CbConfigDropdown.Visible = false;
			// 
			// CommandsMod
			// 
			AccessibleDescription = "Create or modify a command.";
			AccessibleName = "Command mod";
			AccessibleRole = AccessibleRole.Window;
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			BackColor = Color.FromArgb(45, 45, 48);
			CaptionBarColor = Color.FromArgb(63, 63, 70);
			CaptionFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			CaptionForeColor = Color.FromArgb(241, 241, 241);
			ClientSize = new Size(1318, 488);
			Controls.Add(CbConfigDropdown);
			Controls.Add(LblOptional1);
			Controls.Add(LblFriendlyName);
			Controls.Add(TbFriendlyName);
			Controls.Add(TbKeyCode);
			Controls.Add(BtnConfigureCommand);
			Controls.Add(LblActionInfo);
			Controls.Add(PbActionInfo);
			Controls.Add(LblMqttTopic);
			Controls.Add(LblEntityType);
			Controls.Add(CbEntityType);
			Controls.Add(LblSpecificClient);
			Controls.Add(LblService);
			Controls.Add(PbService);
			Controls.Add(LblAgent);
			Controls.Add(PbAgent);
			Controls.Add(TbSelectedType);
			Controls.Add(LblSelectedType);
			Controls.Add(LvCommands);
			Controls.Add(CbCommandSpecific);
			Controls.Add(LblIntegrityInfo);
			Controls.Add(CbRunAsLowIntegrity);
			Controls.Add(PnlDescription);
			Controls.Add(LblDescription);
			Controls.Add(TbSetting);
			Controls.Add(BtnStore);
			Controls.Add(TbName);
			Controls.Add(LblSetting);
			Controls.Add(LblName);
			Controls.Add(LblInfo);
			DoubleBuffered = true;
			ForeColor = Color.FromArgb(241, 241, 241);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			MetroColor = Color.FromArgb(63, 63, 70);
			Name = "CommandsMod";
			ShowMaximizeBox = false;
			ShowMinimizeBox = false;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Command";
			Load += CommandsMod_Load;
			ResizeEnd += CommandsMod_ResizeEnd;
			KeyUp += CommandsMod_KeyUp;
			Layout += CommandsMod_Layout;
			PnlDescription.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)PbService).EndInit();
			((System.ComponentModel.ISupportInitialize)PbAgent).EndInit();
			((System.ComponentModel.ISupportInitialize)PbActionInfo).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Syncfusion.WinForms.Controls.SfButton BtnStore;
		private System.Windows.Forms.Label LblSetting;
		private System.Windows.Forms.Label LblName;
		private System.Windows.Forms.TextBox TbName;
		private System.Windows.Forms.TextBox TbSetting;
		private System.Windows.Forms.Panel PnlDescription;
		private System.Windows.Forms.RichTextBox TbDescription;
		private System.Windows.Forms.Label LblDescription;
		private System.Windows.Forms.CheckBox CbRunAsLowIntegrity;
		private System.Windows.Forms.Label LblIntegrityInfo;
		private System.Windows.Forms.CheckBox CbCommandSpecific;
		private System.Windows.Forms.Label LblInfo;
		private ListView LvCommands;
		private ColumnHeader ClmSensorName;
		private ColumnHeader ClmAgentCompatible;
		private ColumnHeader ClmSatelliteCompatible;
		private ColumnHeader ClmEmpty;
		private TextBox TbSelectedType;
		private Label LblSelectedType;
		private Label LblService;
		private PictureBox PbService;
		private Label LblAgent;
		private PictureBox PbAgent;
		private ImageList ImgLv;
		private Label LblSpecificClient;
		private ComboBox CbEntityType;
		private Label LblEntityType;
		private Label LblMqttTopic;
		private ColumnHeader ClmActionCompatible;
		private Label LblActionInfo;
		private PictureBox PbActionInfo;
		private ColumnHeader ClmSensorId;
		private Syncfusion.WinForms.Controls.SfButton BtnConfigureCommand;
		private TextBox TbKeyCode;
		private Label LblOptional1;
		private Label LblFriendlyName;
		private TextBox TbFriendlyName;
		private ComboBox CbConfigDropdown;
	}
}

