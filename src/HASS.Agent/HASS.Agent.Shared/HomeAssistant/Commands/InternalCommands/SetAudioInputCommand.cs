using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Managers.Audio;
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

    public override void TurnOnWithAction(string action)
    {
        State = "ON";

        try
        {
/*            var audioDevices = AudioManager.GetDevices();
            var inputDevice = audioDevices
                .Where(d => d.Type == DeviceType.Input)
                .Where(d => d.FriendlyName == action)
                .FirstOrDefault();

            if (inputDevice == null)
            {
                Log.Warning("[SETAUDIOIN] No input device {device} found", action);
                return;
            }

            if(inputDevice.Default)
                return;

            AudioManager.Activate(inputDevice);*/

            AudioManager.ActivateDevice(action);
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
