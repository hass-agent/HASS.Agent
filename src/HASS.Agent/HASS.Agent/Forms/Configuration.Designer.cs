﻿using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Forms
{
    partial class Configuration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuration));
            this.ConfigTabs = new Syncfusion.Windows.Forms.Tools.TabControlAdv();
            this.TabGeneral = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabExternalTools = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabHassApi = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabHotKey = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TablLocalApi = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabLocalStorage = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabLogging = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabMediaPlayer = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabMQTT = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabNotifications = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabService = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabStartup = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabTrayIcon = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabUpdates = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.TabNFC = new Syncfusion.Windows.Forms.Tools.TabPageAdv();
            this.BtnAbout = new Syncfusion.WinForms.Controls.SfButton();
            this.BtnHelp = new Syncfusion.WinForms.Controls.SfButton();
            this.BtnStore = new Syncfusion.WinForms.Controls.SfButton();
            this.PnlTabs = new System.Windows.Forms.Panel();
            this.BtnClose = new Syncfusion.WinForms.Controls.SfButton();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigTabs)).BeginInit();
            this.ConfigTabs.SuspendLayout();
            this.PnlTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConfigTabs
            // 
            this.ConfigTabs.AccessibleDescription = "Various tabs for configuring HASS.Agent.";
            this.ConfigTabs.AccessibleName = "Tabs";
            this.ConfigTabs.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTabList;
            this.ConfigTabs.ActiveTabColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ConfigTabs.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.ConfigTabs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ConfigTabs.BeforeTouchSize = new System.Drawing.Size(884, 548);
            this.ConfigTabs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConfigTabs.Controls.Add(this.TabGeneral);
            this.ConfigTabs.Controls.Add(this.TabExternalTools);
            this.ConfigTabs.Controls.Add(this.TabHassApi);
            this.ConfigTabs.Controls.Add(this.TabHotKey);
            this.ConfigTabs.Controls.Add(this.TablLocalApi);
            this.ConfigTabs.Controls.Add(this.TabLocalStorage);
            this.ConfigTabs.Controls.Add(this.TabLogging);
            this.ConfigTabs.Controls.Add(this.TabMediaPlayer);
            this.ConfigTabs.Controls.Add(this.TabMQTT);
            this.ConfigTabs.Controls.Add(this.TabNotifications);
            this.ConfigTabs.Controls.Add(this.TabService);
            this.ConfigTabs.Controls.Add(this.TabStartup);
            this.ConfigTabs.Controls.Add(this.TabTrayIcon);
            this.ConfigTabs.Controls.Add(this.TabUpdates);
            this.ConfigTabs.Controls.Add(this.TabNFC);
            this.ConfigTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConfigTabs.FixedSingleBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.ConfigTabs.FocusOnTabClick = false;
            this.ConfigTabs.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ConfigTabs.InactiveTabColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.ConfigTabs.Location = new System.Drawing.Point(0, 0);
            this.ConfigTabs.Name = "ConfigTabs";
            this.ConfigTabs.RotateTextWhenVertical = true;
            this.ConfigTabs.Size = new System.Drawing.Size(884, 548);
            this.ConfigTabs.TabIndex = 0;
            this.ConfigTabs.TabPanelBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ConfigTabs.TabStyle = typeof(Syncfusion.Windows.Forms.Tools.TabRendererMetro);
            this.ConfigTabs.ThemeName = "TabRendererMetro";
            this.ConfigTabs.ThemesEnabled = true;
            this.ConfigTabs.ThemeStyle.PrimitiveButtonStyle.DisabledNextPageImage = null;
            // 
            // TabGeneral
            // 
            this.TabGeneral.AccessibleDescription = "Contains the \'general configuration\' controls.";
            this.TabGeneral.AccessibleName = "General";
            this.TabGeneral.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabGeneral.AutoScroll = true;
            this.TabGeneral.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabGeneral.Image = null;
            this.TabGeneral.ImageSize = new System.Drawing.Size(16, 16);
            this.TabGeneral.Location = new System.Drawing.Point(142, 2);
            this.TabGeneral.Name = "TabGeneral";
            this.TabGeneral.ShowCloseButton = false;
            this.TabGeneral.Size = new System.Drawing.Size(740, 544);
            this.TabGeneral.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabGeneral.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabGeneral.TabIndex = 9;
            this.TabGeneral.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabGeneral;
            this.TabGeneral.ThemesEnabled = false;
            // 
            // TabExternalTools
            // 
            this.TabExternalTools.AccessibleDescription = "Contains the \'external tools\' controls.";
            this.TabExternalTools.AccessibleName = "External tools";
            this.TabExternalTools.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabExternalTools.AutoScroll = true;
            this.TabExternalTools.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabExternalTools.Image = null;
            this.TabExternalTools.ImageSize = new System.Drawing.Size(16, 16);
            this.TabExternalTools.Location = new System.Drawing.Point(142, 2);
            this.TabExternalTools.Name = "TabExternalTools";
            this.TabExternalTools.ShowCloseButton = true;
            this.TabExternalTools.Size = new System.Drawing.Size(740, 544);
            this.TabExternalTools.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabExternalTools.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabExternalTools.TabIndex = 10;
            this.TabExternalTools.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabExternalTools;
            this.TabExternalTools.ThemesEnabled = false;
            // 
            // TabHassApi
            // 
            this.TabHassApi.AccessibleDescription = "Contains the \'Home Assistant API\' controls.";
            this.TabHassApi.AccessibleName = "HA API";
            this.TabHassApi.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabHassApi.AutoScroll = true;
            this.TabHassApi.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabHassApi.Image = null;
            this.TabHassApi.ImageSize = new System.Drawing.Size(16, 16);
            this.TabHassApi.Location = new System.Drawing.Point(142, 2);
            this.TabHassApi.Name = "TabHassApi";
            this.TabHassApi.ShowCloseButton = false;
            this.TabHassApi.Size = new System.Drawing.Size(740, 544);
            this.TabHassApi.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabHassApi.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabHassApi.TabIndex = 1;
            this.TabHassApi.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabHassApi;
            this.TabHassApi.ThemesEnabled = false;
            // 
            // TabHotKey
            // 
            this.TabHotKey.AccessibleDescription = "Contains the \'hotkey\' controls.";
            this.TabHotKey.AccessibleName = "Hotkey";
            this.TabHotKey.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabHotKey.AutoScroll = true;
            this.TabHotKey.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabHotKey.Image = null;
            this.TabHotKey.ImageSize = new System.Drawing.Size(16, 16);
            this.TabHotKey.Location = new System.Drawing.Point(142, 2);
            this.TabHotKey.Name = "TabHotKey";
            this.TabHotKey.ShowCloseButton = true;
            this.TabHotKey.Size = new System.Drawing.Size(740, 544);
            this.TabHotKey.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabHotKey.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabHotKey.TabIndex = 5;
            this.TabHotKey.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabHotKey;
            this.TabHotKey.ThemesEnabled = false;
            // 
            // TablLocalApi
            // 
            this.TablLocalApi.AccessibleDescription = "Contains the \'local API\' controls.";
            this.TablLocalApi.AccessibleName = "Local API";
            this.TablLocalApi.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TablLocalApi.Image = null;
            this.TablLocalApi.ImageSize = new System.Drawing.Size(16, 16);
            this.TablLocalApi.Location = new System.Drawing.Point(142, 2);
            this.TablLocalApi.Name = "TablLocalApi";
            this.TablLocalApi.ShowCloseButton = true;
            this.TablLocalApi.Size = new System.Drawing.Size(740, 544);
            this.TablLocalApi.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TablLocalApi.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TablLocalApi.TabIndex = 13;
            this.TablLocalApi.Text = Languages.Configuration_TablLocalApi;
            this.TablLocalApi.ThemesEnabled = false;
            // 
            // TabLocalStorage
            // 
            this.TabLocalStorage.AccessibleDescription = "Contains the \'local storage\' controls.";
            this.TabLocalStorage.AccessibleName = "Local storage";
            this.TabLocalStorage.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabLocalStorage.AutoScroll = true;
            this.TabLocalStorage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabLocalStorage.Image = null;
            this.TabLocalStorage.ImageSize = new System.Drawing.Size(16, 16);
            this.TabLocalStorage.Location = new System.Drawing.Point(142, 2);
            this.TabLocalStorage.Name = "TabLocalStorage";
            this.TabLocalStorage.ShowCloseButton = true;
            this.TabLocalStorage.Size = new System.Drawing.Size(740, 544);
            this.TabLocalStorage.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabLocalStorage.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabLocalStorage.TabIndex = 8;
            this.TabLocalStorage.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabLocalStorage;
            this.TabLocalStorage.ThemesEnabled = false;
            // 
            // TabLogging
            // 
            this.TabLogging.AccessibleDescription = "Contains the \'logging\' controls.";
            this.TabLogging.AccessibleName = "Logging";
            this.TabLogging.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabLogging.AutoScroll = true;
            this.TabLogging.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabLogging.Image = null;
            this.TabLogging.ImageSize = new System.Drawing.Size(16, 16);
            this.TabLogging.Location = new System.Drawing.Point(142, 2);
            this.TabLogging.Name = "TabLogging";
            this.TabLogging.ShowCloseButton = true;
            this.TabLogging.Size = new System.Drawing.Size(740, 544);
            this.TabLogging.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabLogging.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabLogging.TabIndex = 7;
            this.TabLogging.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabLogging;
            this.TabLogging.ThemesEnabled = false;
            // 
            // TabMediaPlayer
            // 
            this.TabMediaPlayer.AccessibleDescription = "Contains the \'mediaplayer\' controls.";
            this.TabMediaPlayer.AccessibleName = "Mediaplayer";
            this.TabMediaPlayer.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabMediaPlayer.Image = null;
            this.TabMediaPlayer.ImageSize = new System.Drawing.Size(16, 16);
            this.TabMediaPlayer.Location = new System.Drawing.Point(142, 2);
            this.TabMediaPlayer.Name = "TabMediaPlayer";
            this.TabMediaPlayer.ShowCloseButton = true;
            this.TabMediaPlayer.Size = new System.Drawing.Size(740, 544);
            this.TabMediaPlayer.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabMediaPlayer.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabMediaPlayer.TabIndex = 12;
            this.TabMediaPlayer.Text = Languages.Configuration_TabMediaPlayer;
            this.TabMediaPlayer.ThemesEnabled = false;
            // 
            // TabMQTT
            // 
            this.TabMQTT.AccessibleDescription = "Contains the \'MQTT configuration\' controls.";
            this.TabMQTT.AccessibleName = "MQTT";
            this.TabMQTT.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabMQTT.AutoScroll = true;
            this.TabMQTT.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabMQTT.Image = null;
            this.TabMQTT.ImageSize = new System.Drawing.Size(16, 16);
            this.TabMQTT.Location = new System.Drawing.Point(142, 2);
            this.TabMQTT.Name = "TabMQTT";
            this.TabMQTT.ShowCloseButton = true;
            this.TabMQTT.Size = new System.Drawing.Size(740, 544);
            this.TabMQTT.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabMQTT.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabMQTT.TabIndex = 3;
            this.TabMQTT.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabMQTT;
            this.TabMQTT.ThemesEnabled = false;
            // 
            // TabNotifications
            // 
            this.TabNotifications.AccessibleDescription = "Contains the \'notifications\' controls.";
            this.TabNotifications.AccessibleName = "Notifications";
            this.TabNotifications.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabNotifications.AutoScroll = true;
            this.TabNotifications.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabNotifications.Image = null;
            this.TabNotifications.ImageSize = new System.Drawing.Size(16, 16);
            this.TabNotifications.Location = new System.Drawing.Point(142, 2);
            this.TabNotifications.Name = "TabNotifications";
            this.TabNotifications.ShowCloseButton = true;
            this.TabNotifications.Size = new System.Drawing.Size(740, 544);
            this.TabNotifications.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabNotifications.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabNotifications.TabIndex = 2;
            this.TabNotifications.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabNotifications;
            this.TabNotifications.ThemesEnabled = false;
            // 
            // TabService
            // 
            this.TabService.AccessibleDescription = "Contains the \'satellite service\' controls.";
            this.TabService.AccessibleName = "Satellite service";
            this.TabService.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabService.AutoScroll = true;
            this.TabService.Image = null;
            this.TabService.ImageSize = new System.Drawing.Size(16, 16);
            this.TabService.Location = new System.Drawing.Point(142, 2);
            this.TabService.Name = "TabService";
            this.TabService.ShowCloseButton = true;
            this.TabService.Size = new System.Drawing.Size(740, 544);
            this.TabService.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabService.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabService.TabIndex = 11;
            this.TabService.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabService;
            this.TabService.ThemesEnabled = false;
            // 
            // TabStartup
            // 
            this.TabStartup.AccessibleDescription = "Contains the \'startup\' controls.";
            this.TabStartup.AccessibleName = "Startup";
            this.TabStartup.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabStartup.AutoScroll = true;
            this.TabStartup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabStartup.Image = null;
            this.TabStartup.ImageSize = new System.Drawing.Size(16, 16);
            this.TabStartup.Location = new System.Drawing.Point(142, 2);
            this.TabStartup.Name = "TabStartup";
            this.TabStartup.ShowCloseButton = true;
            this.TabStartup.Size = new System.Drawing.Size(740, 544);
            this.TabStartup.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabStartup.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabStartup.TabIndex = 4;
            this.TabStartup.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabStartup;
            this.TabStartup.ThemesEnabled = false;
            // 
            // TabTrayIcon
            // 
            this.TabTrayIcon.AccessibleDescription = "Contains the \'tray icon\' controls.";
            this.TabTrayIcon.AccessibleName = "Tray icon";
            this.TabTrayIcon.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabTrayIcon.Image = null;
            this.TabTrayIcon.ImageSize = new System.Drawing.Size(16, 16);
            this.TabTrayIcon.Location = new System.Drawing.Point(142, 2);
            this.TabTrayIcon.Name = "TabTrayIcon";
            this.TabTrayIcon.ShowCloseButton = true;
            this.TabTrayIcon.Size = new System.Drawing.Size(740, 544);
            this.TabTrayIcon.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabTrayIcon.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabTrayIcon.TabIndex = 14;
            this.TabTrayIcon.Text = Languages.Configuration_TabTrayIcon;
            this.TabTrayIcon.ThemesEnabled = false;
            // 
            // TabUpdates
            // 
            this.TabUpdates.AccessibleDescription = "Contains the \'updates\' controls.";
            this.TabUpdates.AccessibleName = "Updates";
            this.TabUpdates.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabUpdates.AutoScroll = true;
            this.TabUpdates.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabUpdates.Image = null;
            this.TabUpdates.ImageSize = new System.Drawing.Size(16, 16);
            this.TabUpdates.Location = new System.Drawing.Point(142, 2);
            this.TabUpdates.Name = "TabUpdates";
            this.TabUpdates.ShowCloseButton = true;
            this.TabUpdates.Size = new System.Drawing.Size(740, 544);
            this.TabUpdates.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabUpdates.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabUpdates.TabIndex = 6;
            this.TabUpdates.Text = global::HASS.Agent.Resources.Localization.Languages.Configuration_TabUpdates;
            this.TabUpdates.ThemesEnabled = false;
            // 
            // TabNFC
            // 
            this.TabNFC.AccessibleDescription = "Contains the \'NFC\' controls.";
            this.TabNFC.AccessibleName = "NFC";
            this.TabNFC.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTab;
            this.TabNFC.Image = null;
            this.TabNFC.ImageSize = new System.Drawing.Size(16, 16);
            this.TabNFC.Location = new System.Drawing.Point(142, 2);
            this.TabNFC.Name = "TabNFC";
            this.TabNFC.ShowCloseButton = true;
            this.TabNFC.Size = new System.Drawing.Size(740, 544);
            this.TabNFC.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.TabNFC.TabForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.TabNFC.TabIndex = 15;
            this.TabNFC.Text = Languages.Configuration_NFC;
            this.TabNFC.ThemesEnabled = false;
            // 
            // BtnAbout
            // 
            this.BtnAbout.AccessibleDescription = "Opens the about window.";
            this.BtnAbout.AccessibleName = "About";
            this.BtnAbout.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnAbout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnAbout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnAbout.Location = new System.Drawing.Point(-1, 563);
            this.BtnAbout.Name = "BtnAbout";
            this.BtnAbout.Size = new System.Drawing.Size(172, 31);
            this.BtnAbout.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnAbout.Style.FocusedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnAbout.Style.FocusedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnAbout.Style.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnAbout.Style.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnAbout.Style.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnAbout.Style.PressedForeColor = System.Drawing.Color.Black;
            this.BtnAbout.TabIndex = 3;
            this.BtnAbout.Text = Languages.Configuration_BtnAbout;
            this.BtnAbout.UseVisualStyleBackColor = false;
            this.BtnAbout.Click += new System.EventHandler(this.BtnAbout_Click);
            // 
            // BtnHelp
            // 
            this.BtnHelp.AccessibleDescription = "Opens the help and contact window.";
            this.BtnHelp.AccessibleName = "Help";
            this.BtnHelp.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnHelp.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnHelp.Location = new System.Drawing.Point(177, 563);
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Size = new System.Drawing.Size(172, 31);
            this.BtnHelp.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnHelp.Style.FocusedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnHelp.Style.FocusedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnHelp.Style.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnHelp.Style.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnHelp.Style.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnHelp.Style.PressedForeColor = System.Drawing.Color.Black;
            this.BtnHelp.TabIndex = 2;
            this.BtnHelp.Text = Languages.Configuration_BtnHelp;
            this.BtnHelp.UseVisualStyleBackColor = false;
            this.BtnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // BtnStore
            // 
            this.BtnStore.AccessibleDescription = "Saves your settings and closes the window. Might show a messagebox with additiona" +
    "l info.";
            this.BtnStore.AccessibleName = "Save and close";
            this.BtnStore.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnStore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnStore.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnStore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnStore.Location = new System.Drawing.Point(533, 563);
            this.BtnStore.Name = "BtnStore";
            this.BtnStore.Size = new System.Drawing.Size(352, 31);
            this.BtnStore.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnStore.Style.DisabledForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnStore.Style.FocusedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnStore.Style.FocusedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnStore.Style.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnStore.Style.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnStore.Style.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnStore.Style.PressedForeColor = System.Drawing.Color.Black;
            this.BtnStore.TabIndex = 0;
            this.BtnStore.Text = Languages.Configuration_BtnStore;
            this.BtnStore.UseVisualStyleBackColor = false;
            this.BtnStore.Click += new System.EventHandler(this.BtnStore_Click);
            // 
            // PnlTabs
            // 
            this.PnlTabs.AccessibleDescription = "Contains the tabs control.";
            this.PnlTabs.AccessibleName = "Tab pane";
            this.PnlTabs.AccessibleRole = System.Windows.Forms.AccessibleRole.Pane;
            this.PnlTabs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlTabs.Controls.Add(this.ConfigTabs);
            this.PnlTabs.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PnlTabs.Location = new System.Drawing.Point(0, 7);
            this.PnlTabs.Name = "PnlTabs";
            this.PnlTabs.Size = new System.Drawing.Size(886, 550);
            this.PnlTabs.TabIndex = 11;
            // 
            // BtnClose
            // 
            this.BtnClose.AccessibleDescription = "Closes the window without saving your changes.";
            this.BtnClose.AccessibleName = "Close no save";
            this.BtnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnClose.Location = new System.Drawing.Point(355, 563);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(172, 31);
            this.BtnClose.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnClose.Style.FocusedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnClose.Style.FocusedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnClose.Style.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnClose.Style.HoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BtnClose.Style.HoverForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.BtnClose.Style.PressedForeColor = System.Drawing.Color.Black;
            this.BtnClose.TabIndex = 1;
            this.BtnClose.Text = Languages.Configuration_BtnClose;
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // Configuration
            // 
            this.AccessibleDescription = "Contains the various configuration panels for HASS.Agent.";
            this.AccessibleName = "Configuration";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.CaptionBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.CaptionFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CaptionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(886, 595);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.PnlTabs);
            this.Controls.Add(this.BtnStore);
            this.Controls.Add(this.BtnAbout);
            this.Controls.Add(this.BtnHelp);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.Name = "Configuration";
            this.ShowMaximizeBox = false;
            this.ShowMinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = Languages.Configuration_Title;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Configuration_FormClosing);
            this.Load += new System.EventHandler(this.Configuration_Load);
            this.ResizeEnd += new System.EventHandler(this.Configuration_ResizeEnd);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Configuration_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.ConfigTabs)).EndInit();
            this.ConfigTabs.ResumeLayout(false);
            this.PnlTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.TabControlAdv ConfigTabs;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabHassApi;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabNotifications;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabMQTT;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabStartup;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabHotKey;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabUpdates;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabLogging;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabLocalStorage;
        private Syncfusion.WinForms.Controls.SfButton BtnAbout;
        private Syncfusion.WinForms.Controls.SfButton BtnHelp;
        private Syncfusion.WinForms.Controls.SfButton BtnStore;
        private System.Windows.Forms.Panel PnlTabs;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabGeneral;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabExternalTools;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabService;
        private Syncfusion.WinForms.Controls.SfButton BtnClose;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TablLocalApi;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabMediaPlayer;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabTrayIcon;
        private Syncfusion.Windows.Forms.Tools.TabPageAdv TabNFC;
    }
}

