using Serilog;

namespace HASS.Agent.Satellite.Service.RPC
{
    /// <summary>
    /// Contains helper functions for the RPC server
    /// </summary>
    internal static class RpcHelper
    {
        /// <summary>
        /// Returns an devicenameresponse containing the provided error, logs the message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        internal static GetDeviceNameResponse ReturnDeviceNameError(string errorMessage, string peer)
        {
            Log.Warning("[RPC] [{method}] [{peer}] Request denied or failed: {err}", "DeviceName", peer, errorMessage);

            return new GetDeviceNameResponse
            {
                Ok = false,
                Error = errorMessage,
                DeviceName = string.Empty
            };
        }

        /// <summary>
        /// Returns an mqttstatusresponse containing the provided error, logs the message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        internal static GetMqttStatusResponse ReturnMqttStatusError(string errorMessage, string peer)
        {
            Log.Warning("[RPC] [{method}] [{peer}] Request denied or failed: {err}", "MqttStatus", peer, errorMessage);

            return new GetMqttStatusResponse
            {
                Ok = false,
                Error = errorMessage,
                MqttStatus = -1
            };
        }

        /// <summary>
        /// Returns an servicesettingsresponse containing the provided error, logs the message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        internal static GetServiceSettingsResponse ReturnServiceSettingsError(string errorMessage, string peer)
        {
            Log.Warning("[RPC] [{method}] [{peer}] Request denied or failed: {err}", "ServiceSettings", peer, errorMessage);

            return new GetServiceSettingsResponse
            {
                Ok = false,
                Error = errorMessage
            };
        }

        /// <summary>
        /// Returns an servicemqttsettingsresponse containing the provided error, logs the message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        internal static GetServiceMqttSettingsResponse ReturnServiceMqttSettingsError(string errorMessage, string peer)
        {
            Log.Warning("[RPC] [{method}] [{peer}] Request denied or failed: {err}", "ServiceMqttSettings", peer, errorMessage);

            return new GetServiceMqttSettingsResponse
            {
                Ok = false,
                Error = errorMessage
            };
        }

        /// <summary>
        /// Returns an configuredcommandsresponse containing the provided error, logs the message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        internal static GetConfiguredCommandsResponse ReturnConfiguredCommandsError(string errorMessage, string peer)
        {
            Log.Warning("[RPC] [{method}] [{peer}] Request denied or failed: {err}", "ConfiguredCommands", peer, errorMessage);

            return new GetConfiguredCommandsResponse
            {
                Ok = false,
                Error = errorMessage
            };
        }

        /// <summary>
        /// Returns an configuredsensorsresponse containing the provided error, logs the message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        internal static GetConfiguredSensorsResponse ReturnConfiguredSensorsError(string errorMessage, string peer)
        {
            Log.Warning("[RPC] [{method}] [{peer}] Request denied or failed: {err}", "ConfiguredSensors", peer, errorMessage);

            return new GetConfiguredSensorsResponse
            {
                Ok = false,
                Error = errorMessage
            };
        }

        /// <summary>
        /// Returns an allgood ping response
        /// </summary>
        /// <returns></returns>
        internal static PingResponse ReturnPingOk() =>
            new()
            {
                Ok = true,
                Version = Variables.Version,
                Error = string.Empty
            };

        /// <summary>
        /// Returns an ackresponse containing the provided error, logs the message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        internal static AckResponse ReturnError(string errorMessage, string peer)
        {
            Log.Warning("[RPC] [{method}] [{peer}] Request denied or failed: {err}", "General", peer, errorMessage);

            return new AckResponse
            {
                Ok = false,
                Error = errorMessage
            };
        }

        /// <summary>
        /// Returns an allgood ackresponse
        /// </summary>
        /// <returns></returns>
        internal static AckResponse ReturnOk() =>
            new()
            {
                Ok = true,
                Error = string.Empty
            };
    }
}
