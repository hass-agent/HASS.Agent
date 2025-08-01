using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HASS.Agent.Shared;
using HASS.Agent.Shared.Models.Config.Service;
using HASS.Agent.Satellite.Service.Commands;
using HASS.Agent.Satellite.Service.Extensions;
using HASS.Agent.Satellite.Service.Sensors;
using HASS.Agent.Shared.Models.HomeAssistant;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Satellite.Service.Settings
{
    /// <summary>
    /// Handles loading and storing objects and settings
    /// </summary>
    internal class SettingsManager
    {
        /// <summary>
        /// Load all stored settings and objects
        /// </summary>
        /// <returns></returns>
        internal static async Task<bool> LoadAsync(bool createInitialSettings = true)
        {
            Log.Information("[SETTINGS] Config storage path: {path}", Variables.ConfigPath);

            // check config dir
            if (!Directory.Exists(Variables.ConfigPath))
            {
                // only create initial settings when asked
                if (!createInitialSettings)
                {
                    StoreInitialSettings(false);
                    StoreInitialMqttSettings(false);
                    return true;
                }

                // create dir
                Directory.CreateDirectory(Variables.ConfigPath);

                // create default config
                StoreInitialSettings();
                StoreInitialMqttSettings();

                // done
                return true;
            }

            var allGood = true;

            // load app settings
            var ok = LoadServiceSettings();
            if (!ok) allGood = false;

            // load mqtt settings
            ok = LoadServiceMqttSettings();
            if (!ok) allGood = false;

            // load commands
            ok = await StoredCommands.LoadAsync();
            if (!ok) allGood = false;

            // load sensors
            ok = await StoredSensors.LoadAsync();
            if (!ok) allGood = false;

            // done
            return allGood;
        }

        /// <summary>
        /// Load stored application settings
        /// </summary>
        /// <returns></returns>
        private static bool LoadServiceSettings()
        {
            try
            {
                if (!File.Exists(Variables.ServiceSettingsFile))
                {
                    // store default settings
                    StoreInitialSettings();

                    // done
                    return true;
                }

                var appSettingsRaw = File.ReadAllText(Variables.ServiceSettingsFile);
                if (string.IsNullOrWhiteSpace(appSettingsRaw)) return true;

                // get the global settings
                Variables.ServiceSettings = JsonConvert.DeserializeObject<ServiceSettings>(appSettingsRaw);
                if (Variables.ServiceSettings == null)
                {
                    // something went wrong, but we're not saving new ones, user might want to recover values
                    Log.Error("[SETTINGS] Error loading settings: returned null object");
                    return false;
                }

                // set shared config
                AgentSharedBase.SetDeviceName(Variables.ServiceSettings.DeviceName);
                AgentSharedBase.SetCustomExecutorBinary(Variables.ServiceSettings.CustomExecutorBinary);
                
                // done
                Log.Information("[SETTINGS] Configuration loaded");
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error loading app settings: {err}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Load stored application MQTT settings
        /// </summary>
        /// <returns></returns>
        private static bool LoadServiceMqttSettings()
        {
            try
            {
                if (!File.Exists(Variables.ServiceMqttSettingsFile))
                {
                    // store default settings
                    StoreInitialMqttSettings();

                    // done
                    return true;
                }

                var appSettingsRaw = File.ReadAllText(Variables.ServiceMqttSettingsFile);
                if (string.IsNullOrWhiteSpace(appSettingsRaw)) return true;

                // get the global settings
                Variables.ServiceMqttSettings = JsonConvert.DeserializeObject<ServiceMqttSettings>(appSettingsRaw);
                if (Variables.ServiceMqttSettings == null)
                {
                    // something went wrong, but we're not saving new ones, user might want to recover values
                    Log.Error("[SETTINGS] Error loading MQTT settings: returned null object");
                    return false;
                }

                // done
                Log.Information("[SETTINGS] MQTT configuration loaded");
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error loading app MQTT settings: {err}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Store default settings
        /// </summary>
        /// <returns></returns>
        private static bool StoreInitialSettings(bool storeSettings = true)
        {
            try
            {
                Log.Information("[SETTINGS] No config found, storing default settings");

                // empty collections
                Variables.Commands = new List<AbstractCommand>();
                Variables.SingleValueSensors = new List<AbstractSingleValueSensor>();

                // default settings
                Variables.ServiceSettings = new ServiceSettings();

                // store ?
                if (!storeSettings) return true;

                // jep
                var appSettings = JsonConvert.SerializeObject(Variables.ServiceSettings, Formatting.Indented);
                File.WriteAllText(Variables.ServiceSettingsFile, appSettings);

                // done
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error storing initial settings: {err}", ex.Message);
                return false;
            }
        }

        private static bool StoreInitialMqttSettings(bool storeSettings = true)
        {
            try
            {
                Log.Information("[SETTINGS] No MQTT config found, storing default settings");
                
                // default settings
                Variables.ServiceMqttSettings = new ServiceMqttSettings();

                // store ?
                if (!storeSettings) return true;

                // jep
                var appMqttSettings = JsonConvert.SerializeObject(Variables.ServiceMqttSettings, Formatting.Indented);
                File.WriteAllText(Variables.ServiceMqttSettingsFile, appMqttSettings);

                // done
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error storing initial MQTT settings: {err}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Store all settings and objects
        /// </summary>
        /// <returns></returns>
        internal static bool Store()
        {
            // start positive
            var allGood = true;

            // store app settings
            var ok = StoreServiceSettings();
            if (!ok) allGood = false;
            
            // store commands
            ok = StoredCommands.Store();
            if (!ok) allGood = false;

            // store sensors
            ok = StoredSensors.Store();
            if (!ok) allGood = false;

            // done
            return allGood;
        }

        /// <summary>
        /// Store and apply received application settings
        /// </summary>
        /// <returns></returns>
        internal static bool ProcessReceivedServiceSettings(ServiceSettings serviceSettings)
        {
            try
            {
                // we don't want to change the device name
                var deviceName = Variables.ServiceSettings?.DeviceName;

                // bind received settings
                Variables.ServiceSettings = serviceSettings;

                // reset device name
                Variables.ServiceSettings.DeviceName = deviceName;

                // set shared config
                AgentSharedBase.SetCustomExecutorBinary(Variables.ServiceSettings.CustomExecutorBinary);

                // store
                var stored = StoreServiceSettings();
                if (stored) Log.Information("[SETTINGS] Received settings stored");
                else Log.Error("[SETTINGS] Errors while storing settings");

                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error storing received settings: {err}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Store received MQTT settings
        /// </summary>
        /// <param name="serviceMqttSettings"></param>
        /// <returns></returns>
        internal static bool ProcessReceivedServiceMqttSettings(ServiceMqttSettings serviceMqttSettings)
        {
            try
            {
                if (Variables.ServiceMqttSettings != null)
                {
                    if (!serviceMqttSettings.SettingsChanged(Variables.ServiceMqttSettings))
                    {
                        Log.Information("[SETTINGS] Received MQTT settings are identical, nothing to do");
                        return true;
                    }
                }
                
                // bind received settings
                Variables.ServiceMqttSettings = serviceMqttSettings;
                
                // store
                var stored = StoreServiceSettings();
                if (stored) Log.Information("[SETTINGS] Received MQTT settings stored");
                else Log.Error("[SETTINGS] Errors while storing MQTT settings");

                // relaunch mqtt
                Variables.MqttManager.ReloadConfiguration();

                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error storing received MQTT settings: {err}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Changes the device's name internally and with HA
        /// </summary>
        /// <param name="deviceName"></param>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        internal static async void ProcessNameChange(string deviceName)
        {
            Variables.ServiceSettings ??= new ServiceSettings();

            if (deviceName == Variables.ServiceSettings.DeviceName)
            {
                Log.Information("[SETTINGS] Name change requested, but name is same as before, ignoring");
                return;
            }

            Log.Information("[SETTINGS] Processing name change to: {name}", deviceName);
            Log.Information("[SETTINGS] Previous name: {name}", Variables.ServiceSettings.DeviceName);

            // make sure the managers stop publishing
            SensorsManager.Pause();
            CommandsManager.Pause();

            // give the managers some time to stop
            await Task.Delay(250);

            // unpublish all entities
            await SensorsManager.UnpublishAllSensors();
            await CommandsManager.UnpublishAllCommands();

            // unpublish the current device
            await Variables.MqttManager.ClearDeviceConfigAsync();

            // give everything some time to process
            await Task.Delay(250);

            // set the name
            Variables.ServiceSettings.DeviceName = deviceName;

            // store
            var stored = StoreServiceSettings();
            if (stored) Log.Information("[SETTINGS] New name stored");
            else Log.Error("[SETTINGS] Errors while storing new name");

            // config shared functions
            AgentSharedBase.SetDeviceName(deviceName);

            // restart mqtt
            Variables.MqttManager.ReloadConfiguration();

            // wait while it's connecting
            var stopwatch = Stopwatch.StartNew();
            while (!Variables.MqttManager.IsConnected())
            {
                // check elapsed time
                if (stopwatch.Elapsed.TotalMinutes == 5)
                {
                    Log.Error("[SETTINGS] MQTT didn't reconnect within 5 minutes, unable to process name change");
                    return;
                }

                await Task.Delay(150);
            }

            // restart the managers
            SensorsManager.Resume();
            CommandsManager.Resume();

            // done
            Log.Information("[SETTINGS] Renaming completed");
        }

        /// <summary>
        /// Unpublishes & clears all entities
        /// </summary>
        internal static async void ClearAllEntities()
        {
            Log.Information("[SETTINGS] Unpublishing and clearing all entities ..");

            await ClearAllCommandsAsync();
            await ClearAllSensorsAsync();

            // done
            Log.Information("[SETTINGS] All entities cleared");
        }

        /// <summary>
        /// Unpublishes & clears all commands
        /// </summary>
        internal static async Task ClearAllCommandsAsync()
        {
            Log.Information("[SETTINGS] Unpublishing and clearing all commands ..");

            // make sure the manager stops publishing
            CommandsManager.Pause();

            // give the managers some time to stop
            await Task.Delay(250);

            // unpublish all entities
            await CommandsManager.UnpublishAllCommands();

            // clear entities
            Variables.Commands = new List<AbstractCommand>();

            // store
            StoredCommands.Store();

            // restart the manager
            CommandsManager.Resume();

            // done
            Log.Information("[SETTINGS] All commands cleared");
        }

        /// <summary>
        /// Unpublishes & clears all sensors
        /// </summary>
        internal static async Task ClearAllSensorsAsync()
        {
            Log.Information("[SETTINGS] Unpublishing and clearing all sensors ..");

            // make sure the manager stops publishing
            SensorsManager.Pause();

            // give the manager some time to stop
            await Task.Delay(250);

            // unpublish all entities
            await SensorsManager.UnpublishAllSensors();

            // clear entities
            Variables.SingleValueSensors = new List<AbstractSingleValueSensor>();
            Variables.MultiValueSensors = new List<AbstractMultiValueSensor>();

            // store
            StoredSensors.Store();

            // restart the manager
            SensorsManager.Resume();

            // done
            Log.Information("[SETTINGS] All sensors cleared");
        }

        /// <summary>
        /// Store current application settings
        /// </summary>
        /// <returns></returns>
        internal static bool StoreServiceSettings()
        {
            try
            {
                // check config dir
                if (!Directory.Exists(Variables.ConfigPath))
                {
                    // create
                    Directory.CreateDirectory(Variables.ConfigPath);
                }

                // serialize settings to file
                var appSettings = JsonConvert.SerializeObject(Variables.ServiceSettings, Formatting.Indented);
                File.WriteAllText(Variables.ServiceSettingsFile, appSettings);

                // serialize mqtt settings to file
                var appMqttSettings = JsonConvert.SerializeObject(Variables.ServiceMqttSettings, Formatting.Indented);
                File.WriteAllText(Variables.ServiceMqttSettingsFile, appMqttSettings);

                // done
                Log.Information("[SETTINGS] Configuration stored");
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error storing service settings: {err}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Stores the startup path to the registry
        /// </summary>
        internal static void StoreInstallPath()
        {
            try
            {
                Registry.SetValue(Variables.RootMachineRegKey, "InstallPath", Variables.StartupPath, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error storing install path: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Gets the 'extended logging' setting from registry
        /// </summary>
        /// <returns></returns>
        internal static bool GetExtendedLoggingSetting()
        {
            try
            {
                var setting = Registry.GetValue(Variables.RootHassAgentRegKey, "ExtendedLogging", "0");
                if (setting == null) return false;

                var extendedLoggingSetting = (string)setting;
                if (string.IsNullOrEmpty(extendedLoggingSetting)) return false;
                return extendedLoggingSetting == "1";
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SETTINGS] Error retrieving extended logging setting: {err}", ex.Message);
                return false;
            }
        }
    }
}
