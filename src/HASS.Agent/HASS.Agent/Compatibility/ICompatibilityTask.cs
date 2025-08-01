using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Compatibility
{
    public interface ICompatibilityTask
    {
        public string Name { get; }

        public abstract Task<(bool, string)> Perform();
    }
}
