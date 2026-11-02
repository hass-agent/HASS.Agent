using Windows.Devices.Bluetooth;
using Serilog;
using Windows.Devices.Enumeration;
using BluetoothDevice = HASS.Agent.Models.Internal.BluetoothDevice;
using WindowsBluetoothDevice = Windows.Devices.Bluetooth.BluetoothDevice;
using Windows.Devices.Bluetooth.Advertisement;
using HASS.Agent.Models.Internal;
using System.Threading.Tasks;

namespace HASS.Agent.Managers
{
    internal static class BluetoothManager
    {
        // windows' bluetooth/device apis can hang indefinitely, never wait on them without a limit
        private static readonly TimeSpan QueryTimeout = TimeSpan.FromSeconds(15);

        // how long a resolved LE device is trusted, and how long to wait before retrying a failed lookup
        private static readonly TimeSpan LeLookupRefreshInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan LeLookupRetryInterval = TimeSpan.FromMinutes(5);

        // advertisements can arrive dozens of times per second, limit the parallel device lookups
        private static readonly SemaphoreSlim LeLookupSemaphore = new(2, 2);

        private static readonly object LeDevicesLock = new();
        private static readonly Dictionary<ulong, BluetoothLeDevice> DetectedLeDevices = new();
        private static readonly Dictionary<ulong, LeLookup> LeLookups = new();

        private static BluetoothLEAdvertisementWatcher _leWatcher;
        private static bool _isWatchingLeDevices;

        private static Task<List<BluetoothDevice>> _devicesQuery;

        /// <summary>
        /// Gets all known (connected) bluetooth devices, or null if the bluetooth stack didn't respond in time
        /// </summary>
        /// <returns></returns>
        internal static async Task<List<BluetoothDevice>> GetDevicesAsync()
        {
            try
            {
                // reuse a query that's still running, so a hanging bluetooth stack doesn't pile up new ones
                var query = _devicesQuery;
                if (query == null || query.IsCompleted)
                {
                    query = QueryDevicesAsync();
                    _devicesQuery = query;
                }

                return await query.WaitAsync(QueryTimeout);
            }
            catch (TimeoutException)
            {
                Log.Warning("[BLUETOOTH] Retrieving devices took longer than {timeout} seconds, skipping", QueryTimeout.TotalSeconds);
                return null;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[BLUETOOTH] Error getting devices: {err}", ex.Message);
                return new List<BluetoothDevice>();
            }
        }

        private static async Task<List<BluetoothDevice>> QueryDevicesAsync()
        {
            // first get all paired devices
            var pairedBluetoothDevices = await DeviceInformation.FindAllAsync(WindowsBluetoothDevice.GetDeviceSelectorFromPairingState(true));
            var devices = pairedBluetoothDevices.Select(pairedDevice => new BluetoothDevice { Id = pairedDevice.Id, Name = pairedDevice.Name, Paired = pairedDevice.Pairing.IsPaired, Connected = false, Kind = pairedDevice.Kind.ToString(), LastSeenUtc = DateTime.UtcNow}).ToList();

            // then get all connected devices
            var connectedBluetoothDevices = await DeviceInformation.FindAllAsync(WindowsBluetoothDevice.GetDeviceSelectorFromConnectionStatus(BluetoothConnectionStatus.Connected));
            foreach (var connectedDevice in connectedBluetoothDevices)
            {
                // do we have it already?
                if (devices.Any(x => x.Id == connectedDevice.Id))
                {
                    // set it to connected
                    devices.Find(x => x.Id == connectedDevice.Id)!.Connected = true;
                    continue;
                }

                // add new one
                var device = new BluetoothDevice
                {
                    Id = connectedDevice.Id,
                    Name = connectedDevice.Name,
                    Paired = connectedDevice.Pairing.IsPaired,
                    Connected = true
                };

                devices.Add(device);
            }

            // done
            return devices;
        }

