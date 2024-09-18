using HASS.Agent.Functions;
using HASS.Agent.Models.Internal;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using Serilog;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.Controls.Styles;

namespace HASS.Agent.Forms.Commands.CommandConfig;

public partial class AdvancedSensorSettings : MetroForm
{

    public SensorAdvancedSettings AdvancedSettings { get; private set; } = null;

    public AdvancedSensorSettings(string wmiAdvancedInfo = "")
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(wmiAdvancedInfo))
            return;

        var advancedInfo = JsonConvert.DeserializeObject<SensorAdvancedSettings>(wmiAdvancedInfo);
        if (advancedInfo == null)
            return;

        AdvancedSettings = advancedInfo;
    }

    private void AdvancedSensorConfig_Load(object sender, EventArgs e)
    {
        CaptionBarHeight = 26;

        if (AdvancedSettings != null)
            SetStoredVariables();

        Opacity = 100;
    }

    private void SetStoredVariables()
    {
        TbDeviceClass.Text = AdvancedSettings.DeviceClass;
        TbUnitOfMeasurement.Text = AdvancedSettings.UnitOfMeasurement;
        TbStateClass.Text = AdvancedSettings.StateClass;
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        AdvancedSettings ??= new SensorAdvancedSettings();

        AdvancedSettings.DeviceClass = TbDeviceClass.Text;
        AdvancedSettings.UnitOfMeasurement = TbUnitOfMeasurement.Text;
        AdvancedSettings.StateClass = TbStateClass.Text;

        DialogResult = DialogResult.OK;
    }
}
