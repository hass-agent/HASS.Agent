using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace HASS.Agent.Shared.Managers.Audio.Internal;

internal class DeviceNotificationEventArgs : EventArgs
{
    public string DeviceId { get; private set; }

    public DeviceNotificationEventArgs(string deviceId)
    {
        DeviceId = deviceId;
    }
}

internal class DefaultDeviceChangedEventArgs : DeviceNotificationEventArgs
{
    public DataFlow DataFlow { get; private set; }
    public Role Role { get; private set; }

    public DefaultDeviceChangedEventArgs(string deviceId, DataFlow dataFlow, Role role) : base(deviceId)
    {
        DataFlow = dataFlow;
        Role = role;
    }
}

internal class DeviceStateChangedEventArgs : DeviceNotificationEventArgs
{
    public DeviceState DeviceState { get; private set; }

    public DeviceStateChangedEventArgs(string deviceId, DeviceState deviceState) : base(deviceId)
    {
        DeviceState = deviceState;
    }
}

internal class DevicePropertyChangedEventArgs : DeviceNotificationEventArgs
{
    public PropertyKey PropertyKey { get; private set; }

    public DevicePropertyChangedEventArgs(string deviceId, PropertyKey propertyKey) : base(deviceId)
    {
        PropertyKey = propertyKey;
    }
}