using System;
using System.Threading.Tasks;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Extensions;
using MQTTnet;
using Serilog;

namespace HASS.Agent.Shared.Models.HomeAssistant
{
    /// <summary>
    /// Abstract command from which all commands are derived
    /// </summary>
    public abstract class AbstractCommand : AbstractDiscoverable
    {
        public int UpdateIntervalSeconds => 1;
        public DateTime? LastUpdated { get; set; }
        public string PreviousPublishedState { get; set; } = string.Empty;
        public CommandEntityType EntityType { get; set; }

        protected AbstractCommand(string entityName, string name, CommandEntityType entityType = CommandEntityType.Switch, string id = default)
        {
            Id = id == null || id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : id;
            EntityName = entityName;
            Name = name;
            Domain = entityType.GetEnumMemberValue();
            EntityType = entityType;
        }

        protected CommandDiscoveryConfigModel AutoDiscoveryConfigModel;
        protected CommandDiscoveryConfigModel SetAutoDiscoveryConfigModel(CommandDiscoveryConfigModel config)
        {
            AutoDiscoveryConfigModel = config;
            return config;
        }

        public override void ClearAutoDiscoveryConfig() => AutoDiscoveryConfigModel = null;

        public abstract string GetState();
        
        public async Task PublishStateAsync(bool respectChecks = true)
        {
            try
            {
                if (Variables.MqttManager == null) return;

                var state = GetState();

                if (respectChecks)
                {
                    if (LastUpdated.HasValue && LastUpdated.Value.AddSeconds(UpdateIntervalSeconds) > DateTime.Now) return;
                    if (PreviousPublishedState == state) return;
                }

                var autoDiscoConfig = GetAutoDiscoveryConfig();
                if (autoDiscoConfig == null) return;
                
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(autoDiscoConfig.State_topic)
                    .WithPayload(state)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                    .WithRetainFlag(Variables.MqttManager.UseRetainFlag())
                    .Build();

                var published = await Variables.MqttManager.PublishAsync(message);
                if (!published)
                {
                    // failed, don't store the state
                    return;
                }

                PreviousPublishedState = state;
                LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                Log.Fatal("[COMMAND] [{name}] Error publishing state: {err}", EntityName, ex.Message);
            }
        }

        public async Task PublishAutoDiscoveryConfigAsync()
        {
            if (Variables.MqttManager == null) return;
            await Variables.MqttManager.AnnounceAutoDiscoveryConfigAsync(this, Domain);
        }

        public async Task UnPublishAutoDiscoveryConfigAsync()
        {
            if (Variables.MqttManager == null) return;
            await Variables.MqttManager.AnnounceAutoDiscoveryConfigAsync(this, Domain, true);
        }

        public abstract void TurnOn();
        public abstract void TurnOnWithAction(string action);
        public abstract void TurnOff();
    }
}
