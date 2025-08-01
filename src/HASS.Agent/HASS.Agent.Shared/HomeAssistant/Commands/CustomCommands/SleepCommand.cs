using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to put Windows in sleep
    /// <para>Note: this only works when hibernation is disabled</para>
    /// </summary>
    public class SleepCommand : CustomCommand
    {
        private const string DefaultName = "sleep";

        public SleepCommand(string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("Rundll32.exe powrprof.dll,SetSuspendState 0,1,0", false, entityName ?? DefaultName, name ?? null, entityType, id) => State = "OFF";
    }
}
