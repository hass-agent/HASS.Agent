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

        private AudioDevice GetDeviceOrDefault(string deviceName)
        {
            var device = AudioManager.GetDevices().Where(d => d.FriendlyName == deviceName).FirstOrDefault();
            if (device != null)
                return device;

            var defaultDeviceId = AudioManager.GetDefaultDeviceId(DeviceType.Output, DeviceRole.Multimedia | DeviceRole.Console);
            return AudioManager.GetDevices().Where(d => d.Id == defaultDeviceId).FirstOrDefault();
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

                var audioDevice = GetDeviceOrDefault(actionData.PlaybackDevice);
                if (audioDevice == null)
                    return;

                var applicationAudioSessions = audioDevice.Sessions.Where(s =>
                    s.Application == actionData.ApplicationName
                );

                if (actionData.Volume == -1)
                    Log.Debug("[SETAPPVOLUME] No volume value provided, only mute has been set for {app}", actionData.ApplicationName);


                if (string.IsNullOrWhiteSpace(actionData.SessionId))
                {
                    foreach (var session in applicationAudioSessions)
                    {
                        AudioManager.SetMute(session, actionData.Mute);
                        if (actionData.Volume == -1)
                            return;

                        AudioManager.SetVolume(session, actionData.Volume);
                    }
                }
                else
                {
                    var session = applicationAudioSessions.Where(s => s.Id == actionData.SessionId).FirstOrDefault();
                    if (session == null)
                    {
                        Log.Debug("[SETAPPVOLUME] No session {actionData.SessionId} found for device {device}", actionData.ApplicationName, audioDevice.FriendlyName);
                        return;
                    }

                    AudioManager.SetMute(session, actionData.Mute);
                    if (actionData.Volume == -1)
                        return;

                    AudioManager.SetVolume(session, actionData.Volume);
                }
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
