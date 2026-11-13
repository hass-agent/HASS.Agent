using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains all possible sensor types
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SensorType
    {
        [LocalizedDescription("SensorType_ActiveWindowSensor", typeof(Languages))]
        [EnumMember(Value = "ActiveWindowSensor")]
        ActiveWindowSensor,

        [LocalizedDescription("SensorType_ActiveDesktopSensor", typeof(Languages))]
        [EnumMember(Value = "ActiveDesktopSensor")]
        ActiveDesktopSensor,

        [LocalizedDescription("SensorType_AccentColorSensor", typeof(Languages))]
        [EnumMember(Value = "AccentColorSensor")]
        AccentColorSensor,

        [LocalizedDescription("SensorType_AudioSensors", typeof(Languages))]
        [EnumMember(Value = "AudioSensors")]
        AudioSensors,

        [LocalizedDescription("SensorType_BatterySensors", typeof(Languages))]
        [EnumMember(Value = "BatterySensors")]
        BatterySensors,

        [LocalizedDescription("SensorType_BluetoothDevicesSensor", typeof(Languages))]
        [EnumMember(Value = "BluetoothDevicesSensor")]
        BluetoothDevicesSensor,

        [LocalizedDescription("SensorType_BluetoothLeDevicesSensor", typeof(Languages))]
        [EnumMember(Value = "BluetoothLeDevicesSensor")]
        BluetoothLeDevicesSensor,

        [LocalizedDescription("SensorType_CpuLoadSensor", typeof(Languages))]        
        [EnumMember(Value = "CpuLoadSensor")]
        CpuLoadSensor,

        [LocalizedDescription("SensorType_CurrentClockSpeedSensor", typeof(Languages))]
        [EnumMember(Value = "CurrentClockSpeedSensor")]
        CurrentClockSpeedSensor,

        [LocalizedDescription("SensorType_CurrentVolumeSensor", typeof(Languages))]
        [EnumMember(Value = "CurrentVolumeSensor")]
        CurrentVolumeSensor,

        [LocalizedDescription("SensorType_DisplaySensors", typeof(Languages))]
        [EnumMember(Value = "DisplaySensors")]
        DisplaySensors,

        [LocalizedDescription("SensorType_DummySensor", typeof(Languages))]
        [EnumMember(Value = "DummySensor")]
        DummySensor,

        [LocalizedDescription("SensorType_GeoLocationSensor", typeof(Languages))]
        [EnumMember(Value = "GeoLocationSensor")]
        GeoLocationSensor,

        [LocalizedDescription("SensorType_GpuLoadSensor", typeof(Languages))]
        [EnumMember(Value = "GpuLoadSensor")]
        GpuLoadSensor,

        [LocalizedDescription("SensorType_GpuTemperatureSensor", typeof(Languages))]
        [EnumMember(Value = "GpuTemperatureSensor")]
        GpuTemperatureSensor,

        [LocalizedDescription("SensorType_LastActiveSensor", typeof(Languages))]
        [EnumMember(Value = "LastActiveSensor")]
        LastActiveSensor,

        [LocalizedDescription("SensorType_LastBootSensor", typeof(Languages))]
        [EnumMember(Value = "LastBootSensor")]
        LastBootSensor,

        [LocalizedDescription("SensorType_LastSystemStateChangeSensor", typeof(Languages))]
        [EnumMember(Value = "LastSystemStateChangeSensor")]
        LastSystemStateChangeSensor,

        [LocalizedDescription("SensorType_LoggedUserSensor", typeof(Languages))]
        [EnumMember(Value = "LoggedUserSensor")]
        LoggedUserSensor,

        [LocalizedDescription("SensorType_LoggedUsersSensor", typeof(Languages))]
        [EnumMember(Value = "LoggedUsersSensor")]
        LoggedUsersSensor,

        [LocalizedDescription("SensorType_MemoryUsageSensor", typeof(Languages))]
        [EnumMember(Value = "MemoryUsageSensor")]
        MemoryUsageSensor,

        [LocalizedDescription("SensorType_MicrophoneActiveSensor", typeof(Languages))]
        [EnumMember(Value = "MicrophoneActiveSensor")]
        MicrophoneActiveSensor,

        [LocalizedDescription("SensorType_MicrophoneProcessSensor", typeof(Languages))]
        [EnumMember(Value = "MicrophoneProcessSensor")]
        MicrophoneProcessSensor,

        [LocalizedDescription("SensorType_MonitorPowerStateSensor", typeof(Languages))]
        [EnumMember(Value = "MonitorPowerStateSensor")]
        MonitorPowerStateSensor,

        [LocalizedDescription("SensorType_NamedWindowSensor", typeof(Languages))]
        [EnumMember(Value = "NamedWindowSensor")]
        NamedWindowSensor,

        [LocalizedDescription("SensorType_NamedActiveWindowSensor", typeof(Languages))]
        [EnumMember(Value = "NamedActiveWindowSensor")]
        NamedActiveWindowSensor,

        [LocalizedDescription("SensorType_NetworkSensors", typeof(Languages))]
        [EnumMember(Value = "NetworkSensors")]
        NetworkSensors,

        [LocalizedDescription("SensorType_PerformanceCounterSensor", typeof(Languages))]
        [EnumMember(Value = "PerformanceCounterSensor")]
        PerformanceCounterSensor,

        [LocalizedDescription("SensorType_PowershellSensor", typeof(Languages))]
        [EnumMember(Value = "PowershellSensor")]
        PowershellSensor,

        [LocalizedDescription("SensorType_PrintersSensors", typeof(Languages))]
        [EnumMember(Value = "PrintersSensors")]
        PrintersSensors,

        [LocalizedDescription("SensorType_ProcessActiveSensor", typeof(Languages))]
        [EnumMember(Value = "ProcessActiveSensor")]
        ProcessActiveSensor,

        [LocalizedDescription("SensorType_ServiceStateSensor", typeof(Languages))]
        [EnumMember(Value = "ServiceStateSensor")]
        ServiceStateSensor,

        [LocalizedDescription("SensorType_SessionStateSensor", typeof(Languages))]
        [EnumMember(Value = "SessionStateSensor")]
        SessionStateSensor,

        [LocalizedDescription("SensorType_StorageSensors", typeof(Languages))]
        [EnumMember(Value = "StorageSensors")]
        StorageSensors,

        [LocalizedDescription("SensorType_UserNotificationStateSensor", typeof(Languages))]
        [EnumMember(Value = "UserNotificationStateSensor")]
        UserNotificationStateSensor,

        [LocalizedDescription("SensorType_WebcamActiveSensor", typeof(Languages))]
        [EnumMember(Value = "WebcamActiveSensor")]
        WebcamActiveSensor,

        [LocalizedDescription("SensorType_WebcamProcessSensor", typeof(Languages))]
        [EnumMember(Value = "WebcamProcessSensor")]
        WebcamProcessSensor,

        [LocalizedDescription("SensorType_WindowStateSensor", typeof(Languages))]
        [EnumMember(Value = "WindowStateSensor")]
        WindowStateSensor,

        [LocalizedDescription("SensorType_WindowsUpdatesSensors", typeof(Languages))]
        [EnumMember(Value = "WindowsUpdatesSensors")]
        WindowsUpdatesSensors,

        [LocalizedDescription("SensorType_WmiQuerySensor", typeof(Languages))]
        [EnumMember(Value = "WmiQuerySensor")]
        WmiQuerySensor,

        [LocalizedDescription("SensorType_InternalDeviceSensor", typeof(Languages))]
        [EnumMember(Value = "InternalDeviceSensor")]
        InternalDeviceSensor,

        [LocalizedDescription("SensorType_ScreenshotSensor", typeof(Languages))]
        [EnumMember(Value = "ScreenshotSensor")]
        ScreenshotSensor
    }
}