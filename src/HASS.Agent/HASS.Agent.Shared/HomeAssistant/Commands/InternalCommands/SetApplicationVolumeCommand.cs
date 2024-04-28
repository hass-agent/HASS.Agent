using CoreAudio;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
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
        private static readonly Dictionary<int, string> ApplicationNames = new Dictionary<int, string>();

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

        private MMDevice GetAudioDeviceOrDefault(string playbackDeviceName)
        {
            var playbackDevice = AudioManager.GetPlaybackDevices().Where(d => d.DeviceFriendlyName == playbackDeviceName).FirstOrDefault();

            return playbackDevice ?? AudioManager.GetDefaultDevice(DataFlow.Render, Role.Multimedia);
        }

        private string GetSessionDisplayName(AudioSessionControl2 session)
        {
            if (string.IsNullOrWhiteSpace(session.DisplayName))
                return session.DisplayName;

            var procId = (int)session.ProcessID;

            if (procId <= 0)
                return session.DisplayName;

            if (ApplicationNames.ContainsKey(procId))
                return ApplicationNames[procId];

            using var p = Process.GetProcessById(procId);
            ApplicationNames.Add(procId, p.ProcessName);

            return p.ProcessName;
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

                using var audioDevice = GetAudioDeviceOrDefault(actionData.PlaybackDevice);
                using var sessionManager = audioDevice.AudioSessionManager2;
                if (sessionManager == null)
                    return;

                var sessions = audioDevice.AudioSessionManager2?.Sessions?.Where(s =>
                    s != null &&
                    actionData.ApplicationName == GetSessionDisplayName(s)
                );

                foreach (var session in sessions)
                {
                    session.SimpleAudioVolume.Mute = actionData.Mute;
                    if (actionData.Volume == -1)
                    {
                        Log.Debug("[SETAPPVOLUME] No volume value provided, only mute has been set for {app}", actionData.ApplicationName);

                        return;
                    }

                    var volume = Math.Clamp(actionData.Volume, 0, 100) / 100.0f;
                    session.SimpleAudioVolume.MasterVolume = volume;
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
        }
    }
}
