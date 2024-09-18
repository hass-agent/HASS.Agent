using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Resources.Localization;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MultipleKeysCommand : AbstractCommand
    {
        private const string DefaultName = "multiplekeys";

        public string State { get; protected set; }
        public List<string> Keys { get; set; }

        public MultipleKeysCommand(List<string> keys, string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, entityType, id)
        {
            Keys = keys;
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

        public async override void TurnOn()
        {
            try
            {
                State = "ON";

                foreach (var key in Keys)
                {
                    SendKeys.SendWait(key);
                    SendKeys.Flush();
                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[MULTIPLEKEYS] [{name}] Executing command failed: {ex}", EntityName, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }

        public async override void TurnOnWithAction(string action)
        {
            var keys = ParseMultipleKeys(action);
            if (keys.Count == 0)
                return;

            foreach (var key in keys)
            {
                SendKeys.SendWait(key);
                SendKeys.Flush();
                await Task.Delay(50);
            }
        }

        private List<string> ParseMultipleKeys(string keyString)
        {
            var keys = new List<string>();

            try
            {
                if (string.IsNullOrWhiteSpace(keyString))
                    return keys;


                if (!keyString.Contains('[') || !keyString.Contains(']'))
                    return keys;

                // replace all escaped brackets
                // todo: ugly, let regex do that
                keyString = keyString.Replace(@"\[", "left_bracket");
                keyString = keyString.Replace(@"\]", "right_bracket");

                // lets see if the brackets corresponds
                var leftBrackets = keyString.Count(x => x == '[');
                var rightBrackets = keyString.Count(x => x == ']');

                if (leftBrackets != rightBrackets)
                    return keys;

                // ok, try parsen
                var pattern = @"\[(.*?)\]";
                var matches = Regex.Matches(keyString, pattern);
                keys.AddRange(from Match m in matches select m.Groups[1].ToString());

                // restore escaped brackets
                for (var i = 0; i < keys.Count; i++)
                {
                    if (keys[i] == "left_bracket") keys[i] = "[";
                    if (keys[i] == "right_bracket") keys[i] = "]";
                }
            }
            catch (Exception ex)
            {
                Log.Error("[MULTIPLEKEYS] [{name}] Error parsing multiple keys: {msg}", EntityName, ex.Message);
            }

            return keys;
        }
    }
}
