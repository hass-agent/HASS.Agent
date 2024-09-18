using System.Text.Json.Nodes;
using HASS.Agent.Enums;
using Newtonsoft.Json.Linq;

namespace HASS.Agent.Models.HomeAssistant;

public class MqttMediaPlayerCommand
{
    public MediaPlayerCommand Command { get; set; }
    public JValue Data { get; set; }
}