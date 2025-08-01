using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to log off the current Windows session
    /// </summary>
    public class LogOffCommand : CustomCommand
    {
        private const string DefaultName = "logoff";

        public LogOffCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /l", false, entityName ?? DefaultName, name ?? null, entityType, id) => State = "OFF";
    }
}
