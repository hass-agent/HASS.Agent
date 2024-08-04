using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using Serilog;
using Vanara.PInvoke;
using static Vanara.PInvoke.PowrProf;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    /// <summary>
    /// Command to put all monitors to sleep
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MonitorSleepPowerPlanCommand : InternalCommand
    {
        private static readonly HKEY s_key = new HKEY();

        private const string DefaultName = "monitorsleep-pp";

        public MonitorSleepPowerPlanCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(entityName ?? DefaultName, name ?? null, string.Empty, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            try
            {
                var win32Error = PowerGetActiveScheme(out var activeScheme);
                if (win32Error != Win32Error.ERROR_SUCCESS)
                    throw new Exception("cannot get active power scheme");

                var schemes = PowerEnumerate<Guid>(null, null);
                var removalError = false;
                foreach (var scheme in schemes)
                {
                    var name = PowerReadFriendlyName(scheme);
                    if (name.EndsWith(" - HASS.Agent Monitor Sleep"))
                    {
                        win32Error = PowerDeleteScheme(s_key, scheme);
                        if (win32Error != Win32Error.ERROR_SUCCESS)
                            removalError = true;
                    }
                }

                if (removalError)
                    Log.Warning("[MONITORSLEEP-PP] [{name}] Error occurred while trying to remove HASS.Agent temp power plan/s", EntityName, EntityName);


                win32Error = PowerDuplicateScheme(s_key, activeScheme, out var duplicateScheme);
                if (win32Error != Win32Error.ERROR_SUCCESS)
                    throw new Exception("cannot duplicate current power scheme");

                var duplicatedSchemeGuid = duplicateScheme.ToStructure<Guid>();
                var newDuplicatedName = PowerReadFriendlyName(activeScheme) + " - HASS.Agent Monitor Sleep";
                win32Error = PowerWriteFriendlyName(duplicatedSchemeGuid, null, null, newDuplicatedName);
                if (win32Error != Win32Error.ERROR_SUCCESS)
                    throw new Exception("error setting new name to the duplicated power plan");

                win32Error = PowerWriteACValueIndex(s_key, duplicatedSchemeGuid, GUID_VIDEO_SUBGROUP, GUID_VIDEO_POWERDOWN_TIMEOUT, 1);
                if (win32Error != Win32Error.ERROR_SUCCESS)
                    throw new Exception("error changing the power plan timeout value");

                win32Error = PowerSetActiveScheme(s_key, duplicatedSchemeGuid);
                if (win32Error != Win32Error.ERROR_SUCCESS)
                    throw new Exception("error activating the temporary power plan");

                Thread.Sleep(1500); //TODO(Amadeo): ugly...

                win32Error = PowerSetActiveScheme(s_key, activeScheme);
                if (win32Error != Win32Error.ERROR_SUCCESS)
                    throw new Exception("error activating the original power plan");

                win32Error = PowerDeleteScheme(s_key, duplicatedSchemeGuid);
                if (win32Error != Win32Error.ERROR_SUCCESS)
                    throw new Exception("error removing the duplicated power plan");
            }
            catch (Exception ex)
            {
                Log.Error("[MONITORSLEEP-PP] [{name}] Error activating the command: {msg}", EntityName, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }
    }
}
