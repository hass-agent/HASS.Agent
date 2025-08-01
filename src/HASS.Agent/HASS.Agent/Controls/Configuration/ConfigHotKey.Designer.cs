using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Controls.Configuration
{
    partial class ConfigHotKey
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
            BtnClearHotKey = new Syncfusion.WinForms.Controls.SfButton();
            LblInfo1 = new Label();
            TbQuickActionsHotkey = new TextBox();
            CbEnableQuickActionsHotkey = new CheckBox();
            LblHotkeyCombo = new Label();
            SuspendLayout();
            // 
            // BtnClearHotKey
            // 
            BtnClearHotKey.AccessibleDescription = "Clears the hotkey combination.";
            BtnClearHotKey.AccessibleName = "Clear hotkey";
            BtnClearHotKey.AccessibleRole = AccessibleRole.PushButton;
            BtnClearHotKey.BackColor = Color.FromArgb(63, 63, 70);
            BtnClearHotKey.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BtnClearHotKey.ForeColor = Color.FromArgb(241, 241, 241);
            BtnClearHotKey.Location = new Point(438, 279);
            BtnClearHotKey.Name = "BtnClearHotKey";
            BtnClearHotKey.Size = new Size(102, 25);
            BtnClearHotKey.Style.BackColor = Color.FromArgb(63, 63, 70);
            BtnClearHotKey.Style.FocusedBackColor = Color.FromArgb(63, 63, 70);
            BtnClearHotKey.Style.FocusedForeColor = Color.FromArgb(241, 241, 241);
            BtnClearHotKey.Style.ForeColor = Color.FromArgb(241, 241, 241);
            BtnClearHotKey.Style.HoverBackColor = Color.FromArgb(63, 63, 70);
            BtnClearHotKey.Style.HoverForeColor = Color.FromArgb(241, 241, 241);
            BtnClearHotKey.Style.PressedForeColor = Color.Black;
            BtnClearHotKey.TabIndex = 2;
            BtnClearHotKey.Text = Languages.ConfigHotKey_BtnClearHotKey;
            BtnClearHotKey.UseVisualStyleBackColor = false;
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "Hotkey information.";
            LblInfo1.AccessibleName = "Information";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo1.Location = new Point(70, 36);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(594, 108);
            LblInfo1.TabIndex = 17;
            LblInfo1.Text = "An easy way to pull up your quick actions is to use a global hotkey.\r\n\r\nThis way, whatever you're doing on your machine, you can always interact with Home Assistant.";
            // 
            // TbQuickActionsHotkey
            // 
            TbQuickActionsHotkey.AccessibleDescription = "The hotkey combination that will show your Quick Actions when triggered.";
            TbQuickActionsHotkey.AccessibleName = "Hotkey combination";
            TbQuickActionsHotkey.AccessibleRole = AccessibleRole.HotkeyField;
            TbQuickActionsHotkey.BackColor = Color.FromArgb(63, 63, 70);
            TbQuickActionsHotkey.BorderStyle = BorderStyle.FixedSingle;
            TbQuickActionsHotkey.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            TbQuickActionsHotkey.ForeColor = Color.FromArgb(241, 241, 241);
            TbQuickActionsHotkey.Location = new Point(201, 279);
            TbQuickActionsHotkey.Name = "TbQuickActionsHotkey";
            TbQuickActionsHotkey.Size = new Size(231, 25);
            TbQuickActionsHotkey.TabIndex = 1;
            // 
            // CbEnableQuickActionsHotkey
            // 
            CbEnableQuickActionsHotkey.AccessibleDescription = "Enable the quick actions hotkey.";
            CbEnableQuickActionsHotkey.AccessibleName = "Quick actions hotkey";
            CbEnableQuickActionsHotkey.AccessibleRole = AccessibleRole.CheckButton;
            CbEnableQuickActionsHotkey.AutoSize = true;
            CbEnableQuickActionsHotkey.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CbEnableQuickActionsHotkey.Location = new Point(201, 181);
            CbEnableQuickActionsHotkey.Name = "CbEnableQuickActionsHotkey";
            CbEnableQuickActionsHotkey.Size = new Size(204, 23);
            CbEnableQuickActionsHotkey.TabIndex = 0;
            CbEnableQuickActionsHotkey.Text = Languages.ConfigHotKey_CbEnableQuickActionsHotkey;
            CbEnableQuickActionsHotkey.UseVisualStyleBackColor = true;
            // 
            // LblHotkeyCombo
            // 
            LblHotkeyCombo.AccessibleDescription = "Hotkey textbox description";
            LblHotkeyCombo.AccessibleName = "Hotkey info";
            LblHotkeyCombo.AccessibleRole = AccessibleRole.StaticText;
            LblHotkeyCombo.AutoSize = true;
            LblHotkeyCombo.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblHotkeyCombo.Location = new Point(201, 257);
            LblHotkeyCombo.Name = "LblHotkeyCombo";
            LblHotkeyCombo.Size = new Size(136, 19);
            LblHotkeyCombo.TabIndex = 14;
            LblHotkeyCombo.Text = "&Hotkey Combination";
            // 
            // ConfigHotKey
            // 
            AccessibleDescription = "Panel containing the hotkey configuration.";
            AccessibleName = "Hotkey";
            AccessibleRole = AccessibleRole.Pane;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            Controls.Add(BtnClearHotKey);
            Controls.Add(LblInfo1);
            Controls.Add(TbQuickActionsHotkey);
            Controls.Add(CbEnableQuickActionsHotkey);
            Controls.Add(LblHotkeyCombo);
            ForeColor = Color.FromArgb(241, 241, 241);
            Margin = new Padding(4);
            Name = "ConfigHotKey";
            Size = new Size(700, 544);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label LblInfo1;
        private System.Windows.Forms.Label LblHotkeyCombo;
        internal Syncfusion.WinForms.Controls.SfButton BtnClearHotKey;
        internal System.Windows.Forms.TextBox TbQuickActionsHotkey;
        internal System.Windows.Forms.CheckBox CbEnableQuickActionsHotkey;
    }
}
