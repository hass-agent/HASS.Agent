using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreAudio;
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
                var audioDevice = AudioManager.GetDefaultDevice(DataFlow.Render, Role.Multimedia);

                var volume = Convert.ToInt32(Math.Round(audioDevice.AudioEndpointVolume?.MasterVolumeLevelScalar * 100 ?? 0, 0));

                // Log.Debug("[MEDIA] Current volume: {vol}", volume);

                return volume;
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
                var audioDevice = AudioManager.GetDefaultDevice(DataFlow.Render, Role.Multimedia);

                // Log.Debug("[MEDIA] Muted: {mute}", muted);

                return audioDevice.AudioEndpointVolume?.Mute ?? false;
            }
            catch (Exception ex)
            {
                Log.Error("[MEDIA] Unable to get the mute state: {err}", ex.Message);
                return false;
            }
        }
    }
}
