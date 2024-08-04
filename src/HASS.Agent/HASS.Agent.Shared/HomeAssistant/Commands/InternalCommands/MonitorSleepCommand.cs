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
        private const string DefaultName = "monitorsleep";

        public MonitorSleepCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(entityName ?? DefaultName, name ?? null, string.Empty, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, NativeMethods.WM_SYSCOMMAND, (IntPtr)NativeMethods.SC_MONITORPOWER, (IntPtr)2);

            State = "OFF";
        }
    }
}
