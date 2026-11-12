using HASS.Agent.Sensors;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant.Commands;
using Serilog;

namespace HASS.Agent.HomeAssistant.Commands.InternalCommands;

public class PublishSensorCommand : InternalCommand
{
    private const string DefaultName = "publishallsensor";

    internal PublishSensorCommand(string entityName = DefaultName, string name = DefaultName, string sensorName = "", CommandEntityType entityType = CommandEntityType.Switch,
        string id = default) : base(entityName ?? DefaultName, name ?? null, sensorName, entityType, id)
    {
        State = "OFF";
    }

    public override void TurnOn()
    {
        State = "ON";

        if (!string.IsNullOrWhiteSpace(CommandConfig))
        {
            SensorsManager.ResetSensorCheck(CommandConfig);
        }
        else
        {
            Log.Warning("[PUBLISHSENSOR] [{name}] Unable to launch command, it's configured as action-only", EntityName);
        }

        State = "OFF";
    }
    
    public override void TurnOnWithAction(string action)
    {
        State = "ON";

        if (!string.IsNullOrWhiteSpace(action))
        {
            SensorsManager.ResetSensorCheck(action);
        }
        else
        {
            Log.Warning("[PUBLISHSENSOR] [{name}] Unable to launch command, it's configured as action-only", EntityName);
        }

        State = "OFF";
    }
}