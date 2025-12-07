using System.Diagnostics;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using HASS.Agent.API;
using HASS.Agent.Enums;
using HASS.Agent.Functions;
using HASS.Agent.Managers;
using HASS.Agent.Media;
using HASS.Agent.Models.HomeAssistant;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Settings;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Managers.Audio;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Mqtt;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Exceptions;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace HASS.Agent.MQTT
{
    /// <summary>
    /// Handles publishing and processing HASS entities (commands and sensors) through MQTT
    /// </summary>
    public class MqttManager : IMqttManager
    {
        private IManagedMqttClient _mqttClient = null;

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
        public bool UseRetainFlag() => Variables.AppSettings?.MqttUseRetainFlag ?? true;

        /// <summary>
        /// Returns the default or configured discovery prefix
        /// </summary>
        /// <returns></returns>
        public string MqttDiscoveryPrefix() => Variables.AppSettings?.MqttDiscoveryPrefix ?? "homeassistant";

        /// <summary>
        /// Returns the device's config model
        /// </summary>
        /// <returns></returns>
        public DeviceConfigModel GetDeviceConfigModel() => Variables.DeviceConfig;

        /// <summary>
        /// Initialize the MQTT manager, establish a connection
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (!Variables.AppSettings.MqttEnabled)
                {
                    _status = MqttStatus.Disconnected;
                    Variables.MainForm?.SetMqttStatus(ComponentStatus.Stopped);
                    Log.Information("[MQTT] Disabled through settings");

                    return;
                }

                if (Variables.DeviceConfig == null)
                    CreateDeviceConfigModel();

                _mqttClient = Variables.MqttFactory.CreateManagedMqttClient();
                _mqttClient.ConnectedAsync += OnMqttConnected;
                _mqttClient.ConnectingFailedAsync += OnMqttConnectionFailed;
                _mqttClient.ApplicationMessageReceivedAsync += e => HandleMessageReceived(e.ApplicationMessage);
                _mqttClient.DisconnectedAsync += OnMqttDisconnected;
                //TODO(Amadeo): handle ApplicationMessageSkippedAsync?

                var options = GetOptions();
                if (options == null)
                {
                    _status = MqttStatus.ConfigMissing;
                    Variables.MainForm?.SetMqttStatus(ComponentStatus.Stopped);
                    Log.Warning("[MQTT] Configuration missing");

                    return;
                }

                Log.Information("[MQTT] Connecting ..");
                Variables.MainForm?.SetMqttStatus(ComponentStatus.Loading);
                StartClient(options);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while initializing: {err}", ex.Message);

                _status = MqttStatus.Error;
                Variables.MainForm?.SetMqttStatus(ComponentStatus.Failed);
                Variables.MainForm?.ShowToolTip(Languages.MqttManager_ToolTip_ConnectionError, true);
            }
        }

        /// <summary>
        /// Fires when the client gets disconnected from the broker
        /// </summary>
        /// <param name="e"></param>
        private async Task OnMqttDisconnected(MqttClientDisconnectedEventArgs arg)
        {
            _isReady = false;

            Variables.MainForm?.SetMqttStatus(ComponentStatus.Connecting);

            var gracePeriod = Variables.AppSettings.DisconnectedGracePeriodSeconds;

            // give the connection the grace period to recover
            var runningTimer = Stopwatch.StartNew();
            while (runningTimer.Elapsed.TotalSeconds < gracePeriod)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                if (IsConnected())
                {
                    _isReady = true;

                    if (_status == MqttStatus.Connected)
                        return;

                    _status = MqttStatus.Connected;
                    Variables.MainForm?.SetMqttStatus(ComponentStatus.Ok);
                    Log.Information("[MQTT] Reconnected from disconnection");

                    return;
                }

                if (Variables.AppSettings.MqttIgnoreGracePeriod)
                {
                    var lastResumed = SystemStateManager.LastEventOccurrence.TryGetValue(SystemStateEvent.Resume, out var lastResumeEventDate);
                    if (lastResumed && DateTime.Now < lastResumeEventDate.AddSeconds(gracePeriod))
                    {
                        Log.Information("[MQTT] System resumed less than {gracePeriod} seconds ago, ignoring grace period on disconnection");
                        break;
                    }
                }
            }

            // nope, call it
            _status = MqttStatus.Disconnected;
            Variables.MainForm?.SetMqttStatus(ComponentStatus.Stopped);

            // log if we're not shutting down, but only once
            if (Variables.ShuttingDown || _disconnectionNotified)
                return;

            _disconnectionNotified = true;

            Variables.MainForm?.ShowToolTip(Languages.MqttManager_ToolTip_Disconnected, true);
            Log.Warning("[MQTT] Disconnected: {reason}", arg.Reason.ToString());
        }

        /// <summary>
        /// Fires when connecting to the broker failed
        /// </summary>
        private async Task OnMqttConnectionFailed(ConnectingFailedEventArgs arg)
        {
            Variables.MainForm?.SetMqttStatus(ComponentStatus.Connecting);

            var gracePeriod = Variables.AppSettings.DisconnectedGracePeriodSeconds;

            // give the connection the grace period to recover
            var runningTimer = Stopwatch.StartNew();
            while (runningTimer.Elapsed.TotalSeconds < gracePeriod)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                if (IsConnected())
                {
                    // recovered
                    if (_status == MqttStatus.Connected)
                        return;

                    _status = MqttStatus.Connected;
                    Variables.MainForm?.SetMqttStatus(ComponentStatus.Ok);
                    Log.Information("[MQTT] Reconnected from failed connection");

                    return;
                }

                if (Variables.AppSettings.MqttIgnoreGracePeriod)
                {
                    var lastResumed = SystemStateManager.LastEventOccurrence.TryGetValue(SystemStateEvent.Resume, out var lastResumeEventDate);
                    if (lastResumed && DateTime.Now < lastResumeEventDate.AddSeconds(gracePeriod))
                    {
                        Log.Information("[MQTT] System resumed more than {gracePeriod} seconds ago, ignoring grace period on connection failed");
                        break;
                    }
                }
            }

            // nope, call it
            _status = MqttStatus.Error;
            Variables.MainForm?.SetMqttStatus(ComponentStatus.Failed);

            // log only once
            if (_connectingFailureNotified)
                return;

            _connectingFailureNotified = true;

            var excMsg = arg.Exception.ToString();
            if (excMsg.Contains("SocketException"))
                Log.Error("[MQTT] Error while connecting: {err}", arg.Exception.Message);
            else if (excMsg.Contains("MqttCommunicationTimedOutException"))
                Log.Error("[MQTT] Error while connecting: {err}", "Connection timed out");
            else if (excMsg.Contains("NotAuthorized"))
                Log.Error("[MQTT] Error while connecting: {err}", "Not authorized, check your credentials.");
            else
                Log.Fatal(arg.Exception, "[MQTT] Error while connecting: {err}", arg.Exception.Message);

            Variables.MainForm?.ShowToolTip(Languages.MqttManager_ToolTip_ConnectionFailed, true);
        }

        private Task OnMqttConnected(MqttClientConnectedEventArgs arg)
        {
            _status = MqttStatus.Connected;
            Variables.MainForm?.SetMqttStatus(ComponentStatus.Ok);
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
                if (!Variables.AppSettings.MqttEnabled)
                    return;

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
                Variables.MainForm?.SetMqttStatus(ComponentStatus.Connecting);
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
            if (!Variables.AppSettings.MqttEnabled || _mqttClient == null)
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
        /// Announce our general availability
        /// </summary>
        private async void InitialRegistration()
        {
            if (!Variables.AppSettings.MqttEnabled || _mqttClient == null)
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
            if (!Variables.AppSettings.MqttEnabled)
                return;

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

        /// <summary>
        /// Publishes the provided message
        /// </summary>
        private DateTime _lastPublishFailedLogged = DateTime.MinValue;
        public async Task<bool> PublishAsync(MqttApplicationMessage message)
        {
            if (!Variables.AppSettings.MqttEnabled)
                return false;

            try
            {
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


                // publish away
                //TODO(Amadeo): rethink usage of managed client/internal client since new version of MQTTnet doesn't return status of enqueued message
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

        /// <summary>
        /// Publish the provided autodiscovery config
        /// </summary>
        /// <param name="discoverable"></param>
        /// <param name="domain"></param>
        /// <param name="clearConfig"></param>
        /// <returns></returns>
        public async Task AnnounceAutoDiscoveryConfigAsync(AbstractDiscoverable discoverable, string domain, bool clearConfig = false)
        {
            if (!Variables.AppSettings.MqttEnabled || !IsConnected())
                return;

            try
            {
                if (string.IsNullOrEmpty(Variables.AppSettings.MqttDiscoveryPrefix))
                    Variables.AppSettings.MqttDiscoveryPrefix = "homeassistant";

                var topic = $"{Variables.AppSettings.MqttDiscoveryPrefix}/{domain}/{Variables.DeviceConfig.Name}/{discoverable.ObjectId}/config";

                var messageBuilder = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithRetainFlag(Variables.AppSettings.MqttUseRetainFlag);

                if (clearConfig)
                {
                    messageBuilder.WithPayload(Array.Empty<byte>());
                }
                else
                {
                    var payload = discoverable.GetAutoDiscoveryConfig();
                    if (discoverable.IgnoreAvailability)
                        payload.Availability_topic = null;

                    messageBuilder.WithPayload(JsonConvert.SerializeObject(payload, payload.GetType(), JsonSerializerSettings));
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

        /// <summary>
        /// Announce our availability
        /// </summary>
        private DateTime _lastAvailableAnnouncement = DateTime.MinValue;
        private DateTime _lastAvailableAnnouncementFailedLogged = DateTime.MinValue;

        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new ToLowerInvariantNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore,
            Converters = { new StringEnumConverter() }
        };

        public async Task AnnounceAvailabilityAsync(bool offline = false)
        {
            if (!Variables.AppSettings.MqttEnabled || !IsConnected())
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

                if (IsConnected())
                {
                    if (string.IsNullOrEmpty(Variables.AppSettings.MqttDiscoveryPrefix))
                        Variables.AppSettings.MqttDiscoveryPrefix = "homeassistant";

                    var messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"{Variables.AppSettings.MqttDiscoveryPrefix}/sensor/{Variables.DeviceConfig.Name}/availability")
                        .WithPayload(offline ? "offline" : "online")
                        .WithRetainFlag(Variables.AppSettings.MqttUseRetainFlag);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());

                    var integrationMsgBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/devices/{Variables.DeviceConfig.Name}")
                        .WithPayload(JsonConvert.SerializeObject(new
                        {
                            serial_number = Variables.SerialNumber,
                            device = Variables.DeviceConfig,
                            apis = new
                            {
                                notifications = Variables.AppSettings.NotificationsEnabled,
                                media_player = Variables.AppSettings.MediaPlayerEnabled
                            }
                        }, JsonSerializerSettings))
                        .WithRetainFlag(Variables.AppSettings.MqttUseRetainFlag);

                    await _mqttClient.InternalClient.PublishAsync(integrationMsgBuilder.Build());
                }
                else
                {
                    // only log failures once every 5 minutes to minimize log growth
                    if ((DateTime.Now - _lastAvailableAnnouncementFailedLogged).TotalMinutes < 5)
                        return;

                    _lastAvailableAnnouncementFailedLogged = DateTime.Now;
                    Log.Warning("[MQTT] Not connected, availability announcement dropped");
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[MQTT] Error while announcing availability: {err}", ex.Message);
            }
        }

        /// <summary>
        /// CLears the device config, removing the retained message
        /// </summary>
        /// <returns></returns>
        public async Task ClearDeviceConfigAsync()
        {
            if (!Variables.AppSettings.MqttEnabled)
                return;

            if (!IsConnected())
            {
                Log.Warning("[MQTT] Not connected, clearing device config failed");

                return;
            }

            try
            {
                if (IsConnected())
                {
                    if (string.IsNullOrEmpty(Variables.AppSettings.MqttDiscoveryPrefix))
                        Variables.AppSettings.MqttDiscoveryPrefix = "homeassistant";
                    var messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"{Variables.AppSettings.MqttDiscoveryPrefix}/sensor/{Variables.DeviceConfig.Name}/availability")
                        .WithPayload(Array.Empty<byte>())
                        .WithRetainFlag(false);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());

                    messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/devices/{Variables.DeviceConfig.Name}")
                        .WithPayload(Array.Empty<byte>())
                        .WithRetainFlag(false);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());

                    messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/media_player/{Variables.DeviceConfig.Name}")
                        .WithPayload(Array.Empty<byte>())
                        .WithRetainFlag(false);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());

                    messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/media_player/{Variables.DeviceConfig.Name}/thumbnail")
                        .WithPayload(Array.Empty<byte>())
                        .WithRetainFlag(false);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());

                    messageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/media_player/{Variables.DeviceConfig.Name}/state")
                        .WithPayload(Array.Empty<byte>())
                        .WithRetainFlag(false);

                    await _mqttClient.InternalClient.PublishAsync(messageBuilder.Build());
                }
                else
                {
                    Log.Warning("[MQTT] Not connected, clearing device config failed");
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
            if (!Variables.AppSettings.MqttEnabled)
                return;

            if (IsConnected())
            {
                _mqttClient?.InternalClient?.DisconnectAsync();
                _mqttClient?.Dispose();
            }

            Log.Information("[MQTT] Disconnected");
        }

        /// <summary>
        /// Subscribe to the provided command's command and action topic
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task SubscribeAsync(AbstractCommand command)
        {
            if (!Variables.AppSettings.MqttEnabled)
                return;

            while (!IsConnected())
                await Task.Delay(250);

            await _mqttClient.SubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Command_topic);
            await _mqttClient.SubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Action_topic);
        }

        /// <summary>
        /// Subscribe to the integration's notification topic
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeNotificationsAsync()
        {
            if (!Variables.AppSettings.MqttEnabled)
                return;

            while (!IsConnected())
                await Task.Delay(250);

            await _mqttClient.SubscribeAsync($"hass.agent/notifications/{HelperFunctions.GetConfiguredDeviceName()}");
        }

        /// <summary>
        /// Subscribe to the integration's mediaplayer topic
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeMediaCommandsAsync()
        {
            if (!Variables.AppSettings.MqttEnabled)
                return;

            while (!IsConnected())
                await Task.Delay(250);

            await _mqttClient.SubscribeAsync($"hass.agent/media_player/{HelperFunctions.GetConfiguredDeviceName()}/cmd");
        }

        /// <summary>
        /// Unsubscribe from the provided command's command and action topic
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task UnsubscribeAsync(AbstractCommand command)
        {
            if (!Variables.AppSettings.MqttEnabled)
                return;

            while (!IsConnected())
                await Task.Delay(250);

            await _mqttClient.UnsubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Command_topic);
            await _mqttClient.UnsubscribeAsync(((CommandDiscoveryConfigModel)command.GetAutoDiscoveryConfig()).Action_topic);
        }

        /// <summary>
        /// Prepare the required settings for a MQTT client
        /// </summary>
        /// <returns></returns>
        private static ManagedMqttClientOptions GetOptions()
        {
            if (string.IsNullOrEmpty(Variables.AppSettings.MqttAddress))
                return null;

            // id can be random, but we'll store it for consistency (unless user-defined)
            if (string.IsNullOrEmpty(Variables.AppSettings.MqttClientId))
            {
                //TODO: make sure that we don't use id which is already in use
                Variables.AppSettings.MqttClientId = Guid.NewGuid().ToString()[..8];
                SettingsManager.StoreAppSettings();
            }

            var clientOptionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(Variables.AppSettings.MqttClientId)
                .WithTcpServer(Variables.AppSettings.MqttAddress, Variables.AppSettings.MqttPort)
                .WithCleanSession()
                .WithWillTopic($"{Variables.AppSettings.MqttDiscoveryPrefix}/sensor/{Variables.DeviceConfig.Name}/availability")
                .WithWillPayload("offline")
                .WithWillRetain(Variables.AppSettings.MqttUseRetainFlag)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(15));

            if (!string.IsNullOrEmpty(Variables.AppSettings.MqttUsername))
                clientOptionsBuilder.WithCredentials(Variables.AppSettings.MqttUsername, Variables.AppSettings.MqttPassword);

            // Load CA cert for server validation
            X509Certificate2? caCert = null;
            if (!string.IsNullOrEmpty(Variables.AppSettings.MqttRootCertificate))
            {
                if (!File.Exists(Variables.AppSettings.MqttRootCertificate))
                    Log.Error("[MQTT] Provided root certificate not found: {cert}", Variables.AppSettings.MqttRootCertificate);
                else
                    caCert = new X509Certificate2(Variables.AppSettings.MqttRootCertificate);
            }

            var certificates = new List<X509Certificate>();
            if (!string.IsNullOrEmpty(Variables.AppSettings.MqttClientCertificate))
            {
                if (!File.Exists(Variables.AppSettings.MqttClientCertificate))
                    Log.Error("[MQTT] Provided client certificate not found: {cert}", Variables.AppSettings.MqttClientCertificate);
                else
                    certificates.Add(new X509Certificate2(Variables.AppSettings.MqttClientCertificate, "")); // P12, empty password
            }

            var clientTlsOptions = new MqttClientTlsOptions()
            {
                UseTls = Variables.AppSettings.MqttUseTls,
                AllowUntrustedCertificates = Variables.AppSettings.MqttAllowUntrustedCertificates,
                SslProtocol = Variables.AppSettings.MqttUseTls ? SslProtocols.Tls12 : SslProtocols.None,
            };

            //TODO(Amadeo): add more granular control to the UI
            if (caCert != null)
            {
                clientTlsOptions.CertificateValidationHandler = ctx =>
                {
                    var chain = new X509Chain();
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                    chain.ChainPolicy.TrustMode = X509ChainTrustMode.System;
                    chain.ChainPolicy.ExtraStore.Add(caCert);

                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

                    var serverCert = new X509Certificate2(ctx.Certificate);
                    var valid = chain.Build(serverCert);

                    if (!valid)
                    {
                        Log.Error("[MQTT] Server certificate validation failed. Chain errors: {errors}",
                            string.Join(", ", chain.ChainStatus
                                .Where(s => s.Status != X509ChainStatusFlags.NoError)
                                .Select(s => $"{s.Status}: {s.StatusInformation}")));
                        return false;
                    }
                    return true;
                };
            }
            else if (Variables.AppSettings.MqttAllowUntrustedCertificates)
            {
                clientTlsOptions.CertificateValidationHandler = ctx => true;
            }
            else
            {
                clientTlsOptions.CertificateValidationHandler = MqttClientDefaultCertificateValidationHandler.Handle;
            }

            if (certificates.Count > 0)
                clientTlsOptions.ClientCertificatesProvider = new DefaultMqttCertificatesProvider(certificates);

            clientOptionsBuilder.WithTlsOptions(clientTlsOptions);

            return new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(clientOptionsBuilder.Build())
                .Build();
        }

        /// <summary>
        /// Handle incoming messages
        /// </summary>
        /// <param name="applicationMessage"></param>
        private Task HandleMessageReceived(MqttApplicationMessage applicationMessage)
        {
            try
            {
                // process as a notification
                if (applicationMessage.Topic == $"hass.agent/notifications/{HelperFunctions.GetConfiguredDeviceName()}")
                {
                    var notification = JsonConvert.DeserializeObject<Notification>(Encoding.UTF8.GetString(applicationMessage.PayloadSegment), JsonSerializerSettings)!;
                    _ = Task.Run(() => NotificationManager.ShowNotification(notification));

                    return Task.CompletedTask;
                }

                // process as a mediaplyer command
                if (applicationMessage.Topic == $"hass.agent/media_player/{HelperFunctions.GetConfiguredDeviceName()}/cmd")
                {
                    var command = JsonConvert.DeserializeObject<MqttMediaPlayerCommand>(Encoding.UTF8.GetString(applicationMessage.PayloadSegment), JsonSerializerSettings)!;

                    switch (command.Command)
                    {
                        case MediaPlayerCommand.PlayMedia:
                            MediaManager.ProcessMedia(command.Data.ToObject<string>());
                            break;
                        case MediaPlayerCommand.Seek:
                            MediaManager.ProcessSeekCommand(TimeSpan.FromSeconds(command.Data.ToObject<double>()).Ticks);
                            break;
                        case MediaPlayerCommand.SetVolume:
                            MediaManagerCommands.SetVolume(command.Data.ToObject<int>());
                            break;
                        case MediaPlayerCommand.Mute:
                            //Note(Amadeo): tech debt :)
                            //              this logic should be in MediaManager.ProcessCommand but it's also used in "local api"
                            //              from all messy and time viable options, this one is the "cleanest"
                            if (command.Data == null)
                                goto default;

                            AudioManager.SetDefaultDeviceProperties(DeviceType.Output, DeviceRole.Multimedia | DeviceRole.Console, -1, command.Data.ToObject<bool>());
                            break;
                        default:
                            MediaManager.ProcessCommand(command.Command);
                            break;
                    }

                    return Task.CompletedTask;
                }

                // process as a hass.agent command if any
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
                Log.Fatal(ex, "[MQTT] Error while processing received message: {err}", ex.Message);
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

        /// <summary>
        /// Handles an action (on + argument)
        /// </summary>
        /// <param name="applicationMessage"></param>
        /// <param name="command"></param>
        private static void HandleActionReceived(MqttApplicationMessage applicationMessage, AbstractCommand command)
        {
            if (applicationMessage.PayloadSegment == null)
                return;

            var payload = Encoding.UTF8.GetString(applicationMessage.PayloadSegment);
            if (string.IsNullOrWhiteSpace(payload))
                return;

            command.TurnOnWithAction(payload);
        }
    }
}
