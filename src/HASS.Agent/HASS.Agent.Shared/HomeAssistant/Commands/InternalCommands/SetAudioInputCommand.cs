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

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands;

public class SetAudioInputCommand : InternalCommand
{
    private const string DefaultName = "setaudioinput";

    private string InputDevice { get => CommandConfig; }

    public SetAudioInputCommand(string entityName = DefaultName, string name = DefaultName, string audioDevice = "", CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(entityName ?? DefaultName, name ?? null, audioDevice, entityType, id)
    {
        State = "OFF";
    }

    public override void TurnOn()
    {
        if (string.IsNullOrWhiteSpace(InputDevice))
        {
            Log.Error("[SETAUDIOIN] Error, input device name cannot be null/blank");

            return;
        }

        TurnOnWithAction(InputDevice);
    }

    private CoreAudioDevice GetAudioDeviceOrDefault(string playbackDeviceName)
    {
        var devices = Variables.AudioDeviceController.GetDevices(DeviceType.Capture, DeviceState.Active);
        var playbackDevice = devices.Where(d => d.FullName == playbackDeviceName).FirstOrDefault();

        return playbackDevice ?? Variables.AudioDeviceController.GetDefaultDevice(DeviceType.Capture, Role.Communications);
    }

    public override void TurnOnWithAction(string action)
    {
        State = "ON";

        try
        {
            var inputDevice = GetAudioDeviceOrDefault(action);
            if (inputDevice == Variables.AudioDeviceController.GetDefaultDevice(DeviceType.Capture, Role.Communications))
                return;

            inputDevice.SetAsDefault();
            inputDevice.SetAsDefaultCommunications();
        }
        catch (Exception ex)
        {
            Log.Error("[SETAUDIOIN] Error while processing action '{action}': {err}", action, ex.Message);
        }
        finally
        {
            State = "OFF";
        }
    }
}
