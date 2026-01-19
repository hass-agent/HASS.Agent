using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Mqtt;
using HASS.Agent.Satellite.Service.Functions;
using HASS.Agent.Satellite.Service.Settings;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Extensions.ManagedClient;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace HASS.Agent.Satellite.Service.MQTT
{
    /// <summary>
    /// Handles publishing and processing HASS entities (commands and sensors) through MQTT
    /// </summary>
    public class MqttManager : IMqttManager
    {
        private IManagedMqttClient? _mqttClient = null;

        private bool _disconnectionNotified = false;
        private bool _connectingFailureNotified = false;

        private MqttStatus _status = MqttStatus.Connecting;

        /// <summary>
        /// Returns whether the client is connected
        /// </summary>
        public bool IsConnected() => _mqttClient is { IsConnected: true };

        private bool _isReady = false;
        /// <summary>
        /// Returns whether MqttManager is ready for operation
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            return IsConnected() && _isReady;
        }

        /// <summary>
        /// Returns whether the user wants the retain flag raised
        /// </summary>
        /// <returns></returns>
        public bool UseRetainFlag() => Variables.ServiceMqttSettings?.MqttUseRetainFlag ?? true;

        public Task SubscribeMediaCommandsAsync() => Task.CompletedTask;
        public Task SubscribeNotificationsAsync() => Task.CompletedTask;

        /// <summary>
        /// Returns the default or configured discovery prefix
        /// </summary>
        /// <returns></returns>
        public string MqttDiscoveryPrefix() => Variables.ServiceMqttSettings?.MqttDiscoveryPrefix ?? "homeassistant";

        /// <summary>
        /// Returns the device's config model
        /// </summary>
        /// <returns></returns>
        public DeviceConfigModel? GetDeviceConfigModel()
        {
            if (Variables.DeviceConfig != null)
                return Variables.DeviceConfig;

            CreateDeviceConfigModel();
            return Variables.DeviceConfig ?? null;
        }

        /// <summary>
        /// Initialize the MQTT manager, establish a connection
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (Variables.DeviceConfig == null)
                    CreateDeviceConfigModel();

                _mqttClient = Variables.MqttFactory.CreateManagedMqttClient();
                _mqttClient.ConnectedAsync += OnMqttConnected;
                _mqttClient.ConnectingFailedAsync += OnMqttConnectionFailed;
                _mqttClient.ApplicationMessageReceivedAsync += e => HandleMessageReceived(e.ApplicationMessage);
                _mqttClient.DisconnectedAsync += OnMqttDisconnected;

                var options = GetOptions();
                if (options == null)
                {
                    _status = MqttStatus.ConfigMissing;
                    Log.Warning("[MQTT] Configuration missing");

                    return;
                }

                Log.Information("[MQTT] Connecting ..");
                StartClient(options);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while initializing: {err}", ex.Message);
                _status = MqttStatus.Error;
            }
        }

        /// <summary>
        /// Fires when the client gets disconnected from the broker
        /// </summary>
        /// <param name="arg"></param>
        private async Task OnMqttDisconnected(MqttClientDisconnectedEventArgs arg)
        {
            try
            {
                _isReady = false;

                // give the connection the grace period to recover
                var runningTimer = Stopwatch.StartNew();
                while (runningTimer.Elapsed.TotalSeconds < Variables.ServiceSettings?.DisconnectedGracePeriodSeconds)
                {
                    if (IsConnected())
                    {
                        _isReady = true;

                        return;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                // nope, call it
                _status = MqttStatus.Disconnected;

                // log if we're not shutting down, but only once
                if (Variables.ShuttingDown || _disconnectionNotified)
                    return;

                _disconnectionNotified = true;
                Log.Warning("[MQTT] Disconnected: {reason}", arg.Reason.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("[MQTT] Error while trying to handle disconnection: {msg}", ex.ToString());
            }
        }

        /// <summary>
        /// Fires when connecting to the broker failed
        /// </summary>
        private async Task OnMqttConnectionFailed(ConnectingFailedEventArgs arg)
        {
            try
            {
                // give the connection the grace period to recover
                var runningTimer = Stopwatch.StartNew();
                while (runningTimer.Elapsed.TotalSeconds < Variables.ServiceSettings?.DisconnectedGracePeriodSeconds)
                {
                    // recovered
                    if (IsConnected())
                        return;

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                // nope, call it
                _status = MqttStatus.Error;

                // log only once
                if (_connectingFailureNotified)
                    return;

                _connectingFailureNotified = true;

                Log.Fatal(arg.Exception, "[MQTT] Error while connecting: {err}", arg.Exception.Message);
            }
            catch (Exception exc)
            {
                Log.Error("[MQTT] Error while trying to handle failed connection: {msg}", exc.ToString());
            }
        }

        private Task OnMqttConnected(MqttClientConnectedEventArgs arg)
        {
            _status = MqttStatus.Connected;
            Log.Information("[MQTT] Connected");

            // reset error notifications 
            _disconnectionNotified = false;
            _connectingFailureNotified = false;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a new MQTT client, using the current configuration
        /// </summary>
        public async void ReloadConfiguration()
        {
            try
            {
                Log.Information("[MQTT] Reloading configuration ..");

                // already connected?
                if (_mqttClient != null)
                {
                    await _mqttClient.StopAsync();

                    _mqttClient.Dispose();
                    _mqttClient = null;
                }

                Variables.DeviceConfig = null;

                // reset state
                _status = MqttStatus.Connecting;
                _disconnectionNotified = false;
                _connectingFailureNotified = false;

                Log.Information("[MQTT] Initializing ..");
                Initialize();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while reloading configuration: {err}", ex.Message);
                _status = MqttStatus.Error;
            }
        }

        /// <summary>
        /// Start and register a MQTT client with the provided options
        /// </summary>
        /// <param name="options"></param>
        private async void StartClient(ManagedMqttClientOptions options)
        {
            if (_mqttClient == null)
                return;

            try
            {
                await _mqttClient.StartAsync(options);
                InitialRegistration();
            }
            catch (MqttConnectingFailedException ex)
            {
                Log.Error("[MQTT] Unable to connect to broker: {msg}", ex.Result.ToString());
            }
            catch (MqttCommunicationException ex)
            {
                Log.Error("[MQTT] Unable to communicate with broker: {msg}", ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error("[MQTT] Exception while connecting with broker: {msg}", ex.ToString());
            }
        }


        /// <summary>
        /// Announce our availability
        /// <para>Deprecated: used to announce sensors/commands, now left to their managers</para>
        /// </summary>
        private async void InitialRegistration()
        {
            if (_mqttClient == null)
                return;

            while (!IsConnected())
                await Task.Delay(2000);

            await AnnounceAvailabilityAsync();
            _isReady = true;

            Log.Information("[MQTT] Initial registration completed");
        }

        /// <summary>
        /// Prepares info for the device we're running on
        /// </summary>
        public void CreateDeviceConfigModel()
        {
            var name = HelperFunctions.GetConfiguredDeviceName();
            Log.Information("[MQTT] Identifying as device: {name}", name);

            Variables.DeviceConfig = new DeviceConfigModel
            {
                Name = name,
                Identifiers = "hass.agent-" + name,
                Manufacturer = "HASS.Agent Team",
                Model = Environment.OSVersion.ToString(),
                Sw_version = Variables.Version
            };
        }

        private DateTime _lastPublishFailedLogged = DateTime.MinValue;
        /// <summary>
        /// Publishes the provided message
        /// </summary>
        public async Task<bool> PublishAsync(MqttApplicationMessage message)
        {
            try
            {
                if (_mqttClient == null)
                    return false;

                if (!IsConnected())
                {
                    // only log failures once every 5 minutes to minimize log growth
                    if ((DateTime.Now - _lastPublishFailedLogged).TotalMinutes < 5)
                        return false;

                    _lastPublishFailedLogged = DateTime.Now;

                    if (Variables.ExtendedLogging)
                        Log.Warning("[MQTT] Not connected, message dropped (won't report again for 5 minutes)");

                    return false;
                }

                var published = await _mqttClient.InternalClient.PublishAsync(message);
                if (published.ReasonCode == MqttClientPublishReasonCode.Success)
                    return true;

                // only log failures once every 5 minutes to minimize log growth
                if ((DateTime.Now - _lastPublishFailedLogged).TotalMinutes < 5)
                    return false;

                _lastPublishFailedLogged = DateTime.Now;

                if (Variables.ExtendedLogging)
                    Log.Warning("[MQTT] Publishing message failed, reason: [{reason}] {reasonStr}", published.ReasonCode.ToString(), published.ReasonString ?? string.Empty);

                return false;
            }
            catch (Exception ex)
            {
                // only log failures once every 5 minutes to minimize log growth
                if ((DateTime.Now - _lastPublishFailedLogged).TotalMinutes < 5)
                    return false;

                _lastPublishFailedLogged = DateTime.Now;
                Log.Fatal("[MQTT] Error publishing message: {err}", ex.Message);

                return false;
            }
        }

        private DateTime _lastAutoDiscoConfigFailedLogged = DateTime.MinValue;
        /// <summary>
        /// Publish the provided autodiscovery config
        /// </summary>
        /// <param name="discoverable"></param>
        /// <param name="domain"></param>
        /// <param name="clearConfig"></param>
        /// <returns></returns>
        public async Task AnnounceAutoDiscoveryConfigAsync(AbstractDiscoverable discoverable, string domain, bool clearConfig = false, bool migration = false)
        {
            if (!IsConnected())
                return;

            try
            {
                if (Variables.DeviceConfig == null)
                {
                    // only log failures once every 5 minutes to minimize log growth
                    if ((DateTime.Now - _lastAutoDiscoConfigFailedLogged).TotalMinutes < 5)
                        return;

                    _lastAutoDiscoConfigFailedLogged = DateTime.Now;
                    Log.Warning("[MQTT] Not connected, autodiscovery config dropped (won't report again for 5 minutes)");

                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new CamelCaseJsonNamingpolicy(),
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                if (string.IsNullOrEmpty(Variables.ServiceMqttSettings!.MqttDiscoveryPrefix))
                    Variables.ServiceMqttSettings.MqttDiscoveryPrefix = "homeassistant";

                var topic = $"{Variables.ServiceMqttSettings.MqttDiscoveryPrefix}/{domain}/{Variables.DeviceConfig.Name}/{discoverable.ObjectId}/config";

                var messageBuilder = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithRetainFlag(Variables.ServiceMqttSettings.MqttUseRetainFlag);

                if (clearConfig)
                {
                    var payload = migration ? Encoding.UTF8.GetBytes("{\"migrate_discovery\": true }") : Array.Empty<byte>();
                    messageBuilder.WithPayload(payload);
                }
                else
                {
                    var payload = discoverable.GetAutoDiscoveryConfig();
                    if (discoverable.IgnoreAvailability)
                        payload.Availability_topic = null;

                    messageBuilder.WithPayload(JsonSerializer.Serialize(payload, payload.GetType(), options));
                }
                await PublishAsync(messageBuilder.Build());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while announcing autodiscovery: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Returns the status of the MQTT manager
        /// </summary>
        /// <returns></returns>
        public MqttStatus GetStatus() => _status;

        private DateTime _lastAvailableAnnouncement = DateTime.MinValue;
        private DateTime _lastAvailableAnnouncementFailedLogged = DateTime.MinValue;
        /// <summary>
        /// Announce our availability
        /// </summary>
        public async Task AnnounceAvailabilityAsync(bool offline = false)
        {
            if (!IsConnected())
                return;

            try
            {
                // offline msgs always need to be sent, the rest once every 30 secs
                if (!offline)
                {
                    if ((DateTime.Now - _lastAvailableAnnouncement).TotalSeconds < 30)
                        return;

                    _lastAvailableAnnouncement = DateTime.Now;
                }

                if (IsConnected() && Variables.DeviceConfig != null)
                {
                    if (string.IsNullOrEmpty(Variables.ServiceMqttSettings!.MqttDiscoveryPrefix))
                        Variables.ServiceMqttSettings.MqttDiscoveryPrefix = "homeassistant";

                    var messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"{Variables.ServiceMqttSettings.MqttDiscoveryPrefix}/sensor/{Variables.DeviceConfig.Name}/availability")
                        .WithPayload(offline ? "offline" : "online")
                        .WithRetainFlag(Variables.ServiceMqttSettings.MqttUseRetainFlag);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());
                }
                else
                {
                    // only log failures once every 5 minutes to minimize log growth
                    if ((DateTime.Now - _lastAvailableAnnouncementFailedLogged).TotalMinutes < 5)
                        return;

                    _lastAvailableAnnouncementFailedLogged = DateTime.Now;

                    Log.Warning(!_mqttClient.IsConnected
                        ? "[MQTT] Not connected, availability announcement dropped"
                        : "[MQTT] Device config not found, availability announcement dropped");
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while announcing availability: {err}", ex.Message);
            }
        }

        private DateTime _lastClearDeviceConfigFailedLogged = DateTime.MinValue;
        /// <summary>
        /// CLears the device config, removing the retained message
        /// </summary>
        /// <returns></returns>
        public async Task ClearDeviceConfigAsync()
        {
            if (!IsConnected())
            {
                Log.Warning("[MQTT] Not connected, clearing device config failed");

                return;
            }

            try
            {
                if (IsConnected() && Variables.DeviceConfig != null)
                {
                    if (string.IsNullOrEmpty(Variables.ServiceMqttSettings!.MqttDiscoveryPrefix))
                        Variables.ServiceMqttSettings.MqttDiscoveryPrefix = "homeassistant";

                    var messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"{Variables.ServiceMqttSettings.MqttDiscoveryPrefix}/sensor/{Variables.DeviceConfig.Name}/availability")
                        .WithPayload(Array.Empty<byte>())
                        .WithRetainFlag(Variables.ServiceMqttSettings.MqttUseRetainFlag);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());
                }
                else
                {
                    // only log failures once every 5 minutes to minimize log growth
                    if ((DateTime.Now - _lastClearDeviceConfigFailedLogged).TotalMinutes < 5)
                        return;

                    _lastClearDeviceConfigFailedLogged = DateTime.Now;

                    Log.Warning(!_mqttClient.IsConnected
                        ? "[MQTT] Not connected, clearing device config failed"
                        : "[MQTT] Device config not found, clearing device config failed");
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while clearing device config: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Disconnect from the broker (if connected)
        /// </summary>
        public void Disconnect()
        {
            if (_mqttClient == null)
                return;

            if (IsConnected())
            {
                _mqttClient.InternalClient.DisconnectAsync();
                _mqttClient.Dispose();
            }

            Log.Information("[MQTT] Disconnected");
        }

        /// <summary>
        /// Subscribe to the provided command's topic
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SubscribeAsync(AbstractCommand command)
        {
            try
            {
                if (_mqttClient == null)
                    return;

                while (!IsConnected())
                    await Task.Delay(250);

                await _mqttClient.SubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Command_topic);
                await _mqttClient.SubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Action_topic);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while subscribing: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Unsubscribe from the provided command's topic
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task UnsubscribeAsync(AbstractCommand command)
        {
            try
            {
                if (_mqttClient == null)
                    return;

                while (!IsConnected())
                    await Task.Delay(250);

                await _mqttClient.UnsubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Command_topic);
                await _mqttClient.UnsubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Action_topic);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while unsubscribing: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Prepare the required settings for a MQTT client
        /// </summary>
        /// <returns></returns>
        private static ManagedMqttClientOptions? GetOptions()
        {
            if (string.IsNullOrEmpty(Variables.ServiceMqttSettings?.MqttAddress) || Variables.DeviceConfig == null)
                return null;

            // id can be random, but we'll store it for consistency (unless user-defined)
            if (string.IsNullOrEmpty(Variables.ServiceMqttSettings.MqttClientId))
            {
                //TODO: make sure that we don't use id which is already in use
                Variables.ServiceMqttSettings.MqttClientId = Guid.NewGuid().ToString()[..8];
                SettingsManager.StoreServiceSettings();
            }

            var clientOptionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(Variables.ServiceMqttSettings.MqttClientId)
                .WithCleanSession()
                .WithWillTopic($"{Variables.ServiceMqttSettings.MqttDiscoveryPrefix}/sensor/{Variables.DeviceConfig.Name}/availability")
                .WithWillPayload("offline")
                .WithWillRetain(Variables.ServiceMqttSettings.MqttUseRetainFlag)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(15));

            if (Variables.ServiceMqttSettings.MqttUseWebSocket)
            {
                clientOptionsBuilder.WithWebSocketServer(o => o.WithUri($"{Variables.ServiceMqttSettings.MqttAddress}:{Variables.ServiceMqttSettings.MqttPort}"));
                Log.Information("[MQTT] Using WebSocket for the connection");
            }
            else
            {
                clientOptionsBuilder.WithTcpServer(Variables.ServiceMqttSettings.MqttAddress, Variables.ServiceMqttSettings.MqttPort);
            }

            if (!string.IsNullOrEmpty(Variables.ServiceMqttSettings.MqttUsername))
                clientOptionsBuilder.WithCredentials(Variables.ServiceMqttSettings.MqttUsername, Variables.ServiceMqttSettings.MqttPassword);

            var certificates = new List<X509Certificate>();
            if (!string.IsNullOrEmpty(Variables.ServiceMqttSettings.MqttRootCertificate))
            {
                if (!File.Exists(Variables.ServiceMqttSettings.MqttRootCertificate))
                    Log.Error("[MQTT] Provided root certificate not found: {cert}", Variables.ServiceMqttSettings.MqttRootCertificate);
                else
                    certificates.Add(new X509Certificate2(Variables.ServiceMqttSettings.MqttRootCertificate));
            }

            if (!string.IsNullOrEmpty(Variables.ServiceMqttSettings.MqttClientCertificate))
            {
                if (!File.Exists(Variables.ServiceMqttSettings.MqttClientCertificate))
                    Log.Error("[MQTT] Provided client certificate not found: {cert}", Variables.ServiceMqttSettings.MqttClientCertificate);
                else
                    certificates.Add(new X509Certificate2(Variables.ServiceMqttSettings.MqttClientCertificate));
            }

            var clientTlsOptions = new MqttClientTlsOptions()
            {
                UseTls = Variables.ServiceMqttSettings.MqttUseTls,
                AllowUntrustedCertificates = Variables.ServiceMqttSettings.MqttAllowUntrustedCertificates,
                SslProtocol = Variables.ServiceMqttSettings.MqttUseTls ? SslProtocols.Tls12 : SslProtocols.None,
            };

            //TODO(Amadeo): add more granular control to the UI
            if (Variables.ServiceMqttSettings.MqttAllowUntrustedCertificates)
            {
                clientTlsOptions.IgnoreCertificateChainErrors = Variables.ServiceMqttSettings.MqttAllowUntrustedCertificates;
                clientTlsOptions.IgnoreCertificateRevocationErrors = Variables.ServiceMqttSettings.MqttAllowUntrustedCertificates;
                clientTlsOptions.CertificateValidationHandler = delegate (MqttClientCertificateValidationEventArgs _)
                {
                    return true;
                };
            }

            if (certificates.Count > 0)
                clientTlsOptions.ClientCertificatesProvider = new DefaultMqttCertificatesProvider(certificates);

            clientOptionsBuilder.WithTlsOptions(clientTlsOptions);
            clientOptionsBuilder.Build();

            return new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(clientOptionsBuilder).Build();
        }

        /// <summary>
        /// Handle incoming messages
        /// </summary>
        /// <param name="applicationMessage"></param>
        private Task HandleMessageReceived(MqttApplicationMessage applicationMessage)
        {
            try
            {
                if (!Variables.Commands.Any())
                    return Task.CompletedTask;

                foreach (var command in Variables.Commands)
                {
                    var commandConfig = (CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig();

                    if (commandConfig.Command_topic == applicationMessage.Topic)
                        HandleCommandReceived(applicationMessage, command);
                    else if (commandConfig.Action_topic == applicationMessage.Topic)
                        HandleActionReceived(applicationMessage, command);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while handling received message: {err}", ex.Message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles a command (on/off and variations)
        /// </summary>
        /// <param name="applicationMessage"></param>
        /// <param name="command"></param>
        private static void HandleCommandReceived(MqttApplicationMessage applicationMessage, AbstractCommand command)
        {
            try
            {
                var payload = Encoding.UTF8.GetString(applicationMessage.PayloadSegment).ToLower();
                if (string.IsNullOrWhiteSpace(payload))
                    return;

                if (payload.Contains("on"))
                {
                    command.TurnOn();
                }
                else if (payload.Contains("off"))
                {
                    command.TurnOff();
                }
                else
                {
                    switch (payload)
                    {
                        case "press":
                        case "lock":
                            command.TurnOn();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while handling received command: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Handles an action (on + argument)
        /// </summary>
        /// <param name="applicationMessage"></param>
        /// <param name="command"></param>
        private static void HandleActionReceived(MqttApplicationMessage applicationMessage, AbstractCommand command)
        {
            try
            {
                var payload = Encoding.UTF8.GetString(applicationMessage.PayloadSegment);
                if (string.IsNullOrWhiteSpace(payload))
                    return;

                command.TurnOnWithAction(payload);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while handling received action: {err}", ex.Message);
            }
        }
    }
}
