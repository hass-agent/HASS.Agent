using HASS.Agent.Managers;
using HASS.Agent.Sensors;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant.Commands;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;
using Windows.Devices.Radios;

namespace HASS.Agent.HomeAssistant.Commands.InternalCommands
{
    internal class RadioCommand : InternalCommand
    {
        private const string DefaultName = "radiocommand";

        private Radio _radio;

        public string RadioName { get; set; }

        internal RadioCommand(string radioName, string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, radioName, entityType, id)
        {
            RadioName = radioName;
            _radio = RadioManager.AvailableRadio.FirstOrDefault(r => r.Name == radioName);

            if (_radio == null)
            {
                Log.Warning("[RADIOCOMMAND] [{name}] '{radioName}' not present, will retry to find on each command execution", EntityName, radioName);
            }
        }

        public override void TurnOn()
        {
            if (_radio == null)
            {
                _radio = RadioManager.AvailableRadio.FirstOrDefault(r => r.Name == RadioName);
            }

            if (_radio != null)
            {
                Task.Run(async () => { await _radio.SetStateAsync(RadioState.On); });
            }
        }

        public override void TurnOff()
        {
            if (_radio == null)
            {
                _radio = RadioManager.AvailableRadio.FirstOrDefault(r => r.Name == RadioName);
            }

            if (_radio != null)
            {
                Task.Run(async () => { await _radio.SetStateAsync(RadioState.Off); });
            }
        }

        public override string GetState()
        {
            return _radio != null ? (_radio.State == RadioState.On ? "ON" : "OFF") : "OFF";
        }
    }
}
