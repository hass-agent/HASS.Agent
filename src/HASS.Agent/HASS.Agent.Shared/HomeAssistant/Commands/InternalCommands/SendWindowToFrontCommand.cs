using System;
using System.Diagnostics;
using System.IO;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    /// <summary>
    /// Command to perform an action through the configured custom executor
    /// </summary>
    public class SendWindowToFrontCommand : InternalCommand
    {
        private const string DefaultName = "sendwindowtofront";

        public SendWindowToFrontCommand(string entityName = DefaultName, string name = DefaultName, string process = "", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, process, entityType, id)
        {
            CommandConfig = process;
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(CommandConfig))
            {
                Log.Warning("[SENDWINDOWTOFRONT] [{name}] Unable to launch command, it's configured as action-only", EntityName);

                State = "OFF";
                return;
            }

            ProcessManager.BringMainWindowToFront(CommandConfig);

            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(action))
            {
                Log.Warning("[SENDWINDOWTOFRONT] [{name}] Unable to launch command, empty action provided", EntityName);

                State = "OFF";
                return;
            }

            if (!string.IsNullOrWhiteSpace(CommandConfig))
            {
                Log.Warning("[SENDWINDOWTOFRONT] [{name}] Command launched by action, command-provided process will be ignored", EntityName);
            }

            ProcessManager.BringMainWindowToFront(action);

            State = "OFF";
        }
    }
}
