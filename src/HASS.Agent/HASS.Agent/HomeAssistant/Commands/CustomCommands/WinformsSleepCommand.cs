using System;
using System.Diagnostics;
using System.IO;
using HASS.Agent.Shared.Enums;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands;

/// <summary>
/// Activates provided Virtual Desktop
/// </summary>
public class WinformsSleepCommand : InternalCommand
{
    private const string DefaultName = "switchdesktop";

    public WinformsSleepCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(entityName ?? DefaultName, name ?? null, string.Empty, entityType, id)
    {
        State = "OFF";
    }

    public override void TurnOn()
    {
        State = "ON";

        Application.SetSuspendState(PowerState.Suspend, false, false);

        State = "OFF";
    }
}
