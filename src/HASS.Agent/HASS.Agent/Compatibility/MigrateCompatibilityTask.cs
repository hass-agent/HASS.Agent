using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using HASS.Agent.Commands;
using HASS.Agent.Extensions;
using HASS.Agent.Functions;
using HASS.Agent.MQTT;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Service;
using HASS.Agent.Settings;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.Config;
using HASS.Agent.Shared.Models.Config.Old;
using HASS.Agent.Shared.Models.HomeAssistant;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;


namespace HASS.Agent.Compatibility
{
    internal class MigrateCompatibilityTask : ICompatibilityTask
    {
        internal const string OldHASSAgentFolder = "LAB02 Research";
        internal const string OldServiceFolder = "HASS.Agent Satellite Service";
        internal const string OldClientFolder = "HASS.Agent";
        internal const string OldServiceName = "HASS.Agent Satellite Service";

        public string Name => Languages.Compat_MigrateTask_Name;

        private void MigrateConfig(string source, string destination)
        {
            if (string.IsNullOrEmpty(destination))
            {
                Log.Information("[COMPATTASK] Destination is not valid");
                return;
            }

            var oldConfigDir = new DirectoryInfo(Path.Combine(source, "config"));
            if (!oldConfigDir.Exists)
            {
                Log.Information("[COMPATTASK] No '{src}' configuration folder found", source);
                return;
            }

            var oldConfigFiles = oldConfigDir.GetFiles();
            if (!oldConfigFiles.Any())
            {
                Log.Information("[COMPATTASK] No configuration files available for migration");
                return;
            }

            Directory.CreateDirectory(destination);

            foreach (var file in oldConfigFiles)
            {
                Log.Information("[COMPATTASK] Migrating {fn}", file.FullName);
                file.CopyTo(Path.Combine(destination, file.Name), true);
            }

            Log.Information("[COMPATTASK] Configuration migrated");
        }

        private void MigrateServiceConfig()
        {
            Log.Information("[COMPATTASK] Service configuration migration start");

            var oldServiceInstallationPath = ServiceManager.GetInstallPath(OldHASSAgentFolder, OldServiceFolder, true);
            if (string.IsNullOrWhiteSpace(oldServiceInstallationPath))
            {
                Log.Information("[COMPATTASK] No service installation folder found");
                return;
            }

            var configPath = Path.Combine(ServiceManager.GetInstallPath(), "config");

            MigrateConfig(oldServiceInstallationPath, configPath);

            MigrateSensors(Path.Combine(configPath, Path.GetFileName(Variables.SensorsFile)));
            MigrateCommands(Path.Combine(configPath, Path.GetFileName(Variables.CommandsFile)));

            Log.Information("[COMPATTASK] Service configuration migration end");
        }

        private void MigrateClientConfig()
        {
            Log.Information("[COMPATTASK] Client configuration migration start");

            var oldClientInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), OldHASSAgentFolder, OldClientFolder);
            if (string.IsNullOrWhiteSpace(oldClientInstallPath))
            {
                Log.Information("[COMPATTASK] No client installation folder found");
                return;
            }

            MigrateConfig(oldClientInstallPath, Variables.ConfigPath);

            MigrateSensors(Variables.SensorsFile);
            MigrateCommands(Variables.CommandsFile);

