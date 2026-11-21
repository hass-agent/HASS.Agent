using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using HASS.Agent.Shared.Functions;
using Serilog;

namespace HASS.Agent.Shared.Managers;

public static partial class CameraManager
{
    private const uint MfVersion = 0x00020070;
    private const uint MfStartupLite = 0x1;

    private static readonly object StartLock = new();
    private static bool _startAttempted;
    
    private static ReportCallback _callback;
    private static IMFSensorActivityMonitor _monitor;
    
    private static volatile int[] _streamingProcessIds;

    private static void Start()
    {
        var mfStarted = false;

        try
        {
            Marshal.ThrowExceptionForHR(MFStartup(MfVersion, MfStartupLite));
            mfStarted = true;

            _callback = new ReportCallback();
            
            Marshal.ThrowExceptionForHR(MFCreateSensorActivityMonitor(_callback, out _monitor));
            Marshal.ThrowExceptionForHR(_monitor.Start());
        }
        catch (Exception ex)
        {
            Log.Warning("[CAMERA] Activity monitor unavailable, registry will be used as fallback: {err}", ex.Message);

            _monitor = null;
            _callback = null;

            if (mfStarted)
            {
                MFShutdown();
            }
        }
    }

    private static void ProcessReport(IMFSensorActivitiesReport report)
    {
        try
        {
            var processIds = new HashSet<int>();
            
            report.GetCount(out var cameraCount);
            for (uint camera = 0; camera < cameraCount; camera++)
            {
                report.GetActivityReport(camera, out var activity);
                activity.GetProcessCount(out var processCount);

                for (uint index = 0; index < processCount; index++)
                {
                    activity.GetProcessActivity(index, out var process);
                    process.GetProcessId(out var processId);
                    process.GetStreamingState(out var streaming);

                    //NOTE(Amadeo): only active camera usage counts (streaming)
                    if (streaming && processId > 0)
                    {
                        processIds.Add((int)processId);
                    }
                }
            }

            _streamingProcessIds = processIds.ToArray();
        }
        catch (Exception ex)
        {
            Log.Warning("[CAMERA] Unable to read the activity report: {err}", ex.Message);
            _streamingProcessIds = null;
        }
    }

    private sealed class ReportCallback : IMFSensorActivitiesReportCallback
    { 
        public int OnActivitiesReport(IMFSensorActivitiesReport report)
        {
            ProcessReport(report);
            return 0;
        }
    }

    internal static int[]? GetStreamingProcessIds()
    {
        lock (StartLock)
        {
            if (_startAttempted)
            {
                return _streamingProcessIds;
            }
            
            _startAttempted = true;
            Start();
        }

        return _streamingProcessIds;
    }
    
    internal static List<string>? GetWebcamApps()
    {
        var processIds = GetStreamingProcessIds();
        if (processIds == null)
        {
            return null;
        }
        
        var appNames = new List<string>();
        foreach (var processId in processIds)
        {
            var appName = SharedHelperFunctions.GetConsentStoreAppName(processId);
            if (string.IsNullOrEmpty(appName))
            {
                continue;
            }
            
            appNames.Add(appName);
        }

        return appNames;
    }
    
    [DllImport("mfplat.dll", ExactSpelling = true)]
    private static extern int MFStartup(uint version, uint flags);

    [DllImport("mfplat.dll", ExactSpelling = true)]
    private static extern int MFShutdown();

    [DllImport("mfsensorgroup.dll", ExactSpelling = true)]
    private static extern int MFCreateSensorActivityMonitor(IMFSensorActivitiesReportCallback callback, out IMFSensorActivityMonitor monitor);

    [ComImport, Guid("D0CEF145-B3F4-4340-A2E5-7A5080CA05CB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal partial interface IMFSensorActivityMonitor
    {
        [PreserveSig] int Start();
        [PreserveSig] int Stop();
    }

    [ComImport, Guid("DE5072EE-DBE3-46DC-8A87-B6F631194751"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMFSensorActivitiesReportCallback
    {
        [PreserveSig] int OnActivitiesReport(IMFSensorActivitiesReport report);
    }

    [ComImport, Guid("683F7A5E-4A19-43CD-B1A9-DBF4AB3F7777"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMFSensorActivitiesReport
    {
        void GetCount(out uint count);
        void GetActivityReport(uint index, out IMFSensorActivityReport report);
    }

    [ComImport, Guid("3E8C4BE1-A8C2-4528-90DE-2851BDE5FEAD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMFSensorActivityReport
    {
        void GetFriendlyName(IntPtr friendlyName, uint length, out uint written);
        void GetSymbolicLink(IntPtr symbolicLink, uint length, out uint written);
        void GetProcessCount(out uint count);
        void GetProcessActivity(uint index, out IMFSensorProcessActivity activity);
    }

    [ComImport, Guid("39DC7F4A-B141-4719-813C-A7F46162A2B8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal partial interface IMFSensorProcessActivity
    {
        void GetProcessId(out uint processId);
        void GetStreamingState([MarshalAs(UnmanagedType.Bool)] out bool streaming);
    }
}