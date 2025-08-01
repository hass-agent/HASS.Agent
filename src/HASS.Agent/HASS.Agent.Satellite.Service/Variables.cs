using System.Reflection;
using GrpcDotNetNamedPipes;
using HASS.Agent.Shared.Models.Config.Service;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Mqtt;
using HASS.Agent.Satellite.Service.MQTT;
using MQTTnet;
using Serilog.Core;
#pragma warning disable CS8602

namespace HASS.Agent.Satellite.Service
{
    internal static class Variables
    {
        /// <summary>
        /// Application info
        /// </summary>
        internal static string ApplicationName { get; } = Assembly.GetExecutingAssembly().GetName().Name ?? "HASS.Agent.Satellite.Service";
        internal static string ApplicationExecutable { get; } = Assembly.GetExecutingAssembly().Location;
        internal static string Version { get; } = $"{Assembly.GetExecutingAssembly().GetName().Version?.Major}.{Assembly.GetExecutingAssembly().GetName().Version?.Minor}.{Assembly.GetExecutingAssembly().GetName().Version?.Build}.{Assembly.GetExecutingAssembly().GetName().Version?.Revision}";

        /// <summary>
        /// RPC
        /// </summary>
        internal static string RpcPipeName => "6aaac90b-d046-4db2-be76-af225e0d249f";
        internal static NamedPipeServer? RpcServer { get; set; } = null;

        /// <summary>
        /// Constants
        /// </summary>
        internal const string RootMachineRegKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\HASSAgent\SatelliteService";
        internal const string RootHassAgentRegKey = @"HKEY_CURRENT_USER\SOFTWARE\HASSAgent\Client";

        /// <summary>
        /// Logging
        /// </summary>
        internal static LoggingLevelSwitch LevelSwitch { get; } = new();

        /// <summary>
        /// Local IO
        /// </summary>
        internal static string StartupPath { get; } = AppDomain.CurrentDomain.BaseDirectory;
        internal static string LogPath { get; } = Path.Combine(StartupPath, "logs");
        internal static string ConfigPath { get; } = Path.Combine(StartupPath, "config");
        internal static string ServiceSettingsFile { get; } = Path.Combine(ConfigPath, "servicesettings.json");
        internal static string ServiceMqttSettingsFile { get; } = Path.Combine(ConfigPath, "servicemqttsettings.json");
        internal static string CommandsFile { get; } = Path.Combine(ConfigPath, "commands.json");
        internal static string SensorsFile { get; } = Path.Combine(ConfigPath, "sensors.json");

        /// <summary>
        /// Internal state
        /// </summary>
        internal static bool ShuttingDown { get; set; } = false;
        internal static bool CommenceShutdown { get; set; } = false;
        internal static bool ExtendedLogging { get; set; } = false;

        /// <summary>
        /// MQTT
        /// </summary>
        internal static IMqttManager MqttManager { get; } = new MqttManager();
        internal static MqttFactory MqttFactory { get; } = new();
        internal static DeviceConfigModel? DeviceConfig { get; set; } = null;

        /// <summary>
        /// Config
        /// </summary>
        internal static ServiceSettings? ServiceSettings { get; set; } = new();
        internal static ServiceMqttSettings? ServiceMqttSettings { get; set; } = new();
        internal static List<AbstractCommand> Commands { get; set; } = new();
        internal static List<AbstractSingleValueSensor> SingleValueSensors { get; set; } = new();
        internal static List<AbstractMultiValueSensor> MultiValueSensors { get; set; } = new();
    }
}
