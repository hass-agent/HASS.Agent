using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Forms.ChildApplications
{
    partial class CompatibilityTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompatibilityTask));
            LblInfo1 = new Label();
            LblTask1 = new Label();
            PbStep1CompatTask = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PbStep1CompatTask).BeginInit();
            SuspendLayout();
            // 
            // LblInfo1
            // 
            LblInfo1.AccessibleDescription = "Information about the task that's being performed.";
            LblInfo1.AccessibleName = "Task information";
            LblInfo1.AccessibleRole = AccessibleRole.StaticText;
            LblInfo1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblInfo1.Location = new Point(12, 39);
            LblInfo1.Name = "LblInfo1";
            LblInfo1.Size = new Size(595, 46);
            LblInfo1.TabIndex = 1;
            LblInfo1.Text = "Please wait a bit while the task is performed ..";
            LblInfo1.TextAlign = ContentAlignment.BottomCenter;
            // 
            // LblTask1
            // 
            LblTask1.AccessibleDescription = "Step one description.";
            LblTask1.AccessibleName = "Step one description";
            LblTask1.AccessibleRole = AccessibleRole.StaticText;
            LblTask1.AutoSize = true;
            LblTask1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            LblTask1.Location = new Point(257, 150);
            LblTask1.Name = "LblTask1";
            LblTask1.Size = new Size(155, 19);
            LblTask1.TabIndex = 57;
            LblTask1.Text = "Performing compat task";
            // 
            // PbStep1CompatTask
            // 
            PbStep1CompatTask.AccessibleDescription = "Step one status visualisation.";
            PbStep1CompatTask.AccessibleName = "Step one status";
            PbStep1CompatTask.AccessibleRole = AccessibleRole.Graphic;
            PbStep1CompatTask.Image = Properties.Resources.todo_32;
            PbStep1CompatTask.Location = new Point(207, 144);
            PbStep1CompatTask.Name = "PbStep1CompatTask";
            PbStep1CompatTask.Size = new Size(32, 32);
            PbStep1CompatTask.SizeMode = PictureBoxSizeMode.AutoSize;
            PbStep1CompatTask.TabIndex = 56;
            PbStep1CompatTask.TabStop = false;
            // 
            // CompatibilityTask
            // 
            AccessibleDescription = "Executes the port reservation and firewall configuration for the local API.";
            AccessibleName = "Port reservation";
            AccessibleRole = AccessibleRole.Window;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(45, 45, 48);
            CaptionBarColor = Color.FromArgb(63, 63, 70);
            CaptionFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            CaptionForeColor = Color.FromArgb(241, 241, 241);
            ClientSize = new Size(619, 284);
            Controls.Add(LblTask1);
            Controls.Add(PbStep1CompatTask);
            Controls.Add(LblInfo1);
            DoubleBuffered = true;
            ForeColor = Color.FromArgb(241, 241, 241);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MetroColor = Color.FromArgb(63, 63, 70);
            Name = "CompatibilityTask";
            ShowMaximizeBox = false;
            ShowMinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HASS.Agent Compatibility Task";
            Load += CompatibilityTask_Load;
            ResizeEnd += CompatibilityTask_ResizeEnd;
            ((System.ComponentModel.ISupportInitialize)PbStep1CompatTask).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label LblInfo1;
        private System.Windows.Forms.Label LblTask1;
        private System.Windows.Forms.PictureBox PbStep1CompatTask;
    }
}

