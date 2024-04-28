using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Serilog;
using Google.Protobuf.WellKnownTypes;
using static HASS.Agent.Shared.Functions.Inputs;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Managers.Audio;

namespace HASS.Agent.Media
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class MediaManagerCommands
    {
        private const string LogTag = "mediamanager";
        internal static void KeyPress(VirtualKeyShort keyCode)
        {
            var inputs = new INPUT[2];
            inputs[0].type = InputType.INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = keyCode;
            inputs[0].U.ki.dwFlags = KEYEVENTF.EXTENDEDKEY;

            inputs[1].type = InputType.INPUT_KEYBOARD;
            inputs[1].U.ki.wVk = keyCode;
            inputs[1].U.ki.dwFlags = KEYEVENTF.KEYUP | KEYEVENTF.EXTENDEDKEY;

            var ret = NativeMethods.SendInput((uint)inputs.Length, inputs, INPUT.Size);
            if (ret != inputs.Length)
            {
                var error = Marshal.GetLastWin32Error();
                Log.Error($"[{LogTag}] Error simulating key press for {keyCode}: {error}");
            }
        }
        internal static void VolumeUp() => KeyPress(VirtualKeyShort.VOLUME_UP);

        internal static void VolumeDown() => KeyPress(VirtualKeyShort.VOLUME_DOWN);


        internal static void Mute() => KeyPress(VirtualKeyShort.VOLUME_MUTE);


        internal static void Play() => KeyPress(VirtualKeyShort.MEDIA_PLAY_PAUSE);


        internal static void Pause() => KeyPress(VirtualKeyShort.MEDIA_PLAY_PAUSE);


        internal static void Stop() => KeyPress(VirtualKeyShort.MEDIA_STOP);


        internal static void Next() => KeyPress(VirtualKeyShort.MEDIA_NEXT_TRACK);


        internal static void Previous() => KeyPress(VirtualKeyShort.MEDIA_PREV_TRACK);

        /// <summary>
        /// Sets the volume to the provided value (0-100)
        /// </summary>
        /// <param name="volume"></param>
        internal static void SetVolume(int volume)
        {
            try
            {
                if (volume < 0) volume = 0;
                if (volume > 100) volume = 100;

                var defaultDeviceId = AudioManager.GetDefaultDeviceId(DeviceType.Output, DeviceRole.Multimedia | DeviceRole.Console);
                var audioDevice = AudioManager.GetDevices().Where(d => d.Id == defaultDeviceId).FirstOrDefault();
                if (audioDevice == null)
                    return;

                AudioManager.SetVolume(audioDevice, volume);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MEDIA] Error while trying to set volume to {val}: {err}", volume, ex.Message);
            }
        }
    }
}
