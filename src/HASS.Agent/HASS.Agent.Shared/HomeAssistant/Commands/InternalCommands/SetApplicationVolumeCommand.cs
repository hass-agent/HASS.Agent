using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Managers.Audio;
using HidSharp;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    public class SetApplicationVolumeCommand : InternalCommand
    {
        private const string DefaultName = "setappvolume";

        public SetApplicationVolumeCommand(string entityName = DefaultName, string name = DefaultName, string commandConfig = "", CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(entityName ?? DefaultName, name ?? null, commandConfig, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            if (string.IsNullOrWhiteSpace(CommandConfig))
            {
                Log.Error("[SETAPPVOLUME] Error, command config is null/empty/blank");

                return;
            }


            TurnOnWithAction(CommandConfig);
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            try
            {
                var actionData = JsonConvert.DeserializeObject<ApplicationVolumeAction>(action);

                if (string.IsNullOrWhiteSpace(actionData.ApplicationName))
                {
                    Log.Error("[SETAPPVOLUME] Error, this command can be run only with action");

                    return;
                }

                AudioManager.SetApplicationProperties(actionData.PlaybackDevice, actionData.ApplicationName, actionData.SessionId, actionData.Volume, actionData.Mute);
            }
            catch (Exception ex)
            {
                Log.Error("[SETAPPVOLUME] Error while processing action '{action}': {err}", action, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }

        private class ApplicationVolumeAction
        {
            public int Volume { get; set; } = -1;
            public bool Mute { get; set; } = false;
            public string ApplicationName { get; set; } = string.Empty;
            public string PlaybackDevice { get; set; } = string.Empty;
            public string SessionId { get; set; } = string.Empty;
        }
    }
}
