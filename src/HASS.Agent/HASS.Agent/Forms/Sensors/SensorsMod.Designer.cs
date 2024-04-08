
using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Forms.Sensors
{
	partial class SensorsMod
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SensorsMod));
			BtnStore = new Syncfusion.WinForms.Controls.SfButton();
			LblSetting1 = new Label();
			TbSetting1 = new TextBox();
			LblType = new Label();
			LblName = new Label();
			TbName = new TextBox();
			LblUpdate = new Label();
			LblSeconds = new Label();
			LblDescription = new Label();
			TbDescription = new RichTextBox();
			PnlDescription = new Panel();
			LblSetting2 = new Label();
			TbSetting2 = new TextBox();
			LblSetting3 = new Label();
			TbSetting3 = new TextBox();
			NumInterval = new Syncfusion.Windows.Forms.Tools.NumericUpDownExt();
			LvSensors = new ListView();
			ClmId = new ColumnHeader();
			ClmSensorName = new ColumnHeader();
			ClmMultiValue = new ColumnHeader("multivalue_16_header");
			ClmAgentCompatible = new ColumnHeader("agent_16_header");
			ClmSatelliteCompatible = new ColumnHeader("service_16_header");
			ClmEmpty = new ColumnHeader();
			ImgLv = new ImageList(components);
			TbSelectedType = new TextBox();
			PbMultiValue = new PictureBox();
			LblMultiValue = new Label();
			LblAgent = new Label();
			PbAgent = new PictureBox();
			LblService = new Label();
			PbService = new PictureBox();
			LblSpecificClient = new Label();
			BtnTest = new Syncfusion.WinForms.Controls.SfButton();
            BtnAdvSettings = new Syncfusion.WinForms.Controls.SfButton();
            CbNetworkCard = new ComboBox();
			NumRound = new Syncfusion.Windows.Forms.Tools.NumericUpDownExt();
			LblDigits = new Label();
			CbApplyRounding = new CheckBox();
			LblFriendlyName = new Label();
			TbFriendlyName = new TextBox();
			LblOptional1 = new Label();
			CbIgnoreAvailability = new CheckBox();
			PnlDescription.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)NumInterval).BeginInit();
			((System.ComponentModel.ISupportInitialize)PbMultiValue).BeginInit();
			((System.ComponentModel.ISupportInitialize)PbAgent).BeginInit();
			((System.ComponentModel.ISupportInitialize)PbService).BeginInit();
			((System.ComponentModel.ISupportInitialize)NumRound).BeginInit();
			SuspendLayout();
			// 
			// BtnStore
			// 
			BtnStore.AccessibleDescription = "Stores the sensor in the sensor list. This does not yet activates it.";
			BtnStore.AccessibleName = "Store";
			BtnStore.AccessibleRole = AccessibleRole.PushButton;
			BtnStore.BackColor = Color.FromArgb(63, 63, 70);
			BtnStore.Dock = DockStyle.Bottom;
			BtnStore.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			BtnStore.ForeColor = Color.FromArgb(241, 241, 241);
			BtnStore.Location = new Point(0, 484);
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
			BtnStore.Text = Languages.SensorsMod_BtnStore;
			BtnStore.UseVisualStyleBackColor = false;
			BtnStore.Click += BtnStore_Click;
			// 
			// LblSetting1
			// 
			LblSetting1.AccessibleDescription = "Sensor specific setting 1 description.";
			LblSetting1.AccessibleName = "Setting 1 description";
			LblSetting1.AccessibleRole = AccessibleRole.StaticText;
			LblSetting1.AutoSize = true;
			LblSetting1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblSetting1.Location = new Point(566, 315);
			LblSetting1.Name = "LblSetting1";
			LblSetting1.Size = new Size(63, 19);
			LblSetting1.TabIndex = 12;
			LblSetting1.Text = "setting 1";
			LblSetting1.Visible = false;
			// 
			// TbSetting1
			// 
			TbSetting1.AccessibleDescription = "Sensor specific configuration.";
			TbSetting1.AccessibleName = "Setting 1";
			TbSetting1.AccessibleRole = AccessibleRole.Text;
			TbSetting1.BackColor = Color.FromArgb(63, 63, 70);
			TbSetting1.BorderStyle = BorderStyle.FixedSingle;
			TbSetting1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbSetting1.ForeColor = Color.FromArgb(241, 241, 241);
			TbSetting1.Location = new Point(566, 337);
			TbSetting1.Name = "TbSetting1";
			TbSetting1.Size = new Size(328, 25);
			TbSetting1.TabIndex = 2;
			TbSetting1.Visible = false;
			// 
			// LblType
			// 
			LblType.AccessibleDescription = "Selected sensor type textbox description.";
			LblType.AccessibleName = "Selected sensor description";
			LblType.AccessibleRole = AccessibleRole.StaticText;
			LblType.AutoSize = true;
			LblType.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblType.Location = new Point(566, 18);
			LblType.Name = "LblType";
			LblType.Size = new Size(91, 19);
			LblType.TabIndex = 3;
			LblType.Text = "Selected Type";
			// 
			// LblName
			// 
			LblName.AccessibleDescription = "Sensor name textbox description";
			LblName.AccessibleName = "Sensor name description";
			LblName.AccessibleRole = AccessibleRole.StaticText;
			LblName.AutoSize = true;
			LblName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblName.Location = new Point(566, 72);
			LblName.Name = "LblName";
			LblName.Size = new Size(45, 19);
			LblName.TabIndex = 10;
			LblName.Text = "&Name";
			// 
			// TbName
			// 
			TbName.AccessibleDescription = "The name as which the sensor will show up in Home Assistant. This has to be unique!";
			TbName.AccessibleName = "Sensor name";
			TbName.AccessibleRole = AccessibleRole.Text;
			TbName.BackColor = Color.FromArgb(63, 63, 70);
			TbName.BorderStyle = BorderStyle.FixedSingle;
			TbName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbName.ForeColor = Color.FromArgb(241, 241, 241);
			TbName.Location = new Point(566, 94);
			TbName.Name = "TbName";
			TbName.Size = new Size(328, 25);
			TbName.TabIndex = 1;
			// 
			// LblUpdate
			// 
			LblUpdate.AccessibleDescription = "Update interval numeric textbox description.";
			LblUpdate.AccessibleName = "Update interval description";
			LblUpdate.AccessibleRole = AccessibleRole.StaticText;
			LblUpdate.AutoSize = true;
			LblUpdate.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblUpdate.Location = new Point(566, 194);
			LblUpdate.Name = "LblUpdate";
			LblUpdate.Size = new Size(91, 19);
			LblUpdate.TabIndex = 13;
			LblUpdate.Text = "&Update every";
			// 
			// LblSeconds
			// 
			LblSeconds.AccessibleDescription = "Update interval time unit.";
			LblSeconds.AccessibleName = "Update interval time unit";
			LblSeconds.AccessibleRole = AccessibleRole.StaticText;
			LblSeconds.AutoSize = true;
			LblSeconds.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblSeconds.Location = new Point(763, 194);
			LblSeconds.Name = "LblSeconds";
			LblSeconds.Size = new Size(58, 19);
			LblSeconds.TabIndex = 15;
			LblSeconds.Text = "seconds";
			// 
			// LblDescription
			// 
			LblDescription.AccessibleDescription = "Sensor description textbox description.";
			LblDescription.AccessibleName = "Sensor description description";
			LblDescription.AccessibleRole = AccessibleRole.StaticText;
			LblDescription.AutoSize = true;
			LblDescription.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblDescription.Location = new Point(955, 17);
			LblDescription.Name = "LblDescription";
			LblDescription.Size = new Size(78, 19);
			LblDescription.TabIndex = 17;
			LblDescription.Text = "Description";
			// 
			// TbDescription
			// 
			TbDescription.AccessibleDescription = "Contains a description and extra information regarding the selected sensor.";
			TbDescription.AccessibleName = "Sensor description";
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
			TbDescription.Size = new Size(352, 347);
			TbDescription.TabIndex = 18;
			TbDescription.Text = "";
			TbDescription.LinkClicked += TbDescription_LinkClicked;
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
			PnlDescription.Size = new Size(354, 349);
			PnlDescription.TabIndex = 19;
			// 
			// LblSetting2
			// 
			LblSetting2.AccessibleDescription = "Sensor specific setting 2 description.";
			LblSetting2.AccessibleName = "Setting 2 description";
			LblSetting2.AccessibleRole = AccessibleRole.StaticText;
			LblSetting2.AutoSize = true;
			LblSetting2.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblSetting2.Location = new Point(566, 368);
			LblSetting2.Name = "LblSetting2";
			LblSetting2.Size = new Size(63, 19);
			LblSetting2.TabIndex = 21;
			LblSetting2.Text = "setting 2";
			LblSetting2.Visible = false;
			// 
			// TbSetting2
			// 
			TbSetting2.AccessibleDescription = "Sensor specific configuration.";
			TbSetting2.AccessibleName = "Setting 2";
			TbSetting2.AccessibleRole = AccessibleRole.Text;
			TbSetting2.BackColor = Color.FromArgb(63, 63, 70);
			TbSetting2.BorderStyle = BorderStyle.FixedSingle;
			TbSetting2.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbSetting2.ForeColor = Color.FromArgb(241, 241, 241);
			TbSetting2.Location = new Point(566, 390);
			TbSetting2.Name = "TbSetting2";
			TbSetting2.Size = new Size(328, 25);
			TbSetting2.TabIndex = 5;
			TbSetting2.Visible = false;
			// 
			// LblSetting3
			// 
			LblSetting3.AccessibleDescription = "Sensor specific setting 3 description.";
			LblSetting3.AccessibleName = "Setting 3 description";
			LblSetting3.AccessibleRole = AccessibleRole.StaticText;
			LblSetting3.AutoSize = true;
			LblSetting3.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblSetting3.Location = new Point(566, 421);
			LblSetting3.Name = "LblSetting3";
			LblSetting3.Size = new Size(63, 19);
			LblSetting3.TabIndex = 23;
			LblSetting3.Text = "setting 3";
			LblSetting3.Visible = false;
			// 
			// TbSetting3
			// 
			TbSetting3.AccessibleDescription = "Sensor specific configuration.";
			TbSetting3.AccessibleName = "Setting 3";
			TbSetting3.AccessibleRole = AccessibleRole.Text;
			TbSetting3.BackColor = Color.FromArgb(63, 63, 70);
			TbSetting3.BorderStyle = BorderStyle.FixedSingle;
			TbSetting3.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbSetting3.ForeColor = Color.FromArgb(241, 241, 241);
			TbSetting3.Location = new Point(566, 443);
			TbSetting3.Name = "TbSetting3";
			TbSetting3.Size = new Size(328, 25);
			TbSetting3.TabIndex = 6;
			TbSetting3.Visible = false;
			// 
			// NumInterval
			// 
			NumInterval.AccessibleDescription = "The amount of seconds between the updates of this sensor's value. Only accepts numeric values.";
			NumInterval.AccessibleName = "Update interval";
			NumInterval.AccessibleRole = AccessibleRole.Text;
			NumInterval.BackColor = Color.FromArgb(63, 63, 70);
			NumInterval.BeforeTouchSize = new Size(83, 25);
			NumInterval.Border3DStyle = Border3DStyle.Flat;
			NumInterval.BorderColor = SystemColors.WindowFrame;
			NumInterval.BorderStyle = BorderStyle.FixedSingle;
			NumInterval.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			NumInterval.ForeColor = Color.FromArgb(241, 241, 241);
			NumInterval.Location = new Point(673, 192);
			NumInterval.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
			NumInterval.MaxLength = 10;
			NumInterval.MetroColor = SystemColors.WindowFrame;
			NumInterval.Name = "NumInterval";
			NumInterval.Size = new Size(83, 25);
			NumInterval.TabIndex = 2;
			NumInterval.ThemeName = "Metro";
			NumInterval.Value = new decimal(new int[] { 10, 0, 0, 0 });
			NumInterval.VisualStyle = Syncfusion.Windows.Forms.VisualStyle.Metro;
			// 
			// LvSensors
			// 
			LvSensors.AccessibleDescription = "List of available sensor types.";
			LvSensors.AccessibleName = "Sensor types";
			LvSensors.AccessibleRole = AccessibleRole.Table;
			LvSensors.BackColor = Color.FromArgb(63, 63, 70);
			LvSensors.Columns.AddRange(new ColumnHeader[] { ClmId, ClmSensorName, ClmMultiValue, ClmAgentCompatible, ClmSatelliteCompatible, ClmEmpty });
			LvSensors.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LvSensors.ForeColor = Color.FromArgb(241, 241, 241);
			LvSensors.FullRowSelect = true;
			LvSensors.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			LvSensors.HideSelection = true;
			LvSensors.LargeImageList = ImgLv;
			LvSensors.Location = new Point(12, 15);
			LvSensors.MultiSelect = false;
			LvSensors.Name = "LvSensors";
			LvSensors.OwnerDraw = true;
			LvSensors.Size = new Size(516, 373);
			LvSensors.SmallImageList = ImgLv;
			LvSensors.TabIndex = 26;
			LvSensors.UseCompatibleStateImageBehavior = false;
			LvSensors.View = View.Details;
			LvSensors.SelectedIndexChanged += LvSensors_SelectedIndexChanged;
			// 
			// ClmId
			// 
			ClmId.Text = "id";
			ClmId.Width = 0;
			// 
			// ClmSensorName
			// 
			ClmSensorName.Text = Languages.SensorsMod_ClmSensorName;
			ClmSensorName.Width = 300;
			// 
			// ClmMultiValue
			// 
			ClmMultiValue.Tag = "hide";
			ClmMultiValue.Text = Languages.SensorsMod_LblMultiValue;
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
			// 
			// TbSelectedType
			// 
			TbSelectedType.AccessibleDescription = "Selected sensor type.";
			TbSelectedType.AccessibleName = "Selected sensor";
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
			// PbMultiValue
			// 
			PbMultiValue.AccessibleDescription = "Multivalue icon image, as shown in the header of the 'multivalue' column.";
			PbMultiValue.AccessibleName = "Multivalue icon";
			PbMultiValue.AccessibleRole = AccessibleRole.Graphic;
			PbMultiValue.Image = Properties.Resources.multivalue_16;
			PbMultiValue.Location = new Point(182, 397);
			PbMultiValue.Name = "PbMultiValue";
			PbMultiValue.Size = new Size(16, 16);
			PbMultiValue.SizeMode = PictureBoxSizeMode.AutoSize;
			PbMultiValue.TabIndex = 28;
			PbMultiValue.TabStop = false;
			// 
			// LblMultiValue
			// 
			LblMultiValue.AccessibleDescription = "Multivalue column description.";
			LblMultiValue.AccessibleName = "Multivalue info";
			LblMultiValue.AccessibleRole = AccessibleRole.StaticText;
			LblMultiValue.AutoSize = true;
			LblMultiValue.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			LblMultiValue.Location = new Point(204, 398);
			LblMultiValue.Name = "LblMultiValue";
			LblMultiValue.Size = new Size(63, 15);
			LblMultiValue.TabIndex = 29;
			LblMultiValue.Text = "Multivalue";
			// 
			// LblAgent
			// 
			LblAgent.AccessibleDescription = "Agent column description.";
			LblAgent.AccessibleName = "Agent info";
			LblAgent.AccessibleRole = AccessibleRole.StaticText;
			LblAgent.AutoSize = true;
			LblAgent.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			LblAgent.Location = new Point(34, 398);
			LblAgent.Name = "LblAgent";
			LblAgent.Size = new Size(39, 15);
			LblAgent.TabIndex = 31;
			LblAgent.Text = "Agent";
			// 
			// PbAgent
			// 
			PbAgent.AccessibleDescription = "Agent icon image, as shown in the header of the 'agent' column.";
			PbAgent.AccessibleName = "Agent icon";
			PbAgent.AccessibleRole = AccessibleRole.Graphic;
			PbAgent.Image = Properties.Resources.agent_16;
			PbAgent.Location = new Point(12, 397);
			PbAgent.Name = "PbAgent";
			PbAgent.Size = new Size(16, 16);
			PbAgent.SizeMode = PictureBoxSizeMode.AutoSize;
			PbAgent.TabIndex = 30;
			PbAgent.TabStop = false;
			// 
			// LblService
			// 
			LblService.AccessibleDescription = "Service column description.";
			LblService.AccessibleName = "Service info";
			LblService.AccessibleRole = AccessibleRole.StaticText;
			LblService.AutoSize = true;
			LblService.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			LblService.Location = new Point(116, 398);
			LblService.Name = "LblService";
			LblService.Size = new Size(44, 15);
			LblService.TabIndex = 33;
			LblService.Text = "Service";
			// 
			// PbService
			// 
			PbService.AccessibleDescription = "Service icon image, as shown in the header of the 'service' column.";
			PbService.AccessibleName = "Service icon";
			PbService.AccessibleRole = AccessibleRole.Graphic;
			PbService.Image = Properties.Resources.service_16;
			PbService.Location = new Point(94, 397);
			PbService.Name = "PbService";
			PbService.Size = new Size(16, 16);
			PbService.SizeMode = PictureBoxSizeMode.AutoSize;
			PbService.TabIndex = 32;
			PbService.TabStop = false;
			// 
			// LblSpecificClient
			// 
			LblSpecificClient.AccessibleDescription = "Warning message that the selected sensor is only available for the HASS.Agent, not the satellite service.";
			LblSpecificClient.AccessibleName = "Compatibility warning";
			LblSpecificClient.AccessibleRole = AccessibleRole.StaticText;
			LblSpecificClient.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
			LblSpecificClient.ForeColor = Color.FromArgb(245, 42, 42);
			LblSpecificClient.Location = new Point(739, 18);
			LblSpecificClient.Name = "LblSpecificClient";
			LblSpecificClient.Size = new Size(155, 19);
			LblSpecificClient.TabIndex = 39;
			LblSpecificClient.Text = "HASS.Agent only!";
			LblSpecificClient.TextAlign = ContentAlignment.TopRight;
			LblSpecificClient.Visible = false;
			// 
			// BtnTest
			// 
			BtnTest.AccessibleDescription = "Tests the provided values to see if they return the expected value.";
			BtnTest.AccessibleName = "Test";
			BtnTest.AccessibleRole = AccessibleRole.PushButton;
			BtnTest.BackColor = Color.FromArgb(63, 63, 70);
			BtnTest.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			BtnTest.ForeColor = Color.FromArgb(241, 241, 241);
			BtnTest.Location = new Point(955, 416);
			BtnTest.Name = "BtnTest";
			BtnTest.Size = new Size(354, 25);
			BtnTest.Style.BackColor = Color.FromArgb(63, 63, 70);
			BtnTest.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
			BtnTest.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
			BtnTest.Style.ForeColor = Color.FromArgb(241, 241, 241);
			BtnTest.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
			BtnTest.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
			BtnTest.Style.PressedForeColor = Color.Black;
			BtnTest.TabIndex = 8;
			BtnTest.Text = Languages.SensorsMod_BtnTest;
			BtnTest.UseVisualStyleBackColor = false;
			BtnTest.Visible = false;
			BtnTest.Click += BtnTest_Click;
            // 
            // BtnAdvSettings
            // 
            BtnAdvSettings.AccessibleDescription = "Shows popup with advanced sensor settings.";
            BtnAdvSettings.AccessibleName = "Advanced settings";
            BtnAdvSettings.AccessibleRole = AccessibleRole.PushButton;
            BtnAdvSettings.BackColor = Color.FromArgb(63, 63, 70);
            BtnAdvSettings.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BtnAdvSettings.ForeColor = Color.FromArgb(241, 241, 241);
            BtnAdvSettings.Location = new Point(955, 446);
            BtnAdvSettings.Name = "BtnTest";
            BtnAdvSettings.Size = new Size(354, 25);
            BtnAdvSettings.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnAdvSettings.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnAdvSettings.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnAdvSettings.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnAdvSettings.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnAdvSettings.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnAdvSettings.Style.PressedForeColor = Color.Black;
            BtnAdvSettings.TabIndex = 48;
            BtnAdvSettings.Text = Languages.SensorsMod_BtnAdvancedSettings;
            BtnAdvSettings.UseVisualStyleBackColor = false;
            BtnAdvSettings.Visible = false;
            BtnAdvSettings.Click += BtnAdvSettings_Click;
            // 
            // CbNetworkCard
            // 
            CbNetworkCard.AccessibleDescription = "List of available network cards.";
			CbNetworkCard.AccessibleName = "Network cards";
			CbNetworkCard.AccessibleRole = AccessibleRole.DropList;
			CbNetworkCard.BackColor = Color.FromArgb(63, 63, 70);
			CbNetworkCard.DrawMode = DrawMode.OwnerDrawFixed;
			CbNetworkCard.DropDownHeight = 300;
			CbNetworkCard.DropDownStyle = ComboBoxStyle.DropDownList;
			CbNetworkCard.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
			CbNetworkCard.ForeColor = Color.FromArgb(241, 241, 241);
			CbNetworkCard.FormattingEnabled = true;
			CbNetworkCard.IntegralHeight = false;
			CbNetworkCard.Location = new Point(566, 338);
			CbNetworkCard.Name = "CbNetworkCard";
			CbNetworkCard.Size = new Size(328, 26);
			CbNetworkCard.TabIndex = 4;
			CbNetworkCard.Visible = false;
			// 
			// NumRound
			// 
			NumRound.AccessibleDescription = "The amount of digit after the comma . Only accepts numeric values.";
			NumRound.AccessibleName = "Round digits";
			NumRound.AccessibleRole = AccessibleRole.Text;
			NumRound.BackColor = Color.FromArgb(63, 63, 70);
			NumRound.BeforeTouchSize = new Size(83, 25);
			NumRound.Border3DStyle = Border3DStyle.Flat;
			NumRound.BorderColor = SystemColors.WindowFrame;
			NumRound.BorderStyle = BorderStyle.FixedSingle;
			NumRound.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			NumRound.ForeColor = Color.FromArgb(241, 241, 241);
			NumRound.Location = new Point(673, 266);
			NumRound.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
			NumRound.MaxLength = 10;
			NumRound.MetroColor = SystemColors.WindowFrame;
			NumRound.Name = "NumRound";
			NumRound.Size = new Size(83, 25);
			NumRound.TabIndex = 3;
			NumRound.Tag = "";
			NumRound.ThemeName = "Metro";
			NumRound.Value = new decimal(new int[] { 2, 0, 0, 0 });
			NumRound.Visible = false;
			NumRound.VisualStyle = Syncfusion.Windows.Forms.VisualStyle.Metro;
			// 
			// LblDigits
			// 
			LblDigits.AccessibleDescription = "Digits description";
			LblDigits.AccessibleName = "Digits description";
			LblDigits.AccessibleRole = AccessibleRole.StaticText;
			LblDigits.AutoSize = true;
			LblDigits.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblDigits.Location = new Point(763, 268);
			LblDigits.Name = "LblDigits";
			LblDigits.Size = new Size(147, 19);
			LblDigits.TabIndex = 42;
			LblDigits.Text = "digits after the comma";
			LblDigits.Visible = false;
			// 
			// CbApplyRounding
			// 
			CbApplyRounding.AccessibleDescription = "Enable rounding the value to the provided digits";
			CbApplyRounding.AccessibleName = "Round option";
			CbApplyRounding.AccessibleRole = AccessibleRole.CheckButton;
			CbApplyRounding.AutoSize = true;
			CbApplyRounding.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			CbApplyRounding.Location = new Point(566, 266);
			CbApplyRounding.Name = "CbApplyRounding";
			CbApplyRounding.Size = new Size(68, 23);
			CbApplyRounding.TabIndex = 43;
			CbApplyRounding.Text = Languages.SensorsMod_CbApplyRounding;
			CbApplyRounding.UseVisualStyleBackColor = true;
			CbApplyRounding.Visible = false;
			CbApplyRounding.CheckedChanged += CbRdValue_CheckedChanged;
			// 
			// LblFriendlyName
			// 
			LblFriendlyName.AccessibleDescription = "Sensor friendly name textbox description";
			LblFriendlyName.AccessibleName = "Sensor friendly name description";
			LblFriendlyName.AccessibleRole = AccessibleRole.StaticText;
			LblFriendlyName.AutoSize = true;
			LblFriendlyName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			LblFriendlyName.Location = new Point(566, 126);
			LblFriendlyName.Name = "LblFriendlyName";
			LblFriendlyName.Size = new Size(95, 19);
			LblFriendlyName.TabIndex = 45;
			LblFriendlyName.Text = "&Friendly name";
			// 
			// TbFriendlyName
			// 
			TbFriendlyName.AccessibleDescription = "The friendly name as which the sensor will show up in Home Assistant.";
			TbFriendlyName.AccessibleName = "Sensor friendly name";
			TbFriendlyName.AccessibleRole = AccessibleRole.Text;
			TbFriendlyName.BackColor = Color.FromArgb(63, 63, 70);
			TbFriendlyName.BorderStyle = BorderStyle.FixedSingle;
			TbFriendlyName.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			TbFriendlyName.ForeColor = Color.FromArgb(241, 241, 241);
			TbFriendlyName.Location = new Point(566, 148);
			TbFriendlyName.Name = "TbFriendlyName";
			TbFriendlyName.Size = new Size(328, 25);
			TbFriendlyName.TabIndex = 2;
			// 
			// LblOptional1
			// 
			LblOptional1.AccessibleDescription = "Indicates that the friendly name is optional.";
			LblOptional1.AccessibleName = "Friendly name optional";
			LblOptional1.AccessibleRole = AccessibleRole.StaticText;
			LblOptional1.AutoSize = true;
			LblOptional1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
			LblOptional1.Location = new Point(842, 129);
			LblOptional1.Name = "LblOptional1";
			LblOptional1.Size = new Size(51, 15);
			LblOptional1.TabIndex = 46;
			LblOptional1.Text = "optional";
			// 
			// CbIgnoreAvailability
			// 
			CbIgnoreAvailability.AccessibleDescription = "Do not send the availability topic for this sensor.";
			CbIgnoreAvailability.AccessibleName = "Ignore availability";
			CbIgnoreAvailability.AccessibleRole = AccessibleRole.CheckButton;
			CbIgnoreAvailability.AutoSize = true;
			CbIgnoreAvailability.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			CbIgnoreAvailability.Location = new Point(566, 230);
			CbIgnoreAvailability.Name = "CbIgnoreAvailability";
			CbIgnoreAvailability.Size = new Size(15, 14);
			CbIgnoreAvailability.TabIndex = 47;
			CbIgnoreAvailability.Text = Languages.SensorsMod_CbIgnoreAvailability;
			CbIgnoreAvailability.UseVisualStyleBackColor = true;
			// 
			// SensorsMod
			// 
			AccessibleDescription = "Create or modify a sensor.";
			AccessibleName = "Sensor mod";
			AccessibleRole = AccessibleRole.Window;
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			BackColor = Color.FromArgb(45, 45, 48);
			CaptionBarColor = Color.FromArgb(63, 63, 70);
			CaptionFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			CaptionForeColor = Color.FromArgb(241, 241, 241);
			ClientSize = new Size(1318, 522);
			Controls.Add(CbIgnoreAvailability);
			Controls.Add(LblOptional1);
			Controls.Add(LblFriendlyName);
			Controls.Add(TbFriendlyName);
			Controls.Add(LblDigits);
			Controls.Add(NumRound);
			Controls.Add(CbNetworkCard);
			Controls.Add(BtnTest);
			Controls.Add(LblSpecificClient);
			Controls.Add(LblService);
			Controls.Add(PbService);
			Controls.Add(LblAgent);
			Controls.Add(PbAgent);
			Controls.Add(LblMultiValue);
			Controls.Add(PbMultiValue);
			Controls.Add(TbSelectedType);
			Controls.Add(LvSensors);
			Controls.Add(NumInterval);
			Controls.Add(LblSetting3);
			Controls.Add(TbSetting3);
			Controls.Add(LblSetting2);
			Controls.Add(TbSetting2);
			Controls.Add(PnlDescription);
			Controls.Add(LblDescription);
			Controls.Add(LblSetting1);
			Controls.Add(BtnStore);
            Controls.Add(BtnAdvSettings);
			Controls.Add(TbSetting1);
			Controls.Add(LblType);
			Controls.Add(LblSeconds);
			Controls.Add(LblName);
			Controls.Add(LblUpdate);
			Controls.Add(TbName);
			Controls.Add(CbApplyRounding);
			DoubleBuffered = true;
			ForeColor = Color.FromArgb(241, 241, 241);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			MetroColor = Color.FromArgb(63, 63, 70);
			Name = "SensorsMod";
			ShowMaximizeBox = false;
			ShowMinimizeBox = false;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Sensor";
			Load += SensorMod_Load;
			ResizeEnd += SensorsMod_ResizeEnd;
			KeyUp += SensorsMod_KeyUp;
			Layout += SensorsMod_Layout;
			PnlDescription.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)NumInterval).EndInit();
			((System.ComponentModel.ISupportInitialize)PbMultiValue).EndInit();
			((System.ComponentModel.ISupportInitialize)PbAgent).EndInit();
			((System.ComponentModel.ISupportInitialize)PbService).EndInit();
			((System.ComponentModel.ISupportInitialize)NumRound).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Syncfusion.WinForms.Controls.SfButton BtnStore;
		private System.Windows.Forms.Label LblSetting1;
		private System.Windows.Forms.TextBox TbSetting1;
		private System.Windows.Forms.Label LblType;
		private System.Windows.Forms.Label LblName;
		private System.Windows.Forms.TextBox TbName;
		private System.Windows.Forms.Label LblSeconds;
		private System.Windows.Forms.Label LblUpdate;
		private System.Windows.Forms.Label LblDescription;
		private System.Windows.Forms.RichTextBox TbDescription;
		private System.Windows.Forms.Panel PnlDescription;
		private System.Windows.Forms.Label LblSetting2;
		private System.Windows.Forms.TextBox TbSetting2;
		private System.Windows.Forms.Label LblSetting3;
		private System.Windows.Forms.TextBox TbSetting3;
		private Syncfusion.Windows.Forms.Tools.NumericUpDownExt NumInterval;
		private ListView LvSensors;
		private ColumnHeader ClmSensorName;
		private ColumnHeader ClmMultiValue;
		private ColumnHeader ClmAgentCompatible;
		private ColumnHeader ClmSatelliteCompatible;
		private ColumnHeader ClmEmpty;
		private TextBox TbSelectedType;
		private ImageList ImgLv;
		private PictureBox PbMultiValue;
		private Label LblMultiValue;
		private Label LblAgent;
		private PictureBox PbAgent;
		private Label LblService;
		private PictureBox PbService;
		private Label LblSpecificClient;
		private ColumnHeader ClmId;
		private Syncfusion.WinForms.Controls.SfButton BtnTest;
        private Syncfusion.WinForms.Controls.SfButton BtnAdvSettings;
        private ComboBox CbNetworkCard;
		private Syncfusion.Windows.Forms.Tools.NumericUpDownExt NumRound;
		private Label LblDigits;
		internal CheckBox CbApplyRounding;
		private Label LblFriendlyName;
		private TextBox TbFriendlyName;
		private Label LblOptional1;
		internal CheckBox CbIgnoreAvailability;
	}
}

