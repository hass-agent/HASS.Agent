using HASS.Agent.Commands;
using HASS.Agent.Extensions;
using HASS.Agent.Managers;
using HASS.Agent.MQTT;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Sensors;
using HASS.Agent.Service;
using HASS.Agent.Settings;
using HASS.Agent.Shared;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.Config;
using HASS.Agent.Shared.Models.HomeAssistant;
using Octokit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HASS.Agent.Compatibility
{
    internal class NameCompatibilityTask : ICompatibilityTask
    {
        public string Name => Languages.Compat_NameTask_Name;

        private (List<ConfiguredSensor>, List<ConfiguredSensor>) ConvertClientSensors(IEnumerable<AbstractDiscoverable> sensors)
        {
            var configuredSensors = new List<ConfiguredSensor>();
            var toBeDeletedSensors = new List<ConfiguredSensor>();

            foreach (var sensor in sensors)
            {
                var currentConfiguredSensor = sensor is AbstractSingleValueSensor
                    ? StoredSensors.ConvertAbstractSingleValueToConfigured(sensor as AbstractSingleValueSensor)
                    : StoredSensors.ConvertAbstractMultiValueToConfigured(sensor as AbstractMultiValueSensor);

                if (!sensor.EntityName.Contains(SharedHelperFunctions.GetSafeConfiguredDeviceName())
                    && !sensor.Name.Contains(SharedHelperFunctions.GetSafeConfiguredDeviceName()))
                {
                    configuredSensors.Add(currentConfiguredSensor);
                    continue;
                }

                var newEntityName = sensor.EntityName.Replace($"{SharedHelperFunctions.GetSafeConfiguredDeviceName()}_", "");
                var newName = sensor.Name.Replace($"{SharedHelperFunctions.GetSafeConfiguredDeviceName()}_", "");
                var objectId = $"{SharedHelperFunctions.GetSafeConfiguredDeviceName()}_{newName}";
                if (objectId == sensor.EntityName)
                {
                    var newConfiguredSensor = sensor is AbstractSingleValueSensor
                    ? StoredSensors.ConvertAbstractSingleValueToConfigured(sensor as AbstractSingleValueSensor)
                    : StoredSensors.ConvertAbstractMultiValueToConfigured(sensor as AbstractMultiValueSensor);

                    newConfiguredSensor.EntityName = newEntityName;
                    newConfiguredSensor.Name = newName;
                    configuredSensors.Add(newConfiguredSensor);

                    toBeDeletedSensors.Add(currentConfiguredSensor);
                }
                else
                {
                    configuredSensors.Add(currentConfiguredSensor);
                }
            }

            return (configuredSensors, toBeDeletedSensors);
        }

        private (List<ConfiguredCommand>, List<ConfiguredCommand>) ConvertClientCommands(List<AbstractCommand> commands)
        {
            var configuredCommands = new List<ConfiguredCommand>();
            var toBeDeletedCommands = new List<ConfiguredCommand>();

            foreach (var command in commands)
            {
                var currentConfiguredCommand = StoredCommands.ConvertAbstractToConfigured(command);

                if (!command.EntityName.Contains(SharedHelperFunctions.GetSafeConfiguredDeviceName())
                    && !command.Name.Contains(SharedHelperFunctions.GetSafeConfiguredDeviceName()))
                {
                    configuredCommands.Add(currentConfiguredCommand);
                    continue;
                }

                var newEntityName = command.EntityName.Replace($"{SharedHelperFunctions.GetSafeConfiguredDeviceName()}_", "");
                var newName = command.Name.Replace($"{SharedHelperFunctions.GetSafeConfiguredDeviceName()}_", "");
                var objectId = $"{SharedHelperFunctions.GetSafeConfiguredDeviceName()}_{newEntityName}";
                if (objectId == command.EntityName)
                {
                    var newConfiguredCommand = StoredCommands.ConvertAbstractToConfigured(command);
                    newConfiguredCommand.EntityName = newEntityName;
                    newConfiguredCommand.Name = newName;
                    configuredCommands.Add(newConfiguredCommand);

                    toBeDeletedCommands.Add(currentConfiguredCommand);
                }
                else
                {
                    configuredCommands.Add(currentConfiguredCommand);
                }
            }

            return (configuredCommands, toBeDeletedCommands);
        }

        private async Task<string> ConvertClient()
        {
            var errorMessage = string.Empty;

            AgentSharedBase.Initialize(Variables.AppSettings.DeviceName, Variables.MqttManager, Variables.AppSettings.CustomExecutorBinary);

            await SettingsManager.LoadEntitiesAsync();
            Variables.MqttManager.Initialize();

            while (!Variables.MqttManager.IsConnected())
                await Task.Delay(1000);

            SensorsManager.Initialize();
            SensorsManager.Pause();
            CommandsManager.Initialize();
            CommandsManager.Pause();

            Log.Information("[COMPATTASK] Client: modifying stored single value sensors");
            var (sensors, toBeDeletedSensors) = ConvertClientSensors(Variables.SingleValueSensors);
            var result = await SensorsManager.StoreAsync(sensors, toBeDeletedSensors);
            SensorsManager.Pause();
            if (!result)
            {
                Log.Error("[COMPATTASK] Client: error modifying stored single value sensors");
                errorMessage += Languages.Compat_NameTask_Error_SingleValueSensors;
            }

            Log.Information("[COMPATTASK] Client: modifying stored multi value sensors");
            (sensors, toBeDeletedSensors) = ConvertClientSensors(Variables.MultiValueSensors);
            result = await SensorsManager.StoreAsync(sensors, toBeDeletedSensors);
            SensorsManager.Pause();
            if (!result)
            {
                Log.Error("[COMPATTASK] Client: error modifying stored multi value sensors");
                errorMessage += Languages.Compat_NameTask_Error_MultiValueSensors;
            }

            Log.Information("[COMPATTASK] Client: modifying stored commands");
            var (commands, toBeDeletedCommands) = ConvertClientCommands(Variables.Commands);
            result = await CommandsManager.StoreAsync(commands, toBeDeletedCommands);
            CommandsManager.Pause();
            if (!result)
            {
                Log.Error("[COMPATTASK] Client: error modifying stored commands");
                errorMessage += Languages.Compat_NameTask_Error_Commands;
            }

            return errorMessage;
        }

        private List<ConfiguredSensor> ConvertServiceSensors(IEnumerable<ConfiguredSensor> sensors, string safeServiceDeviceName)
        {
            var newServiceConfiguredSensors = new List<ConfiguredSensor>();
            foreach (var sensor in sensors)
            {
                if (!sensor.EntityName.Contains(safeServiceDeviceName)
                 && !sensor.Name.Contains(safeServiceDeviceName))
                {
                    newServiceConfiguredSensors.Add(sensor);
                    continue;
                }

                var newEntityName = sensor.EntityName.Replace($"{safeServiceDeviceName}_", "");
                var newName = sensor.Name.Replace($"{safeServiceDeviceName}_", "");
                var objectId = $"{safeServiceDeviceName}_{newName}";
                if (objectId == sensor.EntityName)
                {
                    sensor.EntityName = newEntityName;
                    sensor.Name = newName;
                }

                newServiceConfiguredSensors.Add(sensor);
            }

            return newServiceConfiguredSensors;
        }

        private List<ConfiguredCommand> ConvertServiceCommands(IEnumerable<ConfiguredCommand> commands, string safeServiceDeviceName)
        {
            var newServiceConfiguredCommands = new List<ConfiguredCommand>();
            foreach (var command in commands)
            {
                if (!command.EntityName.Contains(safeServiceDeviceName)
                 && !command.Name.Contains(safeServiceDeviceName))
                {
                    newServiceConfiguredCommands.Add(command);
                    continue;
                }

                var newEntityName = command.EntityName.Replace($"{safeServiceDeviceName}_", "");
                var newName = command.Name.Replace($"{safeServiceDeviceName}_", "");
                var objectId = $"{safeServiceDeviceName}_{newName}";
                if (objectId == command.EntityName)
                {
                    command.EntityName = newEntityName;
                    command.Name = newName;
                }

                newServiceConfiguredCommands.Add(command);
            }

            return newServiceConfiguredCommands;
        }

        private async Task<string> ConvertService()
        {
            var errorMessage = string.Empty;

            ServiceManager.Initialize();
            if (ServiceControllerManager.GetServiceState() != System.ServiceProcess.ServiceControllerStatus.Running)
            {
                Log.Information("[COMPATTASK] Service: not running, attempting start");
                await ServiceControllerManager.StartServiceAsync();
            }

            if (ServiceControllerManager.GetServiceState() != System.ServiceProcess.ServiceControllerStatus.Running)
            {
                Log.Error("[COMPATTASK] Service: cannot be started, service entities not parsed");
                errorMessage += Languages.Compat_NameTask_Error_ServiceStart;
                return errorMessage;
            }

            var (serviceDeviceNameOk, serviceDeviceName, _) = await Variables.RpcClient.GetDeviceNameAsync().WaitAsync(Variables.RpcConnectionTimeout);
            var (serviceCommandsOk, serviceConfiguredCommands, _) = await Variables.RpcClient.GetConfiguredCommandsAsync().WaitAsync(Variables.RpcConnectionTimeout);
            var (serviceSensorsOk, serviceConfiguredSensors, _) = await Variables.RpcClient.GetConfiguredSensorsAsync().WaitAsync(Variables.RpcConnectionTimeout);
            if (!serviceDeviceNameOk || !serviceCommandsOk || !serviceSensorsOk)
            {
                Log.Warning("[COMPATTASK] Service: error communicating with service");
                errorMessage += Languages.Compat_NameTask_Error_ServiceCommunication;
                return errorMessage;
            }

            var safeServiceDeviceName = SharedHelperFunctions.GetSafeValue(serviceDeviceName);

            Log.Information("[COMPATTASK] Service: modifying stored sensors");
            var newServiceConfiguredSensors = ConvertServiceSensors(serviceConfiguredSensors, safeServiceDeviceName);
            var (storedServiceSensorsOk, storedServiceSensorsError) = await Variables.RpcClient.SetConfiguredSensorsAsync(newServiceConfiguredSensors).WaitAsync(Variables.RpcConnectionTimeout);
            if (!storedServiceSensorsOk)
            {
                Log.Error("[COMPATTASK] Service: error modifying stored sensors: {msg}", storedServiceSensorsError);
                errorMessage += Languages.Compat_NameTask_Error_SingleValueSensors;
                return errorMessage;
            }

            Log.Information("[COMPATTASK] Service: modifying stored commands");
            var newServiceConfiguredCommands = ConvertServiceCommands(serviceConfiguredCommands, safeServiceDeviceName);
            var (storedServiceCommandsOk, storedServiceCommandsError) = await Variables.RpcClient.SetConfiguredCommandsAsync(newServiceConfiguredCommands).WaitAsync(Variables.RpcConnectionTimeout);
            if (!storedServiceCommandsOk)
            {
                Log.Error("[COMPATTASK] Service: error modifying stored commands: {msg}", storedServiceCommandsError);
                errorMessage += Languages.Compat_NameTask_Error_Commands;
                return errorMessage;
            }

            return errorMessage;
        }

        public async Task<(bool, string)> Perform()
        {
            try
            {
                Log.Information("[COMPATTASK] Entity name compatibility task started");

                var errorMessage = string.Empty;
                errorMessage += await ConvertClient();
                errorMessage += await ConvertService();

                Log.Information("[COMPATTASK] Entity name compatibility task ended");

                return string.IsNullOrWhiteSpace(errorMessage) ? (true, string.Empty) : (false, errorMessage);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMPATTASK] Error performing entity name compatibility task: {err}", ex.Message);
                return (false, Languages.Compat_Error_CheckLogs);
            }
        }
    }
}
