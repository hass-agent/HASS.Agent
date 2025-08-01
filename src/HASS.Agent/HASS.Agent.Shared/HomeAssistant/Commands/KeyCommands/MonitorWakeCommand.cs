using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'arrow up' key press to wake the monitors
    /// https://stackoverflow.com/a/42393472 ?
    /// </summary>
    public class MonitorWakeCommand : KeyCommand
    {
        private const string DefaultName = "monitorwake";

        public MonitorWakeCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(VirtualKeyShort.UP, entityName ?? DefaultName, name ?? null, entityType, id) { }
    }
}
