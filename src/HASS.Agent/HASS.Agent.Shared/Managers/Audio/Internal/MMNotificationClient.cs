using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.CoreAudioApi;

namespace HASS.Agent.Shared.Managers.Audio.Internal;

internal class MMNotificationClient : IMMNotificationClient
{
    public event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;
    public event EventHandler<DeviceNotificationEventArgs> DeviceAdded;
    public event EventHandler<DeviceNotificationEventArgs> DeviceRemoved;
    public event EventHandler<DefaultDeviceChangedEventArgs> DefaultDeviceChanged;
    public event EventHandler<DevicePropertyChangedEventArgs> DevicePropertyChanged;

    void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState deviceState)
    {
        DeviceStateChanged?.Invoke(this, new DeviceStateChangedEventArgs(deviceId, deviceState));
    }

    void IMMNotificationClient.OnDeviceAdded(string deviceId)
    {
        DeviceAdded?.Invoke(this, new DeviceNotificationEventArgs(deviceId));
    }

    void IMMNotificationClient.OnDeviceRemoved(string deviceId)
    {
        DeviceRemoved?.Invoke(this, new DeviceNotificationEventArgs(deviceId));
    }

    void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow dataFlow, Role role, string deviceId)
    {
        DefaultDeviceChanged?.Invoke(this, new DefaultDeviceChangedEventArgs(deviceId, dataFlow, role));
    }

    void IMMNotificationClient.OnPropertyValueChanged(string deviceId, PropertyKey key)
    {
        DevicePropertyChanged?.Invoke(this, new DevicePropertyChangedEventArgs(deviceId, key));
    }
}
