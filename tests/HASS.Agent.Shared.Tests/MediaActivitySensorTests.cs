using System;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;
using HASS.Agent.Shared.HomeAssistant.Sensors.MediaActivity;
using Xunit;

namespace HASS.Agent.Shared.Tests;

public class MediaActivitySensorTests
{
    [Fact]
    public void ProviderUsesFallbackWhenWindowsApiIsUnavailable()
    {
        var primary = new StubSource(MediaActivitySnapshot.Unavailable);
        var fallback = new StubSource(Snapshot("Elgato.WaveLink", 42));
        var provider = new MediaActivityProvider(primary, fallback);

        var result = provider.GetActivity(MediaActivityKind.Microphone);

        Assert.True(result.IsActive);
        Assert.Equal(1, primary.CallCount);
        Assert.Equal(1, fallback.CallCount);
    }

    [Fact]
    public void ProviderDoesNotUseLegacyRegistryAfterSuccessfulEmptyWindowsApiResult()
    {
        var primary = new StubSource(MediaActivitySnapshot.Available(Array.Empty<MediaActivityProcess>()));
        var fallback = new StubSource(Snapshot("stale-registry-entry", 0));
        var provider = new MediaActivityProvider(primary, fallback);

        var result = provider.GetActivity(MediaActivityKind.Microphone);

        Assert.False(result.IsActive);
        Assert.Equal(0, fallback.CallCount);
    }

    [Fact]
    public void MicrophoneSensorsUseTheSameProviderSnapshot()
    {
        var provider = new StubProvider(Snapshot("Elgato.WaveLink", 26220));
        var activeSensor = new MicrophoneActiveSensor(provider);
        var processSensor = new MicrophoneProcessSensor(provider);

        Assert.Equal("ON", activeSensor.GetState());
        Assert.Equal(MediaActivityKind.Microphone, provider.LastKind);
        Assert.Equal("1", processSensor.GetState());
        Assert.Contains("Elgato.WaveLink", processSensor.GetAttributes());
    }

    [Fact]
    public void WebcamSensorsUseTheSameProviderSnapshot()
    {
        var provider = new StubProvider(Snapshot("WindowsCamera", 123));
        var activeSensor = new WebcamActiveSensor(provider);
        var processSensor = new WebcamProcessSensor(provider);

        Assert.Equal("ON", activeSensor.GetState());
        Assert.Equal(MediaActivityKind.Webcam, provider.LastKind);
        Assert.Equal("1", processSensor.GetState());
        Assert.Contains("WindowsCamera", processSensor.GetAttributes());
    }

    [Fact]
    public void ProcessSensorClearsAttributesWhenCaptureStops()
    {
        var provider = new StubProvider(Snapshot("Elgato.WaveLink", 42));
        var sensor = new MicrophoneProcessSensor(provider);

        Assert.Equal("1", sensor.GetState());
        provider.Snapshot = MediaActivitySnapshot.Available(Array.Empty<MediaActivityProcess>());

        Assert.Equal("0", sensor.GetState());
        Assert.Equal("{}", sensor.GetAttributes());
    }

    [Fact]
    public void SnapshotDeduplicatesSessionsFromTheSameProcess()
    {
        var snapshot = MediaActivitySnapshot.Available(new[]
        {
            new MediaActivityProcess(42, "Elgato.WaveLink"),
            new MediaActivityProcess(42, "Elgato.WaveLink")
        });

        Assert.Single(snapshot.Processes);
    }

    private static MediaActivitySnapshot Snapshot(string name, int processId) =>
        MediaActivitySnapshot.Available(new[]
        {
            new MediaActivityProcess(processId, name)
        });

    private sealed class StubProvider : IMediaActivityProvider
    {
        internal StubProvider(MediaActivitySnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        internal MediaActivitySnapshot Snapshot { get; set; }
        internal MediaActivityKind LastKind { get; private set; }

        public MediaActivitySnapshot GetActivity(MediaActivityKind kind)
        {
            LastKind = kind;
            return Snapshot;
        }
    }

    private sealed class StubSource : IMediaActivitySource
    {
        private readonly MediaActivitySnapshot _snapshot;

        internal StubSource(MediaActivitySnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        internal int CallCount { get; private set; }

        public MediaActivitySnapshot GetActivity(MediaActivityKind kind)
        {
            CallCount++;
            return _snapshot;
        }
    }
}