        /// <summary>
        /// Initialises the LE scanner
        /// </summary>
        internal static void StartWatchingForLeDevices()
        {
            if (_isWatchingLeDevices) return;

            try
            {
                _isWatchingLeDevices = true;

                _leWatcher = new BluetoothLEAdvertisementWatcher
                {
                    ScanningMode = BluetoothLEScanningMode.Active
                };

                _leWatcher.Received += ScanOnReceived;
                _leWatcher.Stopped += ScanOnStopped;

                _leWatcher.Start();

                Log.Information("[BLUETOOTH] LE scanner started");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[BLUETOOTH] Error starting LE scanner: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Stops the LE scanner
        /// </summary>
        internal static void StopLeScan()
        {
            if (!_isWatchingLeDevices) return;

            try
            {
                _isWatchingLeDevices = false;
                _leWatcher.Stop();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[BLUETOOTH] Error stopping LE scanner: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of all detected LE devices since the last retrieval (unless clearList is set to false)
        /// </summary>
        /// <param name="clearList"></param>
        /// <returns></returns>
        internal static Task<List<BluetoothLeDevice>> GetDetectedLeDevicesAsync(bool clearList = true)
        {
            lock (LeDevicesLock)
            {
                // make a copy of the devices
                var deviceList = DetectedLeDevices.Values.ToList();

                // if requested, clear the current list
                if (clearList) DetectedLeDevices.Clear();

                return Task.FromResult(deviceList);
            }
        }

        private static void ScanOnStopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            Log.Information("[BLUETOOTH] LE scanner stopped");
        }

        private static async void ScanOnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var address = args.BluetoothAddress;
            var now = DateTime.UtcNow;

            lock (LeDevicesLock)
            {
                if (LeLookups.TryGetValue(address, out var lookup))
                {
                    // already being looked up, or recently failed
                    if (lookup.InProgress) return;
                    if (lookup.Device == null && now - lookup.LastAttemptUtc < LeLookupRetryInterval) return;

                    // recently resolved, no need to ask windows again
                    if (lookup.Device != null && now - lookup.LastAttemptUtc < LeLookupRefreshInterval)
                    {
                        MarkLeDeviceSeen(address, lookup.Device, now);
                        return;
                    }
                }
            }

            // too many lookups running, this device will advertise again
            if (!LeLookupSemaphore.Wait(0)) return;

            try
            {
                lock (LeDevicesLock)
                {
                    if (LeLookups.TryGetValue(address, out var lookup) && lookup.InProgress) return;
                    LeLookups[address] = new LeLookup { InProgress = true, Device = lookup?.Device, LastAttemptUtc = now };
                }

                BluetoothLeDevice leDevice = null;

                try
                {
                    // fetch the device based on its address
                    using var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address).AsTask().WaitAsync(QueryTimeout);
                    if (device != null)
                    {
                        leDevice = new BluetoothLeDevice
                        {
                            Id = device.DeviceId,
                            Name = device.Name,
                            Connected = device.ConnectionStatus == BluetoothConnectionStatus.Connected
                        };
                    }
                }
                catch (Exception ex)
                {
                    // common for devices that stop advertising or refuse the connection, retried later
                    Log.Debug("[BLUETOOTH] Unable to resolve LE device {address}: {err}", address.ToString("X12"), ex.Message);
                }

                lock (LeDevicesLock)
                {
                    LeLookups[address] = new LeLookup { InProgress = false, Device = leDevice, LastAttemptUtc = DateTime.UtcNow };
                    if (leDevice != null) MarkLeDeviceSeen(address, leDevice, DateTime.UtcNow);
                }
            }
            finally
            {
                LeLookupSemaphore.Release();
            }
        }

        /// <summary>
        /// Adds or refreshes a device in the detected list, caller must hold LeDevicesLock
        /// </summary>
        private static void MarkLeDeviceSeen(ulong address, BluetoothLeDevice resolved, DateTime seenUtc)
        {
            if (DetectedLeDevices.TryGetValue(address, out var existing))
            {
                existing.LastSeenUtc = seenUtc;
                return;
            }

            DetectedLeDevices[address] = new BluetoothLeDevice
            {
                Id = resolved.Id,
                Name = resolved.Name,
                Connected = resolved.Connected,
                LastSeenUtc = seenUtc
            };
        }

        private class LeLookup
        {
            public bool InProgress { get; init; }
            public BluetoothLeDevice Device { get; init; }
            public DateTime LastAttemptUtc { get; init; }
        }
    }
}
