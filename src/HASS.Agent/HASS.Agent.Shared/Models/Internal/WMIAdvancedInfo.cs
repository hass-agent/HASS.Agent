using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Shared.Models.Internal;
public class WMIAdvancedInfo
{
    public string DeviceClass { get; set; }
    public string UnitOfMeasurement { get; set; }
    public string StateClass { get; set; }
}
