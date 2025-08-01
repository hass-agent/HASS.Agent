using HASS.Agent.API;
using HASS.Agent.Commands;
using HASS.Agent.Compatibility;
using HASS.Agent.Functions;
using HASS.Agent.Managers;
using HASS.Agent.Properties;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Sensors;
using HASS.Agent.Settings;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;
using Syncfusion.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace HASS.Agent.Forms.ChildApplications
{
    public partial class CompatibilityTask : MetroForm
    {
        private ICompatibilityTask _comatibilityTask;

        public CompatibilityTask(ICompatibilityTask compatibilityTask)
        {
            _comatibilityTask = compatibilityTask;

            InitializeComponent();

            LblTask1.Text = compatibilityTask.Name;
        }

        private void CompatibilityTask_Load(object sender, EventArgs e) => ProcessCompatibilityTask();

        /// <summary>
        /// Performs compatibility task
        /// </summary>
        private async void ProcessCompatibilityTask()
        {
            // set busy indicator
            PbStep1CompatTask.Image = Properties.Resources.small_loader_32;

            await Task.Delay(TimeSpan.FromSeconds(2));

            var (taskDone, errorMessage) = await _comatibilityTask.Perform();
            PbStep1CompatTask.Image = taskDone ? Properties.Resources.done_32 : Properties.Resources.failed_32;

            if (!taskDone)
            {
                MessageBoxAdv.Show(this, errorMessage, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // wait a bit to show the 'completed' check
                await Task.Delay(750);
            }

            Log.CloseAndFlush();

            Environment.Exit(taskDone ? 0 : -1);
        }

        private void CompatibilityTask_ResizeEnd(object sender, EventArgs e)
        {
            if (Variables.ShuttingDown) return;
            if (!IsHandleCreated) return;
            if (IsDisposed) return;

            try
            {
                Refresh();
            }
            catch
            {
                // best effort
            }
        }
    }
}