            Log.Information("[COMPATTASK] Client configuration migration end");
        }

        private void MigrateRegistryConfig()
        {
            //from HKEY_CURRENT_USER\SOFTWARE\LAB02Research\HASSAgent
            //to   HKEY_CURRENT_USER\SOFTWARE\HASSAgent\Client"

            var source = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\LAB02Research\HASSAgent");
            var destination = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\HASSAgent\Client", true);
            if(source == null)
            {
                Log.Information("[COMPATTASK] No source registry keys found");
                return;
            }

            CopyRegistryKey(source, destination);
        }

        private void StopOriginalInstances()
        {
            try
            {
                var hassAgentProcess = Process.GetProcessesByName("HASS.Agent")
                    .FirstOrDefault(p => p.MainModule.FileName.Contains("LAB02 Research"));
                if (hassAgentProcess != null)
                {
                    Log.Information("[COMPATTASK] Detected running instance of original HASS.Agent, stopping");
                    hassAgentProcess.Kill();
                }

                using var service = new ServiceController(OldServiceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    Log.Information("[COMPATTASK] Detected running instance of HASS.Agent Satellite Service, stopping and setting startup type to manual");
                    service.Stop();
                    ServiceHelper.ChangeStartMode(service, ServiceStartMode.Manual, out var error);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[COMPATTASK] There was an issue stopping original instances of HASS.Agent: {ex}", ex);
            }
        }

        private List<ConfiguredSensor> GetConfiguredSensors(string jsonData)
        {
            List<ConfiguredSensor> sensors = null;
            if (ConfiguredSensorLAB02.InJsonData(jsonData))
            {
                Log.Warning("[COMPATTASK] Migrating sensors from LAB02 version");
                var lab02Sensors = JsonConvert.DeserializeObject<List<ConfiguredSensorLAB02>>(jsonData);
                sensors = lab02Sensors.Select(s => ConfiguredSensor.FromLAB02(s)).ToList();
            }
            else if (ConfiguredSensor2023Beta.InJsonData(jsonData))
            {
                Log.Warning("[COMPATTASK] Migrating sensors from 2023Beta version");
                var beta2023Sensors = JsonConvert.DeserializeObject<List<ConfiguredSensor2023Beta>>(jsonData);
                sensors = beta2023Sensors.Select(s => ConfiguredSensor.From2023Beta(s)).ToList();
            }
            else
            {
                Log.Warning("[COMPATTASK] Cannot identify sensors version");
            }

            return sensors;
        }

        private void MigrateSensors(string sensorsFile)
        {
            try
            {
                if (!File.Exists(sensorsFile))
                {
                    Log.Warning("[COMPATTASK] Sensors configuration file '{sf}' does not exit", sensorsFile);
                    return;
                }

                var sensorsRawIn = File.ReadAllText(sensorsFile);
                if (string.IsNullOrWhiteSpace(sensorsRawIn))
                {
                    Log.Warning("[COMPATTASK] Sensors configuration file is empty");
                    return;
                }

                var configuredSensors = GetConfiguredSensors(sensorsRawIn);
                if (configuredSensors == null)
                    return;

                var sensorsRawOut = JsonConvert.SerializeObject(configuredSensors, Formatting.Indented);
                File.WriteAllText(sensorsFile, sensorsRawOut);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMPATTASK] Error migrating sensors: {err}", ex.Message);
                return;
            }

            Log.Information("[COMPATTASK] Sensors configuration migrated");
        }

        private List<ConfiguredCommand> GetConfiguredCommands(string jsonData)
        {
            List<ConfiguredCommand> commands = null;
            if (ConfiguredCommandLAB02.InJsonData(jsonData))
            {
                Log.Warning("[COMPATTASK] Migrating sensors from LAB02 version");
                var lab02Commands = JsonConvert.DeserializeObject<List<ConfiguredCommandLAB02>>(jsonData);
                commands = lab02Commands.Select(c => ConfiguredCommand.FromLAB02(c)).ToList();
            }
            else if (ConfiguredCommand2023Beta.InJsonData(jsonData))
            {
                Log.Warning("[COMPATTASK] Migrating sensors from 2023Beta version");
                var beta2023Commands = JsonConvert.DeserializeObject<List<ConfiguredCommand2023Beta>>(jsonData);
                commands = beta2023Commands.Select(c => ConfiguredCommand.From2023Beta(c)).ToList();
            }
            else
            {
                Log.Warning("[COMPATTASK] Cannot identify sensors version");
            }

            return commands;
        }

        private void MigrateCommands(string commandsFile)
        {
            try
            {
                if (!File.Exists(commandsFile))
                {
                    Log.Warning("[COMPATTASK] Commands configuration file '{cf}' does not exit", commandsFile);
                    return;
                }

                var commandsRawIn = File.ReadAllText(commandsFile);
                if (string.IsNullOrWhiteSpace(commandsRawIn))
                {
                    Log.Warning("[COMPATTASK] Commands configuration file is empty");
                    return;
                }

                var configuredCommands = GetConfiguredCommands(commandsRawIn);
                if (configuredCommands == null)
                    return;

                var commandsRawOut = JsonConvert.SerializeObject(configuredCommands, Formatting.Indented);
                File.WriteAllText(commandsFile, commandsRawOut);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMPATTASK] Error migrating commands: {err}", ex.Message);
                return;
            }

            Log.Information("[COMPATTASK] Commands configuration migrated");
        }

        public async Task<(bool, string)> Perform()
        {
            try
            {
                var errorMessage = string.Empty;

                Log.Information("[COMPATTASK] Migration name compatibility task started");

                StopOriginalInstances();

                MigrateServiceConfig();
                MigrateClientConfig();
                MigrateRegistryConfig();

                //HelperFunctions.Restart(true);

                return string.IsNullOrWhiteSpace(errorMessage) ? (true, string.Empty) : (false, errorMessage);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMPATTASK] Error performing migration compatibility task: {err}", ex.Message);
                return (false, Languages.Compat_Error_CheckLogs);
            }
        }

        private void CopyRegistryKey(RegistryKey source, RegistryKey destination)
        {
            foreach (var valueName in source.GetValueNames())
                destination.SetValue(valueName, source.GetValue(valueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames), source.GetValueKind(valueName));

            foreach (var name in source.GetSubKeyNames())
            {
                using var sourceSubKey = source.OpenSubKey(name, false);
                var destinationSubKey = destination.CreateSubKey(name);
                CopyRegistryKey(sourceSubKey, destinationSubKey);
            }
        }
    }
}
