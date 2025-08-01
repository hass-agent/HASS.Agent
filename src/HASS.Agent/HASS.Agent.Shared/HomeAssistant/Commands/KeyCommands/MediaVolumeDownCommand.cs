using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'volume down' mediakey press
    /// </summary>
    public class MediaVolumeDownCommand : KeyCommand
    {
        private const string DefaultName = "volumedown";

        public MediaVolumeDownCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VirtualKeyShort.VOLUME_DOWN, entityName ?? DefaultName, name ?? null, entityType, id) { }
    }
}
