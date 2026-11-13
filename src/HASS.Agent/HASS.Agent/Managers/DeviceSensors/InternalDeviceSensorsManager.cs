using Serilog;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal static class InternalDeviceSensorsManager
    {
        private static readonly List<IInternalDeviceSensor> deviceSensors = new();
        private static DeviceWatcher deviceWatcher;

        private static Windows.Devices.Sensors.ProximitySensor _internalProximitySensor;

        public static List<IInternalDeviceSensor> AvailableSensors => deviceSensors.FindAll(s => s.Available);

        public static async Task Initialize()
        {
            try
            {
                deviceWatcher = DeviceInformation.CreateWatcher(Windows.Devices.Sensors.ProximitySensor.GetDeviceSelector());
                deviceWatcher.Added += OnProximitySensorAdded;

                //TODO(Amadeo): this is ugly
                try { deviceSensors.Add(new AccelerometerSensor(Accelerometer.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] Accelerometer not added"); }

                try { deviceSensors.Add(new ActivitySensor(await Windows.Devices.Sensors.ActivitySensor.GetDefaultAsync()));}
                catch { Log.Debug("[INTERNALSENSORS] ActivitySensor not added"); }

                try { deviceSensors.Add(new AltimeterSensor(Altimeter.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] Altimeter not added"); }

                try { deviceSensors.Add(new BarometerSensor(Barometer.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] Barometer not added"); }

                try { deviceSensors.Add(new CompassSensor(Compass.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] Compass not added"); }

                try { deviceSensors.Add(new GyrometerSensor(Gyrometer.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] Gyrometer not added"); }

                try { deviceSensors.Add(new HingeAngleSensor(await Windows.Devices.Sensors.HingeAngleSensor.GetDefaultAsync())); }
                catch { Log.Debug("[INTERNALSENSORS] HingeAngleSensor not added"); }

                try { deviceSensors.Add(new HumanPresenceSensor(Windows.Devices.Sensors.HumanPresenceSensor.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] HumanPresenceSensor not added"); }

                try { deviceSensors.Add(new InclinometerSensor(Inclinometer.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] Inclinometer not added"); }

                try { deviceSensors.Add(new LightSensor(Windows.Devices.Sensors.LightSensor.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] LightSensor not added"); }

                try { deviceSensors.Add(new MagnetometerSensor(Magnetometer.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] Magnetometer not added"); }

                try { deviceSensors.Add(new OrientationSensor(Windows.Devices.Sensors.OrientationSensor.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] OrientationSensor not added"); }

                try { deviceSensors.Add(new PedometerSensor(await Pedometer.GetDefaultAsync())); }
                catch { Log.Debug("[INTERNALSENSORS] Pedometer not added"); }

                try { deviceSensors.Add(new ProximitySensor(await GetDefaultProximitySensorAsync())); }
                catch { Log.Debug("[INTERNALSENSORS] ProximitySensor not added"); }

                try { deviceSensors.Add(new SimpleOrientationSensor(Windows.Devices.Sensors.SimpleOrientationSensor.GetDefault())); }
                catch { Log.Debug("[INTERNALSENSORS] SimpleOrientationSensor not added"); }

                Log.Information("[INTERNALSENSORS] Ready");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[INTERNALSENSORS] Error while initializing: {err}", ex.Message);
            }
        }

        private static void OnProximitySensorAdded(DeviceWatcher sender, DeviceInformation args)
        {
            if (_internalProximitySensor == null)
                _internalProximitySensor = Windows.Devices.Sensors.ProximitySensor.FromId(args.Id);
        }

        private static async Task<Windows.Devices.Sensors.ProximitySensor> GetDefaultProximitySensorAsync()
        {
            // allow 2 seconds for the sensor to load
            await Task.Delay(2000);
            return _internalProximitySensor;
        }
    }
}
