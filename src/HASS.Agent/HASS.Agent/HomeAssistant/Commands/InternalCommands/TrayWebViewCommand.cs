using HASS.Agent.Functions;
using HASS.Agent.Models.Internal;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant.Commands;
using Newtonsoft.Json;
using Serilog;
using Syncfusion.Windows.Forms;

namespace HASS.Agent.HomeAssistant.Commands.InternalCommands
{
    internal class TrayWebViewCommand : InternalCommand
    {
        private const string DefaultName = "traywebview";

        internal TrayWebViewCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, string.Empty, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            if (string.IsNullOrEmpty(Variables.AppSettings.TrayIconWebViewUrl))
            {
                Log.Warning("[TRAYWEBVIEW] [{name}] No webview URL is configured in 'Configuration -> Tray Icon'", EntityName);
                return;
            }

            HelperFunctions.LaunchTrayIconWebView();
        }

        public override void TurnOff()
        {
            Variables.MainForm.Invoke(() =>
            {
                if(Variables.TrayIconWebView == null)
                    return;

                Variables.TrayIconWebView.Opacity = 0;

                if (!Variables.AppSettings.TrayIconWebViewBackgroundLoading) { 
                    Variables.TrayIconWebView.ForceClose();
                    Variables.TrayIconWebView.Dispose();
                }
            });
        }

        public override string GetState()
        {
            return Variables.TrayIconWebView != null && Variables.TrayIconWebView.Opacity != 0 ? "ON" : "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            Log.Warning("[TRAYWEBVIEW] [{name}] Launching with action is not supported", EntityName);
        }
    }
}
