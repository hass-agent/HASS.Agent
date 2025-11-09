using System;
using System.Globalization;
using System.Management;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Managers.VoiceMeeterAudio;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors;

/// <summary>
/// Sensor containing the result of the provided Powershell command or script
/// </summary>
public class VoicemeeterSensor : AbstractSingleValueSensor
{
    private const string DefaultName = "voicemeetersensor";

    public string Command { get; private set; }
    public bool ApplyRounding { get; private set; }
    public int? Round { get; private set; }

    public VoicemeeterSensor(string command, bool applyRounding = false, int? round = null, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, advancedSettings: advancedSettings)
    {
        Command = command;
        ApplyRounding = applyRounding;
        Round = round;
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig()
    {
        if (Variables.MqttManager == null)
            return null;

        var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
        if (deviceConfig == null)
            return null;

        return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel(Domain)
        {
            EntityName = EntityName,
            Name = Name,
            Unique_id = Id,
            Device = deviceConfig,
            State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{EntityName}/state",
            Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
        });
    }

    public override string GetState()
    {
        var result = VoiceMeeterAudioManager.GetParameter(Command);
        if (result == float.NaN)
        {
            return null;
        }
        // optionally apply rounding
        if (ApplyRounding && Round != null)
        {
            result = (float)Math.Round(result, (int)Round);
        }
        return result.ToString();
    }

    public override string GetAttributes() => string.Empty;
}