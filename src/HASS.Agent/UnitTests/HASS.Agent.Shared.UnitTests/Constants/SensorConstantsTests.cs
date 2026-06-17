using HASS.Agent.Shared.Constants;

namespace HASS.Agent.Shared.UnitTests.Constants;

public class SensorConstantsTests
{
    [TestCase("pid_1234_luid_0x00000000_0x0000C5F2_phys_0_eng_0_engtype_3D", "0x00000000_0x0000C5F2")]
    [TestCase("pid_4080_luid_0x00000000_0x0000C8F1_phys_1_eng_2_engtype_3D", "0x00000000_0x0000C8F1")]
    [TestCase("pid_2568_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D", "0x00000000_0x00016e08")]
    public void LuidRegex_ValidInstanceName_CapturesLuidSegment(string instanceName, string expectedCapture)
    {
        var match = SensorConstants.LuidRegex.Match(instanceName);

        Assert.That(match.Success, Is.True);
        Assert.That(match.Groups[1].Value, Is.EqualTo(expectedCapture));
    }

    [TestCase("some_unexpected_counter_instance_name")]
    [TestCase("pid_1234_phys_0_eng_0_engtype_3D")] // missing luid_ segment entirely
    [TestCase("")]
    public void LuidRegex_NoLuidSegment_DoesNotMatch(string instanceName)
    {
        var match = SensorConstants.LuidRegex.Match(instanceName);

        Assert.That(match.Success, Is.False);
    }
}
