using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using HASS.Agent.Shared.Functions;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.MediaActivity;

public enum MediaActivityKind
{
    Microphone,
    Webcam
}

public sealed class MediaActivityProcess
{
    public MediaActivityProcess(int processId, string name)
    {
        ProcessId = processId;
        Name = name;
    }

    public int ProcessId { get; }
    public string Name { get; }
}

public sealed class MediaActivitySnapshot
{
    private MediaActivitySnapshot(bool isAvailable, IReadOnlyCollection<MediaActivityProcess> processes)
    {
        IsAvailable = isAvailable;
        Processes = processes;
    }

    public bool IsAvailable { get; }
    public IReadOnlyCollection<MediaActivityProcess> Processes { get; }
    public bool IsActive => Processes.Count > 0;

    public static MediaActivitySnapshot Available(IEnumerable<MediaActivityProcess> processes)
    {
        var uniqueProcesses = processes
            .Where(process => process != null && !string.IsNullOrWhiteSpace(process.Name))
            .GroupBy(process => process.ProcessId > 0 ? $"pid:{process.ProcessId}" : $"name:{process.Name}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();

        return new MediaActivitySnapshot(true, uniqueProcesses);
    }

    public static MediaActivitySnapshot Unavailable { get; } =
        new(false, Array.Empty<MediaActivityProcess>());
}

public interface IMediaActivityProvider
{
    MediaActivitySnapshot GetActivity(MediaActivityKind kind);
}

internal interface IMediaActivitySource
{
    MediaActivitySnapshot GetActivity(MediaActivityKind kind);
}

internal sealed class MediaActivityProvider : IMediaActivityProvider
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(1);
    private readonly object _syncRoot = new();
    private readonly IMediaActivitySource _primary;
    private readonly IMediaActivitySource _fallback;
    private readonly Dictionary<MediaActivityKind, CacheEntry> _cache = new();

    internal static IMediaActivityProvider Instance { get; } =
        new MediaActivityProvider(new WindowsMediaActivitySource(), new RegistryMediaActivitySource());

    internal MediaActivityProvider(IMediaActivitySource primary, IMediaActivitySource fallback)
    {
        _primary = primary;
        _fallback = fallback;
    }

    public MediaActivitySnapshot GetActivity(MediaActivityKind kind)
    {
        lock (_syncRoot)
        {
            if (_cache.TryGetValue(kind, out var cached) &&
                DateTimeOffset.UtcNow - cached.CreatedAt < CacheDuration)
            {
                return cached.Snapshot;
            }

            var snapshot = _primary.GetActivity(kind);
            if (!snapshot.IsAvailable)
            {
                snapshot = _fallback.GetActivity(kind);
            }

            _cache[kind] = new CacheEntry(DateTimeOffset.UtcNow, snapshot);
            return snapshot;
        }
    }

    private sealed class CacheEntry
    {
        internal CacheEntry(DateTimeOffset createdAt, MediaActivitySnapshot snapshot)
        {
            CreatedAt = createdAt;
            Snapshot = snapshot;
        }

        internal DateTimeOffset CreatedAt { get; }
        internal MediaActivitySnapshot Snapshot { get; }
    }
}

internal sealed class WindowsMediaActivitySource : IMediaActivitySource
{
    private readonly WindowsCameraActivitySource _cameraSource = new();

    public MediaActivitySnapshot GetActivity(MediaActivityKind kind)
    {
        return kind == MediaActivityKind.Microphone
            ? GetMicrophoneActivity()
            : _cameraSource.GetActivity();
    }

    private static MediaActivitySnapshot GetMicrophoneActivity()
    {
        try
        {
            var processes = new List<MediaActivityProcess>();
            using var deviceEnumerator = new MMDeviceEnumerator();
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var device in devices)
            {
                using (device)
                {
                    var sessions = device.AudioSessionManager.Sessions;
                    for (var index = 0; index < sessions.Count; index++)
                    {
                        using var session = sessions[index];
                        if (session.State != AudioSessionState.AudioSessionStateActive)
                        {
                            continue;
                        }

                        var processId = unchecked((int)session.GetProcessID);
                        if (processId <= 0)
                        {
                            continue;
                        }

                        processes.Add(new MediaActivityProcess(processId, GetProcessName(processId)));
                    }
                }
            }

            return MediaActivitySnapshot.Available(processes);
        }
        catch (Exception exception) when (
            exception is COMException ||
            exception is InvalidOperationException ||
            exception is PlatformNotSupportedException)
        {
            return MediaActivitySnapshot.Unavailable;
        }
    }

    internal static string GetProcessName(int processId)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            return process.ProcessName;
        }
        catch
        {
            return $"process_{processId}";
        }
    }
}

internal sealed class RegistryMediaActivitySource : IMediaActivitySource
{
    private const string LastUsedTimeStop = "LastUsedTimeStop";
    private const string ConsentStorePath =
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore";

    public MediaActivitySnapshot GetActivity(MediaActivityKind kind)
    {
        var capability = kind == MediaActivityKind.Microphone ? "microphone" : "webcam";
        var processes = new List<MediaActivityProcess>();

        try
        {
            ReadHive(Registry.LocalMachine, capability, processes);
            ReadHive(Registry.CurrentUser, capability, processes);
            return MediaActivitySnapshot.Available(processes);
        }
        catch (Exception exception) when (
            exception is UnauthorizedAccessException ||
            exception is System.IO.IOException ||
            exception is System.Security.SecurityException)
        {
            return MediaActivitySnapshot.Unavailable;
        }
    }

    private static void ReadHive(
        RegistryKey hive,
        string capability,
        ICollection<MediaActivityProcess> processes)
    {
        using var key = hive.OpenSubKey($@"{ConsentStorePath}\{capability}");
        if (key == null)
        {
            return;
        }

        foreach (var subKeyName in key.GetSubKeyNames())
        {
            if (string.Equals(subKeyName, "NonPackaged", StringComparison.OrdinalIgnoreCase))
            {
                using var nonPackagedKey = key.OpenSubKey(subKeyName);
                if (nonPackagedKey == null)
                {
                    continue;
                }

                foreach (var applicationKeyName in nonPackagedKey.GetSubKeyNames())
                {
                    using var applicationKey = nonPackagedKey.OpenSubKey(applicationKeyName);
                    AddIfActive(applicationKey, processes);
                }
            }
            else
            {
                using var applicationKey = key.OpenSubKey(subKeyName);
                AddIfActive(applicationKey, processes);
            }
        }
    }

    private static void AddIfActive(
        RegistryKey applicationKey,
        ICollection<MediaActivityProcess> processes)
    {
        if (applicationKey?.GetValue(LastUsedTimeStop) is not long stopTime ||
            stopTime > 0)
        {
            return;
        }

        var applicationName =
            SharedHelperFunctions.ParseRegWebcamMicApplicationName(
                applicationKey.Name);
        processes.Add(new MediaActivityProcess(0, applicationName));
    }
}
