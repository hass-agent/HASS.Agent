using Grpc.Core;
using HASS.Agent.Satellite.Service.Extensions;
using HASS.Agent.Satellite.Service.Functions;
using HASS.Agent.Satellite.Service.Settings;
using Serilog;

namespace HASS.Agent.Satellite.Service.RPC
{
    public partial class HassAgentSatelliteRpcCallsService
    {
        /// <summary>
        /// Returns the configured device name
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetDeviceNameResponse> GetDeviceName(GetDeviceNameRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnDeviceNameError("invalid auth id", peer));

            // prepare the response
            var response = new GetDeviceNameResponse
            {
                Ok = true,
                DeviceName = Variables.ServiceSettings?.DeviceName ?? "-"
            };

            // done
            return Task.FromResult(response);
        }

        /// <summary>
        /// Returns the state of the MQTT manager
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetMqttStatusResponse> GetMqttStatus(GetMqttStatusRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnMqttStatusError("invalid auth id", peer));

            // prepare the response
            var response = new GetMqttStatusResponse
            {
                Ok = true,
                MqttStatus = (int)Variables.MqttManager.GetStatus()
            };

            // done
            return Task.FromResult(response);
        }

        /// <summary>
        /// Returns the current service settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetServiceSettingsResponse> GetServiceSettings(GetServiceSettingsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnServiceSettingsError("invalid auth id", peer));

            // prepare the response
            var response = new GetServiceSettingsResponse
            {
                Ok = true,
                ServiceSettings = Variables.ServiceSettings?.ConvertToRpcServiceSettings() ?? new RpcServiceSettings()
            };

            // done
            return Task.FromResult(response);
        }

        /// <summary>
        /// Returns the current MQTT settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetServiceMqttSettingsResponse> GetServiceMqttSettings(GetServiceMqttSettingsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnServiceMqttSettingsError("invalid auth id", peer));

            // prepare the response
            var response = new GetServiceMqttSettingsResponse
            {
                Ok = true,
                ServiceMqttSettings = Variables.ServiceMqttSettings?.ConvertToRpcServiceMqttSettings() ?? new RpcServiceMqttSettings()
            };

            // done
            return Task.FromResult(response);
        }

        /// <summary>
        /// Returns a list of stored commands
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetConfiguredCommandsResponse> GetConfiguredCommands(GetConfiguredCommandsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);
            
            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnConfiguredCommandsError("invalid auth id", peer));

            // prepare the response
            var response = new GetConfiguredCommandsResponse
            {
                Ok = true
            };

            // add the commands
            var commands = Variables.Commands.Select(StoredCommands.ConvertAbstractToConfigured).ToList();
            foreach (var command in commands.Select(y => y?.ConvertToRpcConfiguredCommand())) response.ConfiguredServerCommands.Add(command);

            // done
            return Task.FromResult(response);
        }

        /// <summary>
        /// Returns a list of stored sensors
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetConfiguredSensorsResponse> GetConfiguredSensors(GetConfiguredSensorsRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnConfiguredSensorsError("invalid auth id", peer));

            // prepare the response
            var response = new GetConfiguredSensorsResponse
            {
                Ok = true
            };

            // add the sensors
            var sensors = Variables.SingleValueSensors.Select(StoredSensors.ConvertAbstractSingleValueToConfigured).Concat(Variables.MultiValueSensors.Select(StoredSensors.ConvertAbstractMultiValueToConfigured));
            foreach (var sensor in sensors.Select(y => y?.ConvertToRpcConfiguredSensor())) response.ConfiguredServerSensors.Add(sensor);

            // done
            return Task.FromResult(response);
        }
    }
}
