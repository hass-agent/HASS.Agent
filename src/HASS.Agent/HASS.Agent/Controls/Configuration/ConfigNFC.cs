using System.Globalization;
using HASS.Agent.Functions;
using HASS.Agent.Managers;
using HASS.Agent.Resources.Localization;
using Syncfusion.Windows.Forms;

namespace HASS.Agent.Controls.Configuration;

public partial class ConfigNFC : UserControl
{
    public ConfigNFC()
    {
        InitializeComponent();

        BindComboBoxTheme();
    }

    private void BindComboBoxTheme() => CbNfcScanner.DrawItem += ComboBoxTheme.DrawItem;

    private void ConfigNFC_Load(object sender, EventArgs e)
    {
        CbNfcScanner.Items.Add(Languages.SensorsMod_None);

        foreach (var nfcReaderName in RadioManager.AvailableNFCReaderNames)
            CbNfcScanner.Items.Add(nfcReaderName);

        CbNfcScanner.SelectedItem = string.IsNullOrWhiteSpace(Variables.AppSettings.NfcSelectedScanner) ? Languages.SensorsMod_None : Variables.AppSettings.NfcSelectedScanner;
    }
}
