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

                deviceSensors.Add(new AccelerometerSensor(Accelerometer.GetDefault()));
                deviceSensors.Add(new ActivitySensor(await Windows.Devices.Sensors.ActivitySensor.GetDefaultAsync()));
                deviceSensors.Add(new AltimeterSensor(Altimeter.GetDefault()));
                deviceSensors.Add(new BarometerSensor(Barometer.GetDefault()));
                deviceSensors.Add(new CompassSensor(Compass.GetDefault()));
                deviceSensors.Add(new GyrometerSensor(Gyrometer.GetDefault()));
                deviceSensors.Add(new HingeAngleSensor(await Windows.Devices.Sensors.HingeAngleSensor.GetDefaultAsync()));
                deviceSensors.Add(new HumanPresenceSensor(Windows.Devices.Sensors.HumanPresenceSensor.GetDefault()));
                deviceSensors.Add(new InclinometerSensor(Inclinometer.GetDefault()));
                deviceSensors.Add(new LightSensor(Windows.Devices.Sensors.LightSensor.GetDefault()));
                deviceSensors.Add(new MagnetometerSensor(Magnetometer.GetDefault()));
                deviceSensors.Add(new OrientationSensor(Windows.Devices.Sensors.OrientationSensor.GetDefault()));
                deviceSensors.Add(new PedometerSensor(await Pedometer.GetDefaultAsync()));
                deviceSensors.Add(new ProximitySensor(await GetDefaultProximitySensorAsync()));
                deviceSensors.Add(new SimpleOrientationSensor(Windows.Devices.Sensors.SimpleOrientationSensor.GetDefault()));

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
