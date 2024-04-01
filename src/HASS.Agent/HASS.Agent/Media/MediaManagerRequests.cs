using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using HASS.Agent.Shared.Managers;
using Serilog;

namespace HASS.Agent.Media
{
    internal static class MediaManagerRequests
    {
        /// <summary>
        /// Gets the volume for the default audio device
        /// </summary>
        /// <returns></returns>
        internal static int GetVolume()
        {
            try
            {
                // get the default audio device
                var audioDevice = AudioManager.GetDefaultDevice(DeviceType.Playback, Role.Multimedia);

                // Log.Debug("[MEDIA] Current volume: {vol}", volume);

                // return it
                return (int)audioDevice.Volume;
            }
            catch (Exception ex)
            {
                Log.Error("[MEDIA] Unable to get the volume: {err}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Gets the muted state for the default audio device
        /// </summary>
        /// <returns></returns>
        internal static bool GetMuteState()
        {
            try
            {
                // get the default audio device
                var audioDevice = AudioManager.GetDefaultDevice(DeviceType.Playback, Role.Multimedia);

                // Log.Debug("[MEDIA] Muted: {mute}", muted);

                // return it
                return audioDevice.IsMuted;
            }
            catch (Exception ex)
            {
                Log.Error("[MEDIA] Unable to get the mute state: {err}", ex.Message);
                return false;
            }
        }
    }
}
