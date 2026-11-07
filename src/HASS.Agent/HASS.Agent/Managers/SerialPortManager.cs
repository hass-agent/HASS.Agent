using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue;
using HASS.Agent.Models.Internal;
using Microsoft.Win32;
using Serilog;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace HASS.Agent.Managers
{
    /// <summary>
    /// Keeps track of the system's serial (COM) ports, and notifies the related sensors when ports are added or removed
    /// </summary>
    internal static class SerialPortManager
    {
        private const string PortNameProperty = "System.DeviceInterface.Serial.PortName";
        private const string UsbVendorIdProperty = "System.DeviceInterface.Serial.UsbVendorId";
        private const string UsbProductIdProperty = "System.DeviceInterface.Serial.UsbProductId";

        private const string SerialCommRegistryKey = @"HARDWARE\DEVICEMAP\SERIALCOMM";

        private static readonly Regex PortInNameRegex = new(@"\((COM\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly ConcurrentDictionary<string, SerialPortInfo> WatchedPorts = new();

        private static DeviceWatcher _watcher;

        /// <summary>
        /// Starts watching for serial port arrivals and removals
        /// </summary>
        internal static void Initialize()
        {
            try
            {
                if (_watcher != null)
                    return;

                _watcher = DeviceInformation.CreateWatcher(
                    SerialDevice.GetDeviceSelector(),
                    new[] { PortNameProperty, UsbVendorIdProperty, UsbProductIdProperty },
                    DeviceInformationKind.DeviceInterface);

                _watcher.Added += OnAdded;
                _watcher.Removed += OnRemoved;

                // must be subscribed, otherwise Added/Removed stop firing after the initial enumeration
                _watcher.Updated += OnUpdated;

                _watcher.EnumerationCompleted += OnEnumerationCompleted;
                _watcher.Stopped += OnStopped;

                _watcher.Start();

                Log.Information("[SERIALPORTS] Watching for serial port changes");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[SERIALPORTS] Unable to start serial port watcher: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Stops watching
        /// </summary>
        internal static void Stop()
        {
            try
            {
                if (_watcher == null)
                    return;

                _watcher.Added -= OnAdded;
                _watcher.Removed -= OnRemoved;
                _watcher.Updated -= OnUpdated;
                _watcher.EnumerationCompleted -= OnEnumerationCompleted;
                _watcher.Stopped -= OnStopped;

                if (_watcher.Status is DeviceWatcherStatus.Started or DeviceWatcherStatus.EnumerationCompleted)
                    _watcher.Stop();

                _watcher = null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[SERIALPORTS] Error stopping serial port watcher: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Returns all currently present serial ports, sorted by port number.
        /// Combines the watcher's results with the SERIALCOMM registry key, which also contains
        /// ports that don't expose the COM port device interface (eg. some virtual or bluetooth ports)
        /// </summary>
        internal static List<SerialPortInfo> GetPorts()
        {
            var ports = WatchedPorts.Values
                .Where(x => !string.IsNullOrEmpty(x.Port))
                .GroupBy(x => x.Port, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(SerialCommRegistryKey);
                if (key != null)
                {
                    foreach (var valueName in key.GetValueNames())
                    {
                        if (key.GetValue(valueName) is not string portName || string.IsNullOrWhiteSpace(portName))
                            continue;

                        if (ports.ContainsKey(portName))
                            continue;

                        ports[portName] = new SerialPortInfo
                        {
                            Port = portName,
                            Name = portName,
                            Id = valueName
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "[SERIALPORTS] Unable to read SERIALCOMM registry key: {err}", ex.Message);
            }

            return ports.Values
                .OrderBy(x => PortNumber(x.Port))
                .ThenBy(x => x.Port, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static void OnAdded(DeviceWatcher sender, DeviceInformation device)
        {
            try
            {
                var port = ToSerialPortInfo(device);
                WatchedPorts[device.Id] = port;

                Log.Debug("[SERIALPORTS] Port added: {port} ({name})", port.Port, port.Name);
                NotifyChanged();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[SERIALPORTS] Error processing added port: {err}", ex.Message);
            }
        }

        private static void OnRemoved(DeviceWatcher sender, DeviceInformationUpdate update)
        {
            if (!WatchedPorts.TryRemove(update.Id, out var port))
                return;

            Log.Debug("[SERIALPORTS] Port removed: {port} ({name})", port.Port, port.Name);
            NotifyChanged();
        }

        private static void OnUpdated(DeviceWatcher sender, DeviceInformationUpdate update)
        {
            // nothing to do, the selector only matches enabled interfaces so disabling results in 'Removed'
        }

        private static void OnEnumerationCompleted(DeviceWatcher sender, object args)
        {
            Log.Information("[SERIALPORTS] Initial enumeration completed, {count} port(s) found", WatchedPorts.Count);
            NotifyChanged();
        }

        private static void OnStopped(DeviceWatcher sender, object args)
        {
            if (sender.Status == DeviceWatcherStatus.Aborted)
                Log.Warning("[SERIALPORTS] Serial port watcher was aborted");
        }

        private static SerialPortInfo ToSerialPortInfo(DeviceInformation device)
        {
            var portName = GetProperty(device, PortNameProperty) as string;
            if (string.IsNullOrWhiteSpace(portName))
            {
                var match = PortInNameRegex.Match(device.Name ?? string.Empty);
                if (match.Success)
                    portName = match.Groups[1].Value.ToUpperInvariant();
            }

            return new SerialPortInfo
            {
                Port = portName ?? string.Empty,
                Name = device.Name ?? string.Empty,
                VendorId = FormatUsbId(GetProperty(device, UsbVendorIdProperty)),
                ProductId = FormatUsbId(GetProperty(device, UsbProductIdProperty)),
                Id = device.Id
            };
        }

        private static object GetProperty(DeviceInformation device, string property) =>
            device.Properties.TryGetValue(property, out var value) ? value : null;

        private static string FormatUsbId(object value) => value switch
        {
            ushort u => u.ToString("X4"),
            uint u => u.ToString("X4"),
            int i => i.ToString("X4"),
            _ => string.Empty
        };

        private static int PortNumber(string port) =>
            port != null && port.StartsWith("COM", StringComparison.OrdinalIgnoreCase) && int.TryParse(port[3..], out var number)
                ? number
                : int.MaxValue;

        /// <summary>
        /// Forces all serial port sensors to publish on the next sensor loop iteration
        /// </summary>
        private static void NotifyChanged()
        {
            try
            {
                var sensors = Variables.SingleValueSensors?.OfType<SerialPortsSensor>().ToList();
                if (sensors == null)
                    return;

                foreach (var sensor in sensors)
                    sensor.ResetChecks();
            }
            catch (Exception ex)
            {
                // the sensor list can be modified while we're iterating, the sensor's interval will catch up
                Log.Debug(ex, "[SERIALPORTS] Unable to notify sensors: {err}", ex.Message);
            }
        }
    }
}
