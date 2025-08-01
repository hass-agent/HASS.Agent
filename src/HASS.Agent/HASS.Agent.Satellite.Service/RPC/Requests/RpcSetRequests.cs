using Grpc.Core;
using HASS.Agent.Satellite.Service.Commands;
using HASS.Agent.Satellite.Service.Extensions;
using HASS.Agent.Satellite.Service.Functions;
using HASS.Agent.Satellite.Service.Sensors;
using HASS.Agent.Satellite.Service.Settings;
using Serilog;

namespace HASS.Agent.Satellite.Service.RPC
{
    public partial class HassAgentSatelliteRpcCallsService
    {
        /// <summary>
        /// Changes the registered device name in HA
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<AckResponse> SetDeviceName(SetDeviceNameRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnError("invalid auth id", peer));

            // check name value
            if (string.IsNullOrWhiteSpace(request.Devicename)) return Task.FromResult(RpcHelper.ReturnError("no devicename provided", peer));

            // execute device name change
            Task.Run(() => SettingsManager.ProcessNameChange(request.Devicename));

            // done
            return Task.FromResult(RpcHelper.ReturnOk());
        }

        /// <summary>
        /// Stores and applies the provided settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<AckResponse> SetServiceSettings(SetServiceSettingsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnError("invalid auth id", peer));

            // process the settings
            Task.Run(() => SettingsManager.ProcessReceivedServiceSettings(request.ServiceSettings.ConvertToServiceSettings()));

            // done
            return Task.FromResult(RpcHelper.ReturnOk());
        }

        /// <summary>
        /// Stores and applies the provided MQTT settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<AckResponse> SetServiceMqttSettings(SetServiceMqttSettingsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnError("invalid auth id", peer));

            // process the settings
            Task.Run(() => SettingsManager.ProcessReceivedServiceMqttSettings(request.ServiceMqttSettings.ConvertToServiceMqttSettings()));

            // done
            return Task.FromResult(RpcHelper.ReturnOk());
        }

        /// <summary>
        /// Stores and activates the provided commands
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<AckResponse> SetConfiguredCommands(SetConfiguredCommandsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnError("invalid auth id", peer));

            // process the settings
            Task.Run(() => CommandsManager.ProcessReceivedCommands(request.ConfiguredServerCommands.ConvertToConfiguredCommands()));

            // done
            return Task.FromResult(RpcHelper.ReturnOk());
        }

        /// <summary>
        /// Stores and activates the provided sensors
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<AckResponse> SetConfiguredSensors(SetConfiguredSensorsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnError("invalid auth id", peer));

            // process the settings
            Task.Run(() => SensorsManager.ProcessReceivedSensors(request.ConfiguredServerSensors.ConvertToConfiguredSensors()));

            // done
            return Task.FromResult(RpcHelper.ReturnOk());
        }
    }
}
