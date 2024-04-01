using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using HASS.Agent.Shared.Enums;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    public class SetAudioOutputCommand : InternalCommand
    {
        private const string DefaultName = "setaudiooutput";

        private string OutputDevice { get => CommandConfig; }

        public SetAudioOutputCommand(string entityName = DefaultName, string name = DefaultName, string audioDevice = "", CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(entityName ?? DefaultName, name ?? null, audioDevice, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            if (string.IsNullOrWhiteSpace(OutputDevice))
            {
                Log.Error("[SETAUDIOOUT] Error, output device name cannot be null/blank");

                return;
            }

            TurnOnWithAction(OutputDevice);
        }

        private CoreAudioDevice GetAudioDeviceOrDefault(string playbackDeviceName)
        {
            var devices = Variables.AudioDeviceController.GetDevices(DeviceType.Playback, DeviceState.Active);
            var playbackDevice = devices.Where(d => d.FullName == playbackDeviceName).FirstOrDefault();

            return playbackDevice ?? Variables.AudioDeviceController.GetDefaultDevice(DeviceType.Playback, Role.Multimedia);
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";
 
            try
            {
                var outputDevice = GetAudioDeviceOrDefault(action);
                if (outputDevice == Variables.AudioDeviceController.GetDefaultDevice(DeviceType.Playback, Role.Multimedia))
                    return;

                outputDevice.SetAsDefault();
            }
            catch (Exception ex)
            {
                Log.Error("[SETAUDIOOUT] Error while processing action '{action}': {err}", action, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }
    }
}
