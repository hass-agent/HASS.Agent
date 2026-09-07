using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using HASS.Agent.Shared.Models.Internal;
using Vanara.PInvoke.NetListMgr;

namespace HASS.Agent.Shared.Managers;

public static class NetworkManager
{
    private static INetworkListManager _networkListManager = new INetworkListManager();

    public const NLM_CONNECTIVITY InternetMask = NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV4_INTERNET |
                                                 NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV6_INTERNET;

    public const NLM_CONNECTIVITY LocalMask = NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV4_SUBNET |
                                              NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV4_LOCALNETWORK |
                                              NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV6_SUBNET |
                                              NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV6_LOCALNETWORK;
    
    public static NetworkAccessType GetNetworkAccessType(NetworkInterface networkInterface)
    {
        if (!Guid.TryParse(networkInterface.Id, out var adapterGuid))
        {
            return NetworkAccessType.NoNetworkAccess;
        }

        var networkConnection = _networkListManager.GetNetworkConnections().Cast<INetworkConnection>().FirstOrDefault(c => c.GetAdapterId() == adapterGuid);

        return networkConnection == null ? NetworkAccessType.NoNetworkAccess : ConnectivityToAccessType(networkConnection.GetConnectivity());
    }

    public static NetworkAccessType ConnectivityToAccessType(NLM_CONNECTIVITY connectivity)
    {
        if ((connectivity & InternetMask) != 0)
        {
            return NetworkAccessType.Internet;
        }

        if ((connectivity & LocalMask) != 0)
        {
            return NetworkAccessType.NoInternetAccess;
        }

        return NetworkAccessType.NoNetworkAccess;
    }
}