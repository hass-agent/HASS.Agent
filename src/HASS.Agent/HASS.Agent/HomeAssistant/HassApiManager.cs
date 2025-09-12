using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using HADotNet.Core;
using HADotNet.Core.Clients;
using HASS.Agent.Enums;
using HASS.Agent.Extensions;
using HASS.Agent.Functions;
using HASS.Agent.Models.HomeAssistant;
using HASS.Agent.Models.Internal;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Sensors;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using Newtonsoft.Json;
using Serilog;
using static Grpc.Core.Metadata;

namespace HASS.Agent.HomeAssistant
{
    /// <summary>
    /// Uses HASS's API to fetch entities, their status and to execute actions (on, off, etc) 
    /// </summary>
    internal static class HassApiManager
    {
        private static ConfigClient _configClient;
        private static ServiceClient _serviceClient;
        private static EntityClient _entityClient;
        private static StatesClient _statesClient;
        private static EventClient _eventClient;

        internal static HassManagerStatus ManagerStatus = HassManagerStatus.Initialising;

        internal static List<string> AutomationList = new();
        internal static List<string> ScriptList = new();
        internal static List<string> InputBooleanList = new();
        internal static List<string> SceneList = new();
        internal static List<string> SwitchList = new();
        internal static List<string> LightList = new();
        internal static List<string> CoverList = new();
        internal static List<string> ClimateList = new();
        internal static List<string> MediaPlayerList = new();
        internal static List<string> ButtonList = new();
        internal static List<string> InputButtonList = new();

        private static readonly string[] OnStates = { "on", "playing", "open", "opening" };
        private static readonly string[] OffStates = { "off", "idle", "paused", "stopped", "closed", "closing" };

        private static readonly List<string> NotFoundEntities = new();

        private static readonly SemaphoreSlim ConfigCheckSemaphore = new(1, 1);

        public static string HaVersion { get; private set; } = string.Empty;

        /// <summary>
        /// Initializes the HASS API manager, establishes a connection and loads the entities
        /// </summary>
        /// <returns></returns>
        internal static async Task<HassManagerStatus> InitializeAsync()
        {
            try
            {
                if (!CheckSettings())
                {
                    ManagerStatus = HassManagerStatus.ConfigMissing;
                    Variables.MainForm?.SetHassApiStatus(ComponentStatus.Stopped);

                    return ManagerStatus;
                }

                var clientInitialized = InitializeClient();
                if (!clientInitialized)
                {
                    Variables.MainForm?.SetHassApiStatus(ComponentStatus.Failed);
                    ManagerStatus = HassManagerStatus.Failed;

                    Variables.MainForm?.ShowToolTip(Languages.HassApiManager_ToolTip_ConnectionSetupFailed, true);

                    return ManagerStatus;
                }

                var firstAttempt = true;
                while (!await GetConfig())
                {
                    if (firstAttempt)
                    {
                        Variables.MainForm?.ShowToolTip(Languages.HassApiManager_ToolTip_InitialConnectionFailed, true);
                        firstAttempt = false;
                    }

                    if (Variables.ShuttingDown)
                        return HassManagerStatus.Failed;

                    await Task.Delay(150);

                    // reset state to let the user know we're trying
                    Variables.MainForm?.SetHassApiStatus(ComponentStatus.Connecting);
                    ManagerStatus = HassManagerStatus.Initialising;
                }

                _serviceClient = ClientFactory.GetClient<ServiceClient>();
                _entityClient = ClientFactory.GetClient<EntityClient>();
                _statesClient = ClientFactory.GetClient<StatesClient>();
                _eventClient = ClientFactory.GetClient<EventClient>();

                ManagerStatus = HassManagerStatus.LoadingData;
                await LoadEntitiesAsync();

                _ = Task.Run(PeriodicStatusUpdates);
                _ = Task.Run(PeriodicEntityReload);

                Log.Information("[HASS_API] System connected with {ip}", Variables.AppSettings.HassUri);
                Variables.MainForm?.SetHassApiStatus(ComponentStatus.Ok);

                ManagerStatus = HassManagerStatus.Ready;

                return ManagerStatus;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[HASS_API] Error while initializing: {err}", ex.Message);

                Variables.MainForm?.SetHassApiStatus(ComponentStatus.Failed);
                ManagerStatus = HassManagerStatus.Failed;

                Variables.MainForm?.ShowToolTip(Languages.HassApiManager_ToolTip_ConnectionFailed, true);

                return ManagerStatus;
            }
        }

        /// <summary>
        /// Initializes the HA API client, optionally using provided certificate config
        /// </summary>
        /// <returns></returns>
        private static bool InitializeClient()
        {
            try
            {
                var hassUri = new Uri(Variables.AppSettings.HassUri);

                var handler = new HttpClientHandler();
                if (Variables.AppSettings.HassAutoClientCertificate)
                {
                    Log.Information("[HASS_API] Connecting using automatic client certificate selection");
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                }
                else if (!string.IsNullOrEmpty(Variables.AppSettings.HassClientCertificate))
                {
                    if (!File.Exists(Variables.AppSettings.HassClientCertificate))
                    {
                        Log.Error("[HASS_API] The specified certificate isn't found: {cert}", Variables.AppSettings.HassClientCertificate);

                        return false;
                    }

                    var certFile = Path.GetFileName(Variables.AppSettings.HassClientCertificate);
                    Log.Information("[HASS_API] Connecting using client certificate: {cert}", certFile);

                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Add(new X509Certificate2(Variables.AppSettings.HassClientCertificate!));
                }

                if (Variables.AppSettings.HassAllowUntrustedCertificates)
                {
                    handler.CheckCertificateRevocationList = false;
                    handler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
                }

                ClientFactory.Initialize(hassUri, Variables.AppSettings.HassToken, handler);

                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[HASS_API] Error while initializing client: {err}", ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Fetches Home Assistant's config, will keep retrying in a 60 seconds period
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> GetConfig()
        {
            var runningTimer = Stopwatch.StartNew();
            Exception err = null;

            if (!ClientFactory.IsInitialized)
            {
                InitializeClient();

                _serviceClient ??= ClientFactory.GetClient<ServiceClient>();
                _entityClient ??= ClientFactory.GetClient<EntityClient>();
                _statesClient ??= ClientFactory.GetClient<StatesClient>();
            }

            _configClient ??= ClientFactory.GetClient<ConfigClient>();

            while (runningTimer.Elapsed.TotalSeconds < Variables.AppSettings.DisconnectedGracePeriodSeconds)
            {
                try
                {
                    var config = await _configClient.GetConfiguration();

                    // if we're here, the connection works
                    if (config.Version == HaVersion)
                        return true;

                    // version changed since last check (or this is the first check), log
                    HaVersion = config.Version;
                    Log.Information("[HASS_API] Home Assistant version: {version}", config.Version);

                    return true;
                }
                catch (Exception ex)
                {
                    if (err == null)
                    {
                        // set state to loading on the first failed attempt
                        Variables.MainForm?.SetHassApiStatus(ComponentStatus.Connecting);
                        ManagerStatus = HassManagerStatus.Initialising;
                    }

                    err = ex;

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            ClientFactory.Reset();

            _configClient = null;
            _serviceClient = null;
            _entityClient = null;
            _statesClient = null;

            Variables.MainForm?.SetHassApiStatus(ComponentStatus.Failed);
            ManagerStatus = HassManagerStatus.Failed;

            if (err != null)
                Log.Fatal("[HASS_API] Error while fetching HA config: {err}", err.Message);
            else
                Log.Error("[HASS_API] Error while fetching HA config: timeout");

            return false;
        }

        /// <summary>
        /// Checks if the connection's working, if not, will retry for max 60 seconds through GetConfig()
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> CheckConnectionAsync()
        {
            await ConfigCheckSemaphore.WaitAsync();

            try
            {
                if (!await GetConfig())
                    return false;

                if (ManagerStatus != HassManagerStatus.Ready)
                {
                    _serviceClient = ClientFactory.GetClient<ServiceClient>();
                    _entityClient = ClientFactory.GetClient<EntityClient>();
                    _statesClient = ClientFactory.GetClient<StatesClient>();

                    ManagerStatus = HassManagerStatus.Ready;
                    Variables.MainForm?.SetHassApiStatus(ComponentStatus.Ok);

                    Log.Information("[HASS_API] Server recovered from failed state");

                    SensorsManager.ResetAllSensorChecks();
                }

                return true;
            }
            finally
            {
                ConfigCheckSemaphore.Release();
            }
        }

        /// <summary>
        /// Checks the provided provided credentials to see if we can connect
        /// <para>This will disconnect any current connections!</para>
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="apiKey"></param>
        /// <param name="automaticClientCertificate"></param>
        /// <param name="allowUntrustedCertificates"></param>
        /// <param name="clientCertificate"></param>
        /// <returns></returns>
        internal static async Task<(bool success, string message)> CheckHassConfigAsync(string uri, string apiKey, bool automaticClientCertificate = false, bool allowUntrustedCertificates = true, string clientCertificate = "")
        {
            try
            {
                if (ClientFactory.IsInitialized)
                    ClientFactory.Reset();

                var handler = new HttpClientHandler();

                if (automaticClientCertificate)
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                }
                else if (!string.IsNullOrEmpty(clientCertificate))
                {
                    if (!File.Exists(clientCertificate))
                        return (false, Languages.HassApiManager_CheckHassConfig_CertNotFound);

                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Add(new X509Certificate2(clientCertificate));
                }

                if (allowUntrustedCertificates)
                {
                    handler.CheckCertificateRevocationList = false;
                    handler.ServerCertificateCustomValidationCallback += (_, _, _, _) => true;
                }

                ClientFactory.Initialize(uri, apiKey, handler);

                if (!ClientFactory.IsInitialized)
                    return (false, Languages.HassApiManager_CheckHassConfig_UnableToConnect);

                _configClient = ClientFactory.GetClient<ConfigClient>();
                var config = await _configClient.GetConfiguration();

                return config == null ? (false, Languages.HassApiManager_CheckHassConfig_ConfigFailed) : (true, config.Version);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[HASS_API] Error while checking config: {err}", ex.Message);

                return (false, Languages.HassApiManager_ConnectionFailed);
            }
            finally
            {
                // reset if we're initialized
                if (ClientFactory.IsInitialized)
                    ClientFactory.Reset();
            }
        }

        /// <summary>
        /// Check whether the HASS config is available
        /// </summary>
        /// <returns></returns>
        private static bool CheckSettings()
        {
            // todo: check data values
            return !string.IsNullOrEmpty(Variables.AppSettings.HassUri)
                && !string.IsNullOrEmpty(Variables.AppSettings.HassToken);
        }

        /// <summary>
        /// Loads entities of given domainName to the entityList
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="entityList"></param>
        /// <returns></returns>
        private static async Task LoadDomain(string domainName, List<string> entityList)
        {
            var entities = await _entityClient.GetEntities(domainName);
            foreach (var entity in entities)
            {
                entityList.Add(entity.Remove(0, domainName.Length + 1));
            }
        }

        /// <summary>
        /// Fetches all entities from HASS
        /// </summary>
        /// <returns></returns>
        private static async Task LoadEntitiesAsync(bool clearCurrent = false)
        {
            if (clearCurrent)
            {
                AutomationList.Clear();
                ScriptList.Clear();
                InputBooleanList.Clear();
                SceneList.Clear();
                SwitchList.Clear();
                LightList.Clear();
                CoverList.Clear();
                ClimateList.Clear();
                MediaPlayerList.Clear();
                ButtonList.Clear();
            }

            try
            {
                await LoadDomain("automation", AutomationList);
                await LoadDomain("script", ScriptList);
                await LoadDomain("input_boolean", InputBooleanList);
                await LoadDomain("scene", SceneList);
                await LoadDomain("switch", SwitchList);
                await LoadDomain("light", LightList);
                await LoadDomain("cover", CoverList);
                await LoadDomain("climate", ClimateList);
                await LoadDomain("media_player", MediaPlayerList);
                await LoadDomain("button", ButtonList);
                await LoadDomain("input_button", InputButtonList);

                if (ManagerStatus != HassManagerStatus.Failed)
                    return;

                // reset failed state and log
                ManagerStatus = HassManagerStatus.Ready;
                Variables.MainForm?.SetHassApiStatus(ComponentStatus.Ok);

                Log.Information("[HASS_API] Server recovered from failed state");
            }
            catch (Exception ex)
            {
                // only log errors once to prevent log spamming
                if (ManagerStatus == HassManagerStatus.Failed)
                    return;

                // set failed state and log
                Variables.MainForm?.SetHassApiStatus(ComponentStatus.Failed);
                ManagerStatus = HassManagerStatus.Failed;

                Log.Error("[HASS_API] Error while reloading entities: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Executes the quick action
        /// </summary>
        /// <param name="quickAction"></param>
        /// <returns></returns>
        internal static async Task ProcessQuickActionAsync(QuickAction quickAction)
        {
            await ProcessActionAsync(quickAction.ToHassEntity(), quickAction.Action);
        }

        /// <summary>
        /// Executes the desired action on the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        internal static async Task<bool> ProcessActionAsync(HassEntity entity, HassAction action)
        {
            var actionVal = action.GetCategory();
            var domainVal = entity.Domain.GetCategory();
            var entityVal = entity.Entity.ToLower();

            // ugly fix until new QA system
            if (entity.Domain == HassDomain.Cover && action == HassAction.Stop)
                actionVal = "stop_cover";

            try
            {
                if (_statesClient == null)
                {
                    Log.Error("[HASS_API] [{domain}.{entity}] Unable to execute action, states client not initialized", domainVal, entityVal);
                    Variables.MainForm?.ShowToolTip(Languages.HassApiManager_ToolTip_QuickActionFailed, true);

                    return false;
                }

                Log.Information("[HASS_API] [{domain}.{entity}] Performing action: {action}", domainVal, entityVal, actionVal);

                var fullEntity = $"{domainVal}.{entityVal}";

                if (action == HassAction.Toggle)
                {
                    var state = await _statesClient.GetState(fullEntity);

                    if (OnStates.Contains(state.State))
                    {
                        Log.Information("[HASS_API] [{domain}.{entity}] Entity currently ON, changing action to 'turn_off'", domainVal, entityVal);
                        action = HassAction.Off;
                        actionVal = action.GetCategory();
                    }
                    else if (OffStates.Contains(state.State))
                    {
                        Log.Information("[HASS_API] [{domain}.{entity}] Entity currently OFF, changing action to 'turn_on'", domainVal, entityVal);
                        action = HassAction.On;
                        actionVal = action.GetCategory();
                    }
                    else
                    {
                        Log.Information("[HASS_API] [{domain}.{entity}] Entity in unknown state ({state}), defaulting to 'turn_on'", domainVal, entityVal, state.State);
                        action = HassAction.On;
                        actionVal = action.GetCategory();
                    }
                }

                var service = DetermineServiceForDomain(entity.Domain, action);
                _ = await _serviceClient.CallService(service, $@"{{""entity_id"":""{fullEntity}""}}");

                Log.Information("[HASS_API] [{domain}.{entity}] Action completed: {action}", domainVal, entityVal, actionVal);

                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("code 404"))
                {
                    Log.Error("[HASS_API] [{domain}.{entity}] Error while processing action: entity not found", domainVal, entityVal);
                    Variables.MainForm?.ShowToolTip(Languages.HassApiManager_ToolTip_QuickActionFailedOnEntity, true);

                    return false;
                }

                Log.Fatal(ex, "[HASS_API] [{domain}.{entity}] Error while processing action: {ex}", domainVal, entityVal, ex.Message);
                Variables.MainForm?.ShowToolTip(Languages.HassApiManager_ToolTip_QuickActionFailed, true);

                return false;
            }
        }

        /// <summary>
        /// Periodically fetches all entities
        /// </summary>
        private static async void PeriodicEntityReload()
        {
            while (!Variables.ShuttingDown)
            {
                await Task.Delay(TimeSpan.FromMinutes(5));

                if (!await CheckConnectionAsync())
                    continue;

                await LoadEntitiesAsync(true);
            }
        }

        /// <summary>
        /// Periodically gets the status of all the QuickActions
        /// <para>If we don't do this, it takes 10 seconds to get the state after idling a while</para>
        /// </summary>
        private static async void PeriodicStatusUpdates()
        {
            while (!Variables.ShuttingDown)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));

                if (!await CheckConnectionAsync())
                    continue;

                foreach (var quickAction in Variables.QuickActions)
                {
                    try
                    {
                        if (quickAction.Domain == HassDomain.HASSAgentCommands)
                            continue;

                        var entity = quickAction.ToHassEntity();

                        var domainVal = entity.Domain.GetCategory();
                        var entityVal = entity.Entity.ToLower();
                        var fullEntity = $"{domainVal}.{entityVal}";

                        _ = await _statesClient.GetState(fullEntity);

                        if (Variables.ShuttingDown)
                            return;
                        if (ManagerStatus != HassManagerStatus.Failed)
                            continue;

                        ManagerStatus = HassManagerStatus.Ready;
                        Variables.MainForm?.SetHassApiStatus(ComponentStatus.Ok);

                        Log.Information("[HASS_API] Server recovered from failed state");
                    }
                    catch (HttpRequestException ex)
                    {
                        if (Variables.ShuttingDown)
                            return;

                        if (ex.Message.Contains("404"))
                        {
                            var notFoundEntity = $"{quickAction.Domain.ToString().ToLower()}.{quickAction.Entity.ToLower()}";

                            // log only once
                            if (NotFoundEntities.Contains(notFoundEntity))
                                continue;

                            NotFoundEntities.Add(notFoundEntity);

                            Log.Warning("[HASS_API] Server returned 404 (not found) while getting entity state. This can happen after a server reboot, or if you've deleted the entity. If the problem persists, please file a ticket on github.\r\nEntity: {entity}\r\nError message: {err}", notFoundEntity, ex.Message);

                            continue;
                        }

                        // only log errors once to prevent log spamming
                        if (ManagerStatus == HassManagerStatus.Failed)
                            continue;

                        // set failed state and log
                        Variables.MainForm?.SetHassApiStatus(ComponentStatus.Failed);
                        ManagerStatus = HassManagerStatus.Failed;

                        Log.Error("[HASS_API] HTTP error while getting periodic status update: {err}", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        if (Variables.ShuttingDown)
                            return;
                        if (ManagerStatus == HassManagerStatus.Failed)
                            continue;

                        // set failed state and log
                        Variables.MainForm?.SetHassApiStatus(ComponentStatus.Failed);
                        ManagerStatus = HassManagerStatus.Failed;

                        Log.Fatal(ex, "[HASS_API] Error while getting periodic status update: {err}", ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Determines the full service call for the domain and action
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string DetermineServiceForDomain(HassDomain domain, HassAction action)
        {
            var domainValue = domain.GetCategory();

            // attempt to fix some impossible settings
            action = domain switch
            {
                HassDomain.Cover when action == HassAction.On => HassAction.Open,
                HassDomain.Cover when action == HassAction.Off => HassAction.Close,
                HassDomain.MediaPlayer when action == HassAction.On => HassAction.Play,
                HassDomain.MediaPlayer when action == HassAction.Off => HassAction.Stop,
                _ => action
            };

            var actionValue = action.GetCategory();

            // ugly fix until new QA system
            if (domain == HassDomain.Cover && action == HassAction.Stop)
                actionValue = "stop_cover";

            return $"{domainValue}.{actionValue}";
        }

        /// <summary>
        /// Fires an event for the specified type and payload
        /// </summary>
        /// <param name="type"></param>
        /// <param name="payload"></param>
        public static async Task FireEvent(string type, object payload)
        {
            await _eventClient.FireEvent(type, payload);
        }
    }
}
