using System.Windows.Forms;
using HASS.Agent.HomeAssistant;
using HASS.Agent.MQTT;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Shared.Functions;
using Serilog;
using Syncfusion.Windows.Forms;

namespace HASS.Agent.Controls.Onboarding
{
    // ReSharper disable once InconsistentNaming
    public partial class OnboardingMqtt : UserControl
    {
        private bool _initializing = true;

        public OnboardingMqtt()
        {
            InitializeComponent();
        }

        private void OnboardingMqtt_Load(object sender, EventArgs e)
        {
            // let's see if we can get the host from the provided HASS uri
            if (!string.IsNullOrEmpty(Variables.AppSettings.HassUri))
            {
                try
                {
                    var host = new Uri(Variables.AppSettings.HassUri).Host;
                    TbMqttAddress.Text = host;
                }
                catch (Exception ex)
                {
                    Log.Error("[MQTT] Unable to parse URI {uri}: {msg}", Variables.AppSettings.HassUri, ex.Message);
                }
            }

            // if the above process failed somewhere, just enter the entire address (if any)
            if (string.IsNullOrEmpty(TbMqttAddress.Text))
            {
                TbMqttAddress.Text = Variables.AppSettings.MqttAddress;
            }

            // optionally set default port
            if (Variables.AppSettings.MqttPort < 1)
            {
                Variables.AppSettings.MqttPort = 1883;
            }
            NumMqttPort.Value = Variables.AppSettings.MqttPort;

            CbMqttTls.Checked = Variables.AppSettings.MqttUseTls;
            TbMqttUsername.Text = Variables.AppSettings.MqttUsername;
            TbMqttPassword.Text = Variables.AppSettings.MqttPassword;
            TbMqttDiscoveryPrefix.Text = Variables.AppSettings.MqttDiscoveryPrefix;
            CbEnableMqtt.CheckState = Variables.AppSettings.MqttEnabled ? CheckState.Checked : CheckState.Unchecked;

            _initializing = false;

            ActiveControl = !string.IsNullOrEmpty(TbMqttAddress.Text) ? TbMqttUsername : TbMqttAddress;
        }

        internal bool Store()
        {
            Variables.AppSettings.MqttAddress = TbMqttAddress.Text;
            Variables.AppSettings.MqttPort = (int)NumMqttPort.Value;
            Variables.AppSettings.MqttUseTls = CbMqttTls.Checked;
            Variables.AppSettings.MqttUsername = TbMqttUsername.Text;
            Variables.AppSettings.MqttPassword = TbMqttPassword.Text;
            Variables.AppSettings.MqttDiscoveryPrefix = TbMqttDiscoveryPrefix.Text;
            Variables.AppSettings.MqttEnabled = CbEnableMqtt.CheckState == CheckState.Checked;

            return true;
        }

        private void CbMqttTls_CheckedChanged(object sender, EventArgs e)
        {
            if (_initializing)
            {
                return;
            }

            NumMqttPort.Value = CbMqttTls.Checked ? 8883 : 1883;
        }

        private void PbShow_Click(object sender, EventArgs e) => TbMqttPassword.UseSystemPasswordChar = !TbMqttPassword.UseSystemPasswordChar;

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            var address = TbMqttAddress.Text.Trim();
            var port = (int)NumMqttPort.Value;
            var username = TbMqttUsername.Text.Trim();
            var password = TbMqttPassword.Text.Trim();
            var useTls = CbMqttTls.Checked;
            var useWebSocket = CbUseWebSocket.Checked;

            CbEnableMqtt.Enabled = false;
            CbUseWebSocket.Enabled = false;
            CbMqttTls.Enabled = false;
            TbMqttAddress.Enabled = false;
            NumMqttPort.Enabled = false;
            TbMqttUsername.Enabled = false;
            TbMqttPassword.Enabled = false;
            TbMqttDiscoveryPrefix.Enabled = false;
            BtnTest.Enabled = false;
            BtnTest.Text = Languages.OnboardingApi_BtnTest_Testing;

            var result = await MqttManager.TestConnection(address, port, useTls, useWebSocket, username, password);
            if (!result)
            {
                MessageBoxAdv.Show(this, Languages.OnboardingMqtt_BtnTest_MessageError, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBoxAdv.Show(this, Languages.OnboardingMqtt_BtnTest_MessageOk, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            CbEnableMqtt.Enabled = true;
            CbUseWebSocket.Enabled = true;
            CbMqttTls.Enabled = true;
            TbMqttAddress.Enabled = true;
            NumMqttPort.Enabled = true;
            TbMqttUsername.Enabled = true;
            TbMqttPassword.Enabled = true;
            TbMqttDiscoveryPrefix.Enabled = true;
            BtnTest.Enabled = true;
            BtnTest.Text = Languages.OnboardingApi_BtnTest;
        }
    }
}
