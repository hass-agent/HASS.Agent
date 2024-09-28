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

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;
public class ScreenshotSensor : AbstractSingleValueSensor
{
    private const string DefaultName = "screenshot";

    public int ScreenIndex;

    public ScreenshotSensor(string screenIndex = "0", int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 15, id)
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
        var captureRectangle = screen.Bounds;
        var captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height, PixelFormat.Format32bppArgb);
        var captureGraphics = Graphics.FromImage(captureBitmap);
        captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

        using var ms = new MemoryStream();
        captureBitmap.Save(ms, ImageFormat.Png);

        return ms.ToArray();
    }

    public override string GetAttributes() => string.Empty;
}
