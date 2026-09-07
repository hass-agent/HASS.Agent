using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HASS.Agent.Shared.Models.Internal;

[JsonConverter(typeof(StringEnumConverter))]
public enum NetworkAccessType
{
    [EnumMember(Value = "NoNetworkAccess")]
    NoNetworkAccess,
    [EnumMember(Value = "NoInternetAccess")]
    NoInternetAccess,
    [EnumMember(Value = "Internet")]
    Internet
}