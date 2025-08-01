using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Text;

namespace HASS.Agent.Shared.Models.Internal
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class NetworkInfo
    {
        public NetworkInfo()
        {
            //
        }

        public string Name { get; set; } = string.Empty;
        public string NetworkInterfaceType { get; set; } = "Unknown";
        public double SpeedBitsPerSecond { get; set; } = 0d;
        public string OperationalStatus { get; set; } = "Unknown";
        public double DataReceivedMB { get; set; } = 0d;
        public double DataSentMB { get; set; } = 0d;
        public double IncomingPacketsDiscarded { get; set; } = 0d;
        public double IncomingPacketsWithErrors { get; set; } = 0d;
        public double IncomingPacketsWithUnknownProtocol { get; set; } = 0d;
        public double OutgoingPacketsDiscarded { get; set; } = 0d;
        public double OutgoingPacketsWithErrors { get; set; } = 0d;
        public List<string> IpAddresses { get; set; } = new List<string>();
        public List<string> MacAddresses { get; set; } = new List<string>();
        public List<string> Gateways { get; set; } = new List<string>();
        public bool DhcpEnabled { get; set; }
        public List<string> DhcpAddresses { get; set; } = new List<string>();
        public bool DnsEnabled { get; set; }
        public string DnsSuffix { get; set; } = string.Empty;
        public List<string> DnsAddresses { get; set; } = new List<string>();
    }
}
