using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Serilog;

namespace HASS.Agent.Shared.Managers
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class ProcessManager
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr WindowHandle);

        private const int SW_RESTORE = 9;

        /// <summary>
        /// Attempts to bring the process' main window to front
        /// </summary>
        /// <param name="procName"></param>
        internal static void BringMainWindowToFront(string procName)
        {
            try
            {
                var processes = Process.GetProcessesByName(procName);
                if (!processes.Any()) return;

                var hWnd = processes[0].MainWindowHandle;
                ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
                SetForegroundWindow(hWnd);

                foreach (var proc in processes) proc?.Dispose();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[PROCESSMANAGER] Unable to bring main window to front for '{proc}': {err}", procName, ex.Message);
            }
        }
    }
}
