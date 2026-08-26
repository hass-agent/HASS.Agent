using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.MediaActivity;

internal sealed class WindowsCameraActivitySource
{
    private const int ReportTimeoutMilliseconds = 500;
    private const int MediaFoundationVersion = 0x00020070;
    private const int MediaFoundationStartupNoSocket = 0x1;

    internal MediaActivitySnapshot GetActivity()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return MediaActivitySnapshot.Unavailable;
        }

        SensorActivitiesCallback callback = null;
        IMFSensorActivityMonitor monitor = null;
        var mediaFoundationStarted = false;

        try
        {
            Marshal.ThrowExceptionForHR(
                MFStartup(MediaFoundationVersion, MediaFoundationStartupNoSocket));
            mediaFoundationStarted = true;
            callback = new SensorActivitiesCallback();
            Marshal.ThrowExceptionForHR(
                MFCreateSensorActivityMonitor(callback, out monitor));
            Marshal.ThrowExceptionForHR(monitor.Start());

            if (!callback.Wait(ReportTimeoutMilliseconds) || callback.Failed)
            {
                return MediaActivitySnapshot.Unavailable;
            }

            return MediaActivitySnapshot.Available(callback.Processes);
        }
        catch (Exception exception) when (
            exception is COMException ||
            exception is DllNotFoundException ||
            exception is EntryPointNotFoundException ||
            exception is PlatformNotSupportedException)
        {
            return MediaActivitySnapshot.Unavailable;
        }
        finally
        {
            if (monitor != null)
            {
                monitor.Stop();
                if (monitor is IMFShutdown shutdown)
                {
                    shutdown.Shutdown();
                }
            }

            callback?.Dispose();

            if (mediaFoundationStarted)
            {
                MFShutdown();
            }
        }
    }

    [DllImport("mfplat.dll", ExactSpelling = true)]
    private static extern int MFStartup(int version, int flags);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    private static extern int MFShutdown();

    [DllImport("mfsensorgroup.dll", ExactSpelling = true)]
    private static extern int MFCreateSensorActivityMonitor(
        IMFSensorActivitiesReportCallback callback,
        out IMFSensorActivityMonitor monitor);

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    private sealed class SensorActivitiesCallback :
        IMFSensorActivitiesReportCallback,
        IDisposable
    {
        private readonly ManualResetEventSlim _activityDetectedOrFailed = new();
        private IReadOnlyCollection<MediaActivityProcess> _processes =
            Array.Empty<MediaActivityProcess>();
        private volatile bool _reportReceived;

        internal IReadOnlyCollection<MediaActivityProcess> Processes => _processes;
        internal bool Failed { get; private set; }

        internal bool Wait(int milliseconds)
        {
            if (_activityDetectedOrFailed.Wait(milliseconds))
            {
                return !Failed;
            }

            return _reportReceived;
        }

        public int OnActivitiesReport(IMFSensorActivitiesReport reports)
        {
            try
            {
                var processes = new List<MediaActivityProcess>();
                Marshal.ThrowExceptionForHR(reports.GetCount(out var reportCount));

                for (uint reportIndex = 0; reportIndex < reportCount; reportIndex++)
                {
                    Marshal.ThrowExceptionForHR(
                        reports.GetActivityReport(reportIndex, out var report));
                    Marshal.ThrowExceptionForHR(
                        report.GetProcessCount(out var processCount));

                    for (uint processIndex = 0; processIndex < processCount; processIndex++)
                    {
                        Marshal.ThrowExceptionForHR(
                            report.GetProcessActivity(processIndex, out var activity));
                        Marshal.ThrowExceptionForHR(
                            activity.GetStreamingState(out var isStreaming));
                        if (!isStreaming)
                        {
                            continue;
                        }

                        Marshal.ThrowExceptionForHR(
                            activity.GetProcessId(out var processId));
                        if (processId > 0)
                        {
                            var id = unchecked((int)processId);
                            processes.Add(new MediaActivityProcess(
                                id,
                                WindowsMediaActivitySource.GetProcessName(id)));
                        }
                    }
                }

                _processes = MediaActivitySnapshot.Available(processes).Processes;
                _reportReceived = true;
                if (_processes.Count > 0)
                {
                    _activityDetectedOrFailed.Set();
                }
            }
            catch
            {
                Failed = true;
                _activityDetectedOrFailed.Set();
            }

            return 0;
        }

        public void Dispose()
        {
            _activityDetectedOrFailed.Dispose();
        }
    }

    [ComImport]
    [Guid("683F7A5E-4A19-43CD-B1A9-DBF4AB3F7777")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMFSensorActivitiesReport
    {
        [PreserveSig] int GetCount(out uint count);
        [PreserveSig] int GetActivityReport(
            uint index,
            out IMFSensorActivityReport report);
        [PreserveSig] int GetActivityReportByDeviceName(
            [MarshalAs(UnmanagedType.LPWStr)] string symbolicName,
            out IMFSensorActivityReport report);
    }

    [ComImport]
    [Guid("3E8C4BE1-A8C2-4528-90DE-2851BDE5FEAD")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMFSensorActivityReport
    {
        [PreserveSig] int GetFriendlyName(IntPtr name, uint capacity, out uint written);
        [PreserveSig] int GetSymbolicLink(IntPtr link, uint capacity, out uint written);
        [PreserveSig] int GetProcessCount(out uint count);
        [PreserveSig] int GetProcessActivity(
            uint index,
            out IMFSensorProcessActivity activity);
    }

    [ComImport]
    [Guid("39DC7F4A-B141-4719-813C-A7F46162A2B8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMFSensorProcessActivity
    {
        [PreserveSig] int GetProcessId(out uint processId);
        [PreserveSig] int GetStreamingState(
            [MarshalAs(UnmanagedType.Bool)] out bool streaming);
        [PreserveSig] int GetStreamingMode(out int mode);
        [PreserveSig] int GetReportTime(out long fileTime);
    }

    [ComVisible(true)]
    [Guid("DE5072EE-DBE3-46DC-8A87-B6F631194751")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMFSensorActivitiesReportCallback
    {
        [PreserveSig] int OnActivitiesReport(IMFSensorActivitiesReport reports);
    }

    [ComImport]
    [Guid("D0CEF145-B3F4-4340-A2E5-7A5080CA05CB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMFSensorActivityMonitor
    {
        [PreserveSig] int Start();
        [PreserveSig] int Stop();
    }

    [ComImport]
    [Guid("97EC2EA4-0E42-4937-97AC-9D6D328824E1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMFShutdown
    {
        [PreserveSig] int Shutdown();
        [PreserveSig] int GetShutdownStatus(out int status);
    }
}
