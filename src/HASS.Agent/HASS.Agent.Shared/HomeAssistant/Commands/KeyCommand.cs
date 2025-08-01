using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;
using static HASS.Agent.Shared.Functions.Inputs;
using static HASS.Agent.Shared.Functions.NativeMethods;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Command to simulate a keypress
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class KeyCommand : AbstractCommand
    {
        private const string DefaultName = "key";

        public string State { get; protected set; }
        
        public VirtualKeyShort KeyCode { get; set; }

        public KeyCommand(VirtualKeyShort keyCode, string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, entityType, id)
        {
            KeyCode = keyCode;
            State = "OFF";
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return new CommandDiscoveryConfigModel
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Command_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/set",
                Action_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/action",
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Device = deviceConfig
            };
        }

        public override string GetState() => State;

        public override void TurnOff()
        {
            //
        }

        public override void TurnOn()
        {
            State = "ON";

            var inputs = new INPUT[2];
            inputs[0].type = InputType.INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = KeyCode;

            inputs[1].type = InputType.INPUT_KEYBOARD;
            inputs[1].U.ki.wVk = KeyCode;
            inputs[1].U.ki.dwFlags = KEYEVENTF.KEYUP;

            var ret = SendInput((uint)inputs.Length, inputs, INPUT.Size);
            if (ret != inputs.Length)
            {
                var error = Marshal.GetLastWin32Error();
                Log.Error($"[{DefaultName}] Error simulating key press for {KeyCode}: {error}");
            }

            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            //
        }
    }
}
