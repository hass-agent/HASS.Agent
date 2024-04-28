using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Shared.Managers.Audio;
public class AudioSession
{
    public string Id { get; set; }
    public string Application { get; set; } = string.Empty;
    public string PlaybackDevice { get; set; } = string.Empty;
    public bool Muted { get; set; }
    public bool Active { get; set; }
    public int MasterVolume { get; set; }
    public int PeakVolume { get; set; }
}
