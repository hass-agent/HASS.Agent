using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using HASS.Agent.Satellite.Service.Commands;
using HASS.Agent.Satellite.Service.Sensors;
using HASS.Agent.Shared.Managers.Audio;
using HASS.Agent.Shared.Managers.VoiceMeeterAudio;
using Serilog;

namespace HASS.Agent.Satellite.Service.Functions
{
    internal static class HelperFunctions
    {
        private static bool _shutdownCalled = false;

        /// <summary>
        /// Returns the configured device name, or a safe version of the machinename if nothing's stored
        /// </summary>
        /// <returns></returns>
        internal static string? GetConfiguredDeviceName() =>
            string.IsNullOrEmpty(Variables.ServiceSettings?.DeviceName)
                ? GetSafeDeviceName()
                : Variables.ServiceSettings.DeviceName;

        /// <summary>
        /// Returns a safe version of this machine's name
        /// </summary>
        /// <returns></returns>
        internal static string? GetSafeDeviceName()
        {
            var val = Regex.Replace($"{Environment.MachineName}-satellite", @"[^a-zA-Z0-9_\-_\s]", "_");
            return val.Replace(" ", "");
        }

        /// <summary>
        /// Try to properly close the service with default 500ms waiting time
        /// </summary>
        /// <returns></returns>
        internal static async Task ShutdownAsync()
        {
            await ShutdownAsync(TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Try to properly close the service
        /// </summary>
        internal static async Task ShutdownAsync(TimeSpan waitBeforeClosing)
        {
            try
            {
                // process only once
                if (_shutdownCalled) return;
                _shutdownCalled = true;

                // announce we're stopping
                Variables.ShuttingDown = true;

                // wait a bit
                await Task.Delay(waitBeforeClosing);

                // log our demise
                Log.Information("[SYSTEM] Service shutting down");

                //stop audio manager
                AudioManager.Shutdown();

                //stop voicemeeter audio manager
                VoiceMeeterAudioManager.Shutdown();

                // stop mqtt
                await Variables.MqttManager.AnnounceAvailabilityAsync(true);
                Variables.MqttManager.Disconnect();

                // stop rpc
                Variables.RpcServer?.Dispose();

                // stop entity managers
                CommandsManager.Stop();
                SensorsManager.Stop();

                // tidy up
                Application.Exit();

                // done
                Log.Information("[SYSTEM] Service shutdown complete");

                // notify the worker
                Variables.CommenceShutdown = true;
            }
            catch (Exception ex)
            {
                Log.Error("[SYSTEM] Error shutting down nicely: {msg}", ex.Message);
            }
        }

        /// <summary>
        /// Returns the name of the current method
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        internal static string CurrentMethodName([CallerMemberName] string? caller = null) => caller ?? "-";
    }

    public class CamelCaseJsonNamingpolicy : JsonNamingPolicy
    {
        /// <summary>
        /// Convert name to lowerinvariant
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string ConvertName(string name) => name.ToLowerInvariant();
    }
}
