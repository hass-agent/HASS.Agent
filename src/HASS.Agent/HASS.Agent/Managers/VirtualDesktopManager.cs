using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Win32;
using Serilog;
using WindowsDesktop;

namespace HASS.Agent.Managers;

//Note(Amadeo): due to poor m$ decisions on availability of the Virtual Desktop management APIs, there is no good way to manage Virtual Desktops with publicly available ones.
//We need to rely on on Community made ones which can stop working with new Windows version due to changes in the COM object name changes.
//This means it's required to limit the functionality on systems not to expose users to runtime application errors.
internal static class VirtualDesktopManager
{
    public static bool Initialized { get; private set; } = false;

    internal static bool Initialize()
    {
        try
        {
            VirtualDesktop.Configure();
            Initialized = true;
            Log.Information("[VIRTDESKT] Virtual Desktop Manager initialized");
        }
        catch
        {
            Initialized = false;
            Log.Error("[VIRTDESKT] Error initializing Virtual Desktop Manager, your Windows version may be unsupported"); //TODO(Amadeo): add link to documentation explaining the issue.
        }

        return Initialized;
    }

    internal static void ActivateDesktop(string virtualDesktopId)
    {
        if (!Initialized)
        {
            Log.Warning("[VIRTDESKT] Cannot activate virtual desktop, manager not initialized");
        }

        Guid targetDesktopGuid;
        var parsed = Guid.TryParse(virtualDesktopId, out targetDesktopGuid);
        if (!parsed)
        {
            Log.Warning("[VIRTDESKT] Unable to parse virtual desktop id: {virtualDesktopId}", virtualDesktopId);
            return;
        }

        ActivateDesktop(targetDesktopGuid);
    }

    internal static void ActivateDesktop(Guid virtualDesktopGuid)
    {
        if (!Initialized)
        {
            Log.Warning("[VIRTDESKT] Cannot activate virtual desktop, manager not initialized");
        }

        try
        {
            var targetDesktop = VirtualDesktop.GetDesktops().FirstOrDefault(d => d.Id == virtualDesktopGuid);
            if (targetDesktop == null)
            {
                Log.Warning("[VIRTDESKT] Unable to find virtual desktop with id: {virtualDesktopId}", virtualDesktopGuid.ToString());
                return;
            }

            if (VirtualDesktop.Current == targetDesktop)
            {
                Log.Information("[VIRTDESKT] Target virtual desktop '{virtualDesktopId}' is already active", virtualDesktopGuid.ToString());
                return;
            }

            targetDesktop.Switch();
        }
        catch (Exception ex)
        {
            Log.Error("[VIRTDESKT] Unhanded exception activating desktop: {ex}", ex.Message);
        }
    }

    internal static VirtualDesktop GetCurrentDesktop()
    {
        if (!Initialized)
        {
            Log.Warning("[VIRTDESKT] Cannot get current desktop, manager not initialized");
            return null;
        }

        try
        {
            return VirtualDesktop.Current;
        }
        catch (Exception ex)
        {
            Log.Error("[VIRTDESKT] Unhanded exception returning current desktop: {ex}", ex.Message);
            return null;
        }
    }

    private static string GetDesktopNameFromRegistry(string id)
    {
        var registryPath = $"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{{{id}}}";
        return (Registry.GetValue(registryPath, "Name", string.Empty) as string) ?? string.Empty;
    }

    internal static Dictionary<string, string> GetAllDesktopsInfo()
    {
        var desktops = new Dictionary<string, string>();

        try
        {
            foreach (var desktop in VirtualDesktop.GetDesktops())
            {
                var id = desktop.Id.ToString();
                desktops[id] = string.IsNullOrWhiteSpace(desktop.Name) ? GetDesktopNameFromRegistry(id) : desktop.Name;
            }
        }
        catch (Exception ex)
        {
            Log.Error("[VIRTDESKT] Unhanded exception returning all desktops information: {ex}", ex.Message);
        }

        return desktops;
    }
}
