using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ByteSizeLib;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue;

/// <summary>
/// Multivalue sensor containing network and NIC info
/// </summary>
public class NetworkSensors : AbstractMultiValueSensor
{
    private const string DefaultName = "network";
    private readonly int _updateInterval;

    public string NetworkCard { get; protected set; }
    private readonly bool _useSpecificCard = false;

    public override sealed Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

    public NetworkSensors(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string networkCard = "*", string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id)
    {
        _updateInterval = updateInterval ?? 30;

        NetworkCard = networkCard;
        _useSpecificCard = networkCard != "*" && !string.IsNullOrEmpty(networkCard);

        UpdateSensorValues();
    }

    private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
    {
        if (!Sensors.ContainsKey(sensorId))
            Sensors.Add(sensorId, sensor);
        else
            Sensors[sensorId] = sensor;
    }

    public override sealed void UpdateSensorValues()
    {
        var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);

        var nicCount = 0;
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var nic in networkInterfaces)
        {
            try
            {
                if (nic == null)
                    continue;

                if (_useSpecificCard && nic.Id != NetworkCard)
                    continue;

                var id = nic.Id.Replace("{", "").Replace("}", "").Replace("-", "").ToLower();
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                var networkInfo = new NetworkInfo
                {
                    Name = nic.Name,
                    NetworkInterfaceType = nic.NetworkInterfaceType.ToString(),
                    SpeedBitsPerSecond = nic.Speed,
                    OperationalStatus = nic.OperationalStatus.ToString()
                };

                var interfaceStats = nic.GetIPv4Statistics();

                networkInfo.DataReceivedMB = Math.Round(ByteSize.FromBytes(interfaceStats.BytesReceived).MegaBytes);
                networkInfo.DataSentMB = Math.Round(ByteSize.FromBytes(interfaceStats.BytesSent).MegaBytes);
                networkInfo.IncomingPacketsDiscarded = interfaceStats.IncomingPacketsDiscarded;
                networkInfo.IncomingPacketsWithErrors = interfaceStats.IncomingPacketsWithErrors;
                networkInfo.IncomingPacketsWithUnknownProtocol = interfaceStats.IncomingUnknownProtocolPackets;
                networkInfo.OutgoingPacketsDiscarded = interfaceStats.OutgoingPacketsDiscarded;
                networkInfo.OutgoingPacketsWithErrors = interfaceStats.OutgoingPacketsWithErrors;

                var nicProperties = nic.GetIPProperties();

                foreach (var unicast in nicProperties.UnicastAddresses)
                {
                    var ip = unicast.Address.ToString();
                    if (!string.IsNullOrEmpty(ip) && !networkInfo.IpAddresses.Contains(ip))
                        networkInfo.IpAddresses.Add(ip);

                    var mac = nic.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(mac) && !networkInfo.MacAddresses.Contains(mac))
                        networkInfo.MacAddresses.Add(mac);
                }

                foreach (var gateway in nicProperties.GatewayAddresses)
                {
                    var gatewayAddress = gateway.Address.ToString();
                    if (!string.IsNullOrEmpty(gatewayAddress) && !networkInfo.Gateways.Contains(gatewayAddress))
                        networkInfo.Gateways.Add(gatewayAddress);
                }

                networkInfo.DhcpEnabled = nicProperties.GetIPv4Properties().IsDhcpEnabled;

                foreach (var dhcp in nicProperties.DhcpServerAddresses)
                {
                    var dhcpAddress = dhcp.ToString();
                    if (!string.IsNullOrEmpty(dhcpAddress) && !networkInfo.DhcpAddresses.Contains(dhcpAddress))
                        networkInfo.DhcpAddresses.Add(dhcpAddress);
                }

                networkInfo.DnsEnabled = nicProperties.IsDnsEnabled;
                networkInfo.DnsSuffix = nicProperties.DnsSuffix;

                foreach (var dns in nicProperties.DnsAddresses)
                {
                    var dnsAddress = dns.ToString();
                    if (!string.IsNullOrEmpty(dnsAddress) && !networkInfo.DnsAddresses.Contains(dnsAddress))
                        networkInfo.DnsAddresses.Add(dnsAddress);
                }

                var info = JsonConvert.SerializeObject(networkInfo, Formatting.Indented);
                var networkInfoEntityName = $"{parentSensorSafeName}_{id}";
                var networkInfoId = $"{Id}_{id}";
                var networkInfoSensor = new DataTypeStringSensor(_updateInterval, networkInfoEntityName, nic.Name, networkInfoId, string.Empty, "mdi:lan", string.Empty, EntityName, true);

                networkInfoSensor.SetState(nic.OperationalStatus.ToString());
                networkInfoSensor.SetAttributes(info);
                AddUpdateSensor(networkInfoId, networkInfoSensor);

                nicCount++;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[NETWORK] [{name}] Error querying NIC: {msg}", EntityName, ex.Message);
            }
        }

        var nicCountEntityName = $"{parentSensorSafeName}_total_network_card_count";
        var nicCountId = $"{Id}_total_network_card_count";
        var nicCountSensor = new DataTypeIntSensor(_updateInterval, nicCountEntityName, "Network Card Count", nicCountId, string.Empty, "measurement", "mdi:lan", string.Empty, EntityName);
        nicCountSensor.SetState(nicCount);
        AddUpdateSensor(nicCountId, nicCountSensor);
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
}
