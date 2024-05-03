using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using Windows.Graphics.Display;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;
public class ScreenshotSensor : AbstractSingleValueSensor
{
    private const string DefaultName = "screenshot";

    private DEVMODE _devMode = new()
    {
        dmSize = (short)Marshal.SizeOf(typeof(DEVMODE))
    };

    public int ScreenIndex;

    public ScreenshotSensor(string screenIndex = "0", int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 15, id, advancedSettings: advancedSettings)
    {
        ScreenIndex = int.TryParse(screenIndex, out var parsedScreenIndex) ? parsedScreenIndex : 0;
        Domain = "camera";
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig()
    {
        if (Variables.MqttManager == null)
            return null;

        var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
        if (deviceConfig == null)
            return null;

        return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new CameraSensorDiscoveryConfigModel()
        {
            EntityName = EntityName,
            Name = Name,
            Unique_id = Id,
            Device = deviceConfig,
            Image_encoding = "b64",
            Icon = "mdi:camera",
            State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
            Topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
            Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability"
        });
    }

    public override string GetState()
    {
        var screenCount = Screen.AllScreens.Length;
        if (ScreenIndex >= screenCount || ScreenIndex < 0)
        {
            Log.Warning("[SCREENSHOT] Wrong index '{index}' - returning image for screen 0", ScreenIndex);
            ScreenIndex = 0;
        }

        var screenImage = CaptureScreen(ScreenIndex);
        return Convert.ToBase64String(screenImage);
    }

    private byte[] CaptureScreen(int screenIndex)
    {
        try
        {
            return CapturePngFile(Screen.AllScreens[screenIndex]);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[SCREENSHOT] Internal Error capturing screen {index}, {ex}", ex.Message);
            return Array.Empty<byte>();
        }
    }

    private byte[] CapturePngFile(Screen screen)
    {
        EnumDisplaySettings(screen.DeviceName, -1, ref _devMode);

        var scalingFactor = Math.Round(decimal.Divide(_devMode.dmPelsWidth, screen.Bounds.Width), 2);

        var captureRectangle = screen.Bounds;
        var captureSize = new Size(Convert.ToInt32(screen.Bounds.Width * scalingFactor), Convert.ToInt32(screen.Bounds.Height * scalingFactor));
        using var captureBitmap = new Bitmap(captureSize.Width, captureSize.Height, PixelFormat.Format32bppArgb);
        var captureGraphics = Graphics.FromImage(captureBitmap);
        captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureSize);

        using var ms = new MemoryStream();
        captureBitmap.Save(ms, ImageFormat.Png);

        return ms.ToArray();
    }

    public override string GetAttributes() => string.Empty;

    [StructLayout(LayoutKind.Sequential)]
    private struct DEVMODE
    {
        private const int CCHDEVICENAME = 0x20;
        private const int CCHFORMNAME = 0x20;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public ScreenOrientation dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);
}
