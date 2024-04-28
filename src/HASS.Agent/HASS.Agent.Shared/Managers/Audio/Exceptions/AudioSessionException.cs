using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Shared.Managers.Audio.Exceptions;
public class AudioSessionException : Exception
{
    public AudioSessionException()
    {
    }

    public AudioSessionException(string message) : base(message)
    {
    }

    public AudioSessionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
