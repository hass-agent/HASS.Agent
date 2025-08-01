using System;
using System.Diagnostics;
using System.IO;
using HASS.Agent.Managers;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using Serilog;
using WindowsDesktop;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands;

/// <summary>
/// Activates provided Virtual Desktop
/// </summary>
public class SwitchDesktopCommand : InternalCommand
{
    private const string DefaultName = "switchdesktop";

    public SwitchDesktopCommand(string entityName = DefaultName, string name = DefaultName, string desktopId = "", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, desktopId, entityType, id)
    {
        CommandConfig = desktopId;
        State = "OFF";
    }

    public override void TurnOn()
    {
        State = "ON";

        if (string.IsNullOrWhiteSpace(CommandConfig))
        {
            Log.Warning("[SWITCHDESKTOP] [{name}] Unable to launch command, it's configured as action-only", EntityName);

            State = "OFF";
            return;
        }

        ActivateVirtualDesktop(CommandConfig);

        State = "OFF";
    }

    public override void TurnOnWithAction(string action)
    {
        State = "ON";

        if (string.IsNullOrWhiteSpace(action))
        {
            Log.Warning("[SWITCHDESKTOP] [{name}] Unable to launch command, empty action provided", EntityName);

            State = "OFF";
            return;
        }

        if (!string.IsNullOrWhiteSpace(CommandConfig))
        {
            Log.Warning("[SWITCHDESKTOP] [{name}] Command launched by action, command-provided process will be ignored", EntityName);

            State = "OFF";
            return;
        }

        ActivateVirtualDesktop(action);

        State = "OFF";
    }

    private void ActivateVirtualDesktop(string virtualDesktopId)
    {
        VirtualDesktopManager.ActivateDesktop(virtualDesktopId);
    }
}
