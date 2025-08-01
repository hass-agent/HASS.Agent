using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HASS.Agent.Shared.Functions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Monitors
    {
        private readonly Screen _screen;
        private readonly DEVMODE _device;

        private Monitors(Screen screen, DEVMODE device)
        {
            _screen = screen;
            _device = device;
        }

        private const int ENUM_CURRENT_SETTINGS = -1;

        public Rectangle PhysicalBounds => new Rectangle(_device.dmPositionX, _device.dmPositionY, _device.dmPelsWidth, _device.dmPelsHeight);

        public Rectangle Bounds => _screen.Bounds;

        public string Name => _screen.DeviceName.Split('\\')?.Last() ?? _screen.DeviceName;

        public int RotatedDegrees => GetRotatedDegrees();

        private int GetRotatedDegrees()
        {
            switch (_device.dmDisplayOrientation)
            {
                case ScreenOrientation.Angle0:
                    return 0;
                case ScreenOrientation.Angle90:
                    return 90;
                case ScreenOrientation.Angle180:
                    return 180;
                case ScreenOrientation.Angle270:
                    return 270;
                default:
                    return 0;
            }
        }

        public static Monitors[] All => Screen.AllScreens.Select(From).ToArray();

        private static Monitors From(Screen screen)
        {
            var dm = new DEVMODE { dmSize = (short)Marshal.SizeOf(typeof(DEVMODE)) };
            EnumDisplaySettings(screen.DeviceName, ENUM_CURRENT_SETTINGS, ref dm);

            return new Monitors(screen, dm);
        }

        [DllImport("user32.dll")]
        private static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
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
    }
}
