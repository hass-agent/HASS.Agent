using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'volume up' mediakey press
    /// </summary>
    public class MediaVolumeUpCommand : KeyCommand
    {
        private const string DefaultName = "volumeup";

        public MediaVolumeUpCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VirtualKeyShort.VOLUME_UP, entityName ?? DefaultName, name ?? null, entityType, id) { }
    }
}
