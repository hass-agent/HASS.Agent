using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HASS.Agent.Shared.Models.Internal
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StorageInfo
    {
        public StorageInfo()
        {
            //
        }

        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string FileSystem { get; set; } = string.Empty;
        public double TotalSizeMB { get; set; } = 0d;
        public double AvailableSpaceMB { get; set; } = 0d;
        public double UsedSpaceMB { get; set; } = 0d;
        public int AvailableSpacePercentage { get; set; } = 0;
        public int UsedSpacePercentage { get; set; } = 0;
    }
}
