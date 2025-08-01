using System.Collections.Generic;

namespace HASS.Agent.Shared.Models.Internal
{
    public class WindowsUpdateInfoCollection
    {
        public WindowsUpdateInfoCollection()
        {
            //
        }

        public WindowsUpdateInfoCollection(List<WindowsUpdateInfo> windowsUpdates)
        {
            foreach (var windowsUpdate in windowsUpdates) WindowsUpdates.Add(windowsUpdate);
        }

        public List<WindowsUpdateInfo> WindowsUpdates { get; set; } = new List<WindowsUpdateInfo>();
    }

    /// <summary>
    /// Contains Windows update information
    /// </summary>
    public class WindowsUpdateInfo
    {
        public WindowsUpdateInfo()
        {
            //
        }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> KbArticleIDs { get; set; } = new List<string>();
        public bool Hidden { get; set; }
        public string SupportUrl { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new List<string>();
    }
}
