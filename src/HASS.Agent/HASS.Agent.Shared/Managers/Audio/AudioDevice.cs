using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Shared.Managers.Audio;
public class AudioDevice
{
    public string State { get; set; } = string.Empty;
    public DeviceType Type { get; set; }
    public string Id { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public int Volume { get; set; }
    public double PeakVolume { get; set; }
    public bool Muted { get; set; }
    public List<AudioSession> Sessions { get; set; } = new();
    public bool Default { get; set; }
}
