using System;
using System.Collections.Generic;
using System.Text;

namespace HASS.Agent.Shared.Models.Internal
{
    public class DisplayInfo
    {
        public DisplayInfo()
        {
            //
        }

        public string Name { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public string VirtualResolution { get; set; } = string.Empty;
        public int VirtualWidth { get; set; } = 0;
        public int VirtualHeight { get; set; } = 0;
        public int BitsPerPixel { get; set; } = 0;
        public bool PrimaryDisplay { get; set; }
        public string WorkingArea { get; set; } = string.Empty;
        public int WorkingAreaWidth { get; set; } = 0;
        public int WorkingAreaHeight { get; set; } = 0;
        public int RotatedDegrees { get; set; } = 0;
    }
}
