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
    public class MonitorSleepCommand : InternalCommand
    {
        private static readonly HKEY s_key = new HKEY();

        private const string DefaultName = "monitorsleep";

        public MonitorSleepCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(entityName ?? DefaultName, name ?? null, string.Empty, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            //NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, NativeMethods.WM_SYSCOMMAND, (IntPtr)NativeMethods.SC_MONITORPOWER, (IntPtr)2);

            PowerGetActiveScheme(out var activeScheme);

            var schemes = PowerEnumerate<Guid>(null, null);
            foreach (var scheme in schemes)
            {
                var name = PowerReadFriendlyName(scheme);
                if (name.EndsWith(" - HASS.Agent Monitor Sleep"))
                    PowerDeleteScheme(s_key, scheme);
            }

            PowerDuplicateScheme(s_key, activeScheme, out var duplicateScheme);
            var duplicatedSchemeGuid = duplicateScheme.ToStructure<Guid>();
            var newDuplicatedName = PowerReadFriendlyName(activeScheme) + " - HASS.Agent Monitor Sleep";
            PowerWriteFriendlyName(duplicatedSchemeGuid, null, null, newDuplicatedName);

            PowerWriteACValueIndex(s_key, duplicatedSchemeGuid, GUID_VIDEO_SUBGROUP, GUID_VIDEO_POWERDOWN_TIMEOUT, 1);
            PowerSetActiveScheme(s_key, duplicatedSchemeGuid);

            Thread.Sleep(1500);

            PowerSetActiveScheme(s_key, activeScheme);
            PowerDeleteScheme(s_key, duplicatedSchemeGuid);

            State = "OFF";
        }
    }
}
