using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'playpause' mediakey press
    /// </summary>
    public class MediaPlayPauseCommand : KeyCommand
    {
        private const string DefaultName = "playpause";

        public MediaPlayPauseCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VirtualKeyShort.MEDIA_PLAY_PAUSE, entityName ?? DefaultName, name ?? null, entityType, id) { }
    }
}
