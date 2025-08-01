using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to lock the current Windows session
    /// </summary>
    public class LockCommand : CustomCommand
    {
        private const string DefaultName = "lock";

        public LockCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("Rundll32.exe user32.dll,LockWorkStation", false, entityName ?? DefaultName, name ?? null, entityType, id) => State = "OFF";
    }
}
