using HASS.Agent.Functions;
using HASS.Agent.Models.Internal;
using HASS.Agent.Resources.Localization;
using Newtonsoft.Json;
using Serilog;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.Controls.Styles;

namespace HASS.Agent.Forms.Commands.CommandConfig;

public partial class WMISensorConfig : MetroForm
{

    public WMIAdvancedInfo WMIAdvancedInfo { get; private set; } = null;

    public WMISensorConfig(string wmiAdvancedInfo = "")
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(wmiAdvancedInfo))
            return;

        var advancedInfo = JsonConvert.DeserializeObject<WMIAdvancedInfo>(wmiAdvancedInfo);
        if (advancedInfo == null)
            return;

        WMIAdvancedInfo = advancedInfo;
    }

    private void WMISensorConfig_Load(object sender, EventArgs e)
    {
        CaptionBarHeight = 26;

        if (WMIAdvancedInfo != null)
            SetStoredVariables();

        // show ourselves
        Opacity = 100;
    }

    private void SetStoredVariables()
    {
        TbDeviceClass.Text = WMIAdvancedInfo.DeviceClass;
        TbUnitOfMeasurement.Text = WMIAdvancedInfo.UnitOfMeasurement;
        TbStateClass.Text = WMIAdvancedInfo.StateClass;
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        WMIAdvancedInfo ??= new WMIAdvancedInfo();

        WMIAdvancedInfo.DeviceClass = TbDeviceClass.Text;
        WMIAdvancedInfo.UnitOfMeasurement = TbUnitOfMeasurement.Text;
        WMIAdvancedInfo.StateClass = TbStateClass.Text;

        DialogResult = DialogResult.OK;
    }
}
