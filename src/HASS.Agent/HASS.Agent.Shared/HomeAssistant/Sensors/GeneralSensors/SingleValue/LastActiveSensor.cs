using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the last moment the user provided any input
    /// </summary>
    public class LastActiveSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "lastactive";

        private DateTime _lastActive = DateTime.MinValue;

        public const int DefaultTimeWindow = 15;

        public bool ApplyRounding { get; private set; }
        public int Round { get; private set; }

        public LastActiveSensor(bool updateOnResume, int? updateOnResumeTimeWindow, int? updateInterval = 10, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, advancedSettings: advancedSettings)
        {
            ApplyRounding = updateOnResume;
            Round = updateOnResumeTimeWindow ?? 30;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null)
                return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null)
                return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:clock-time-three-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability",
                Device_class = "timestamp"
            });
        }

        public override string GetState()
        {
            var lastInput = GetLastInputTime();

            if (ApplyRounding)
            {
                if (SharedSystemStateManager.LastEventOccurrence.TryGetValue(Enums.SystemStateEvent.Resume, out var lastWakeEventDate) // was there a wake event
                    && DateTime.Compare(lastInput, lastWakeEventDate) < 0 // was the last input before the last wake event
                    && (DateTime.Now - lastWakeEventDate).TotalSeconds < Round) // are we within the time window
                {

                    var currentPosition = Cursor.Position;
                    Cursor.Position = new Point(Cursor.Position.X - 10, Cursor.Position.Y - 10);
                    Cursor.Position = currentPosition;

                    lastInput = GetLastInputTime();
                }
            }

            // changed to min. 1 sec difference
            // source: https://github.com/sleevezipper/hass-workstation-service/pull/156
            if ((_lastActive - lastInput).Duration().TotalSeconds > 1)
                _lastActive = lastInput;

            return _lastActive.ToTimeZoneString();
        }

        public override string GetAttributes() => string.Empty;

        private static DateTime GetLastInputTime()
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            var envTicks = Environment.TickCount;

            if (!GetLastInputInfo(ref lastInputInfo))
                return DateTime.Now;

            var lastInputTick = Convert.ToDouble(lastInputInfo.dwTime);

            var idleTime = envTicks - lastInputTick;
            return idleTime > 0 ? DateTime.Now - TimeSpan.FromMilliseconds(idleTime) : DateTime.Now;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        private struct LASTINPUTINFO
        {
            private static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }
    }
}
