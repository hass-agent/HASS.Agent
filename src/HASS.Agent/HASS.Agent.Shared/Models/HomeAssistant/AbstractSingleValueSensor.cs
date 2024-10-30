using System;
using System.Threading.Tasks;
using HASS.Agent.Shared.Models.Internal;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Shared.Models.HomeAssistant;

/// <summary>
/// Abstract singlevalue-sensor from which all singlevalue-sensors are derived
/// </summary>
public abstract class AbstractSingleValueSensor : AbstractDiscoverable
{
    private SensorAdvancedSettings _advancedInfo;

    public int UpdateIntervalSeconds { get; protected set; }
    public DateTime? LastUpdated { get; protected set; }

    public string PreviousPublishedState { get; protected set; } = string.Empty;
    public string PreviousPublishedAttributes { get; protected set; } = string.Empty;

    public string AdvancedSettings { get; private set; }

    protected AbstractSingleValueSensor(string entityName, string name, int updateIntervalSeconds = 10, string id = default, bool useAttributes = false, string advancedSettings = default)
    {
        Id = id == null || id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : id;
        EntityName = entityName;
        Name = name;
        UpdateIntervalSeconds = updateIntervalSeconds;
        Domain = "sensor";
        UseAttributes = useAttributes;

        if (!string.IsNullOrWhiteSpace(advancedSettings))
        {
            AdvancedSettings = advancedSettings;
            _advancedInfo = JsonConvert.DeserializeObject<SensorAdvancedSettings>(advancedSettings);
        }
    }

    protected SensorDiscoveryConfigModel AutoDiscoveryConfigModel;
    protected SensorDiscoveryConfigModel SetAutoDiscoveryConfigModel(SensorDiscoveryConfigModel config)
    {
        AutoDiscoveryConfigModel = config;

        // overwrite with advanced settings
        if (_advancedInfo != null)
        {
            if (!string.IsNullOrWhiteSpace(_advancedInfo.DeviceClass))
                AutoDiscoveryConfigModel.Device_class = _advancedInfo.DeviceClass;
            if (!string.IsNullOrWhiteSpace(_advancedInfo.UnitOfMeasurement))
                AutoDiscoveryConfigModel.Unit_of_measurement = _advancedInfo.UnitOfMeasurement;
            if (!string.IsNullOrWhiteSpace(_advancedInfo.StateClass))
                AutoDiscoveryConfigModel.State_class = _advancedInfo.StateClass;
        }

        return AutoDiscoveryConfigModel;
    }

    public override void ClearAutoDiscoveryConfig() => AutoDiscoveryConfigModel = null;

    // nullable in preparation for possible future "nullable enablement"
    public abstract string? GetState();

    // nullable in preparation for possible future "nullable enablement"
    public abstract string? GetAttributes();

    public void ResetChecks()
    {
        LastUpdated = DateTime.MinValue;

        PreviousPublishedState = string.Empty;
        PreviousPublishedAttributes = string.Empty;
    }

    public async Task PublishStateAsync(bool respectChecks = true)
    {
        try
        {
            if (Variables.MqttManager == null)
                return;

            // are we asked to check elapsed time?
            if (respectChecks)
            {
                if (LastUpdated.HasValue && LastUpdated.Value.AddSeconds(UpdateIntervalSeconds) > DateTime.Now)
                    return;
            }

            // get the current state/attributes
            var state = GetState();
            if (state == null)
                return;

            var attributes = GetAttributes();

            // are we asked to check state changes?
            if (respectChecks)
            {
                if (PreviousPublishedState == state && PreviousPublishedAttributes == attributes)
                {
                    LastUpdated = DateTime.Now;
                    return;
                }
            }

            // fetch the autodiscovery config
            var autoDiscoConfig = (SensorDiscoveryConfigModel)GetAutoDiscoveryConfig();
            if (autoDiscoConfig == null)
                return;

            // prepare the state message
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(autoDiscoConfig.State_topic)
                .WithPayload(state)
                .WithRetainFlag(Variables.MqttManager.UseRetainFlag())
                .Build();

            // send it
            var published = await Variables.MqttManager.PublishAsync(message);
            if (!published)
                return;

            // optionally prepare and send attributes
            if (UseAttributes && attributes != null)
            {
                message = new MqttApplicationMessageBuilder()
                    .WithTopic(autoDiscoConfig.Json_attributes_topic)
                    .WithPayload(attributes)
                    .WithRetainFlag(Variables.MqttManager.UseRetainFlag())
                    .Build();

                published = await Variables.MqttManager.PublishAsync(message);
                if (!published)
                    return;
            }

            // only store the values if the checks are respected
            // otherwise, we might stay in 'unknown' state until the value changes
            if (!respectChecks)
                return;

            PreviousPublishedState = state;
            if (attributes != null)
                PreviousPublishedAttributes = attributes;

            LastUpdated = DateTime.Now;
        }
        catch (Exception ex)
        {
            Log.Fatal("[SENSOR] [{name}] Error publishing state: {err}", EntityName, ex.Message);
        }
    }

    public async Task PublishAutoDiscoveryConfigAsync()
    {
        if (Variables.MqttManager == null)
            return;

        await Variables.MqttManager.AnnounceAutoDiscoveryConfigAsync(this, Domain);
    }

    public async Task UnPublishAutoDiscoveryConfigAsync()
    {
        if (Variables.MqttManager == null)
            return;

        await Variables.MqttManager.AnnounceAutoDiscoveryConfigAsync(this, Domain, true);
    }
}
