﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Managers.Audio;
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
                var defaultDeviceId = AudioManager.GetDefaultDeviceId(DeviceType.Output, DeviceRole.Multimedia);
                var audioDevice = AudioManager.GetDevices().Where(d => d.Id == defaultDeviceId).FirstOrDefault();
                if (audioDevice == null)
                    return 0;

                return audioDevice.Volume;
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
                var defaultDeviceId = AudioManager.GetDefaultDeviceId(DeviceType.Output, DeviceRole.Multimedia);
                var audioDevice = AudioManager.GetDevices().Where(d => d.Id == defaultDeviceId).FirstOrDefault();
                if (audioDevice == null)
                    return false;

                return audioDevice.Muted;
            }
            catch (Exception ex)
            {
                Log.Error("[MEDIA] Unable to get the mute state: {err}", ex.Message);
                return false;
            }
        }
    }
}
