using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to put Windows in hibernation
    /// </summary>
    public class HibernateCommand : CustomCommand
    {
        private const string DefaultName = "hibernate";

        public HibernateCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /h", false, entityName ?? DefaultName, name ?? null, entityType, id) => State = "OFF";
    }
}
