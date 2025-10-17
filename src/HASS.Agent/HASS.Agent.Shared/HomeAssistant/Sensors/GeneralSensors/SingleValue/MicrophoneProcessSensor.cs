using System.Collections.Generic;
using System.Linq;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;

/// <summary>
/// Sensor indicating whether the microphone is in use
/// </summary>
public class MicrophoneProcessSensor : AbstractSingleValueSensor
{
    private const string DefaultName = "microphoneprocess";

    private const string _lastUsedTimeStop = "LastUsedTimeStop";
    private const string _regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone";
    
    public MicrophoneProcessSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, true, advancedSettings: advancedSettings)
    {
        //
    }

    private readonly Dictionary<string, string> _processes = new();

    private string _attributes = string.Empty;

    public override DiscoveryConfigModel GetAutoDiscoveryConfig()
    {
        if (Variables.MqttManager == null)
        {
            return null;
        }

        var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
        if (deviceConfig == null)
        {
            return null;
        }

        var model = new SensorDiscoveryConfigModel(Domain)
        {
            EntityName = EntityName,
            Name = Name,
            Unique_id = Id,
            Device = deviceConfig,
            State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
            State_class = "measurement",
            Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
            Icon = "mdi:microphone",
            Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes"
        };

        return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(model);
    }

    private string MicrophoneProcess()
    {
        _processes.Clear();

        // first local machine
        using (var key = Registry.LocalMachine.OpenSubKey(_regKey))
        {
            CheckRegForMicrophoneInUse(key);
        }

        // then current user
        using (var key = Registry.CurrentUser.OpenSubKey(_regKey))
        {
            CheckRegForMicrophoneInUse(key);
        }

        // add processes as attributes
        _attributes = _processes.Count > 0 ? JsonConvert.SerializeObject(_processes, Formatting.Indented) : "{}";

        // return the count
        return _processes.Count.ToString();
    }

    private void CheckRegForMicrophoneInUse(RegistryKey key)
    {
        if (key == null)
        {
            return;
        }

        foreach (var subKeyName in key.GetSubKeyNames())
        {
            // NonPackaged has multiple subkeys
            if (subKeyName == "NonPackaged")
            {
                using var nonpackagedkey = key.OpenSubKey(subKeyName);
                if (nonpackagedkey == null)
                {
                    continue;
                }

                foreach (var nonpackagedSubKeyName in nonpackagedkey.GetSubKeyNames())
                {
                    using var subKey = nonpackagedkey.OpenSubKey(nonpackagedSubKeyName);
                    if (subKey == null || !subKey.GetValueNames().Contains(_lastUsedTimeStop))
                    {
                        continue;
                    }

                    var endTime = subKey.GetValue(_lastUsedTimeStop) is long
                        ? (long)(subKey.GetValue(_lastUsedTimeStop) ?? -1)
                        : -1;

                    if (endTime <= 0)
                    {
                        _processes[SharedHelperFunctions.ParseRegWebcamMicApplicationName(subKey.Name)] = "on";
                    }
                }
            }
            else
            {
                using var subKey = key.OpenSubKey(subKeyName);
                if (subKey == null || !subKey.GetValueNames().Contains(_lastUsedTimeStop))
                {
                    continue;
                }

                var endTime = subKey.GetValue(_lastUsedTimeStop) is long ? (long)(subKey.GetValue(_lastUsedTimeStop) ?? -1) : -1;
                if (endTime <= 0)
                {
                    _processes[SharedHelperFunctions.ParseRegWebcamMicApplicationName(subKey.Name)] = "on";
                }
            }
        }
    }

    public override string GetState() => MicrophoneProcess();
    public override string GetAttributes() => _attributes;
}
