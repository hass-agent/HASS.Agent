using Grpc.Core;
using HASS.Agent.Satellite.Service.Extensions;
using HASS.Agent.Satellite.Service.Functions;
using HASS.Agent.Satellite.Service.Settings;
using Serilog;

namespace HASS.Agent.Satellite.Service.RPC
{
    public partial class HassAgentSatelliteRpcCallsService : HassAgentSatelliteRpcCalls.HassAgentSatelliteRpcCallsBase
    {
        public HassAgentSatelliteRpcCallsService()
        {
            //
        }

        /// <summary>
        /// Processes a ping request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // no auth check for pings
            return Task.FromResult(RpcHelper.ReturnPingOk());
        }

        /// <summary>
        /// Clears all entities and unpublishes them
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<AckResponse> ClearEntities(ClearEntitiesRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnError("invalid auth id", peer));

            // execute clearing of entities
            Task.Run(SettingsManager.ClearAllEntities);

            // done
            return Task.FromResult(RpcHelper.ReturnOk());
        }

        /// <summary>
        /// Shuts the service down
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<AckResponse> ShutdownService(ShutdownServiceRequest request, ServerCallContext context)
        {
            var method = HelperFunctions.CurrentMethodName();

            // ServerCallContext.Peer isn't implemented ..
            // var peer = context.Peer ?? "-";
            var peer = "-";

            Log.Debug("[RPC] [{method}] [{peer}] Received request", method, peer);

            // check id
            if (!request.Auth.CheckAuthId(method, true)) return Task.FromResult(RpcHelper.ReturnError("invalid auth id", peer));

            // shut us down
            Task.Run(HelperFunctions.ShutdownAsync);

            // done
            return Task.FromResult(RpcHelper.ReturnOk());
        }
    }
}
