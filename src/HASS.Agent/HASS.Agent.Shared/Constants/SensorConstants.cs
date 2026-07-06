using System.Text.RegularExpressions;


namespace HASS.Agent.Shared.Constants;

public static class SensorConstants
{
    public const string DropdownAll = "*";

    public const string DropdownNone = "none";

    public static readonly Regex LuidRegex = new(@"luid_(0x[0-9A-Fa-f]+_0x[0-9A-Fa-f]+)", RegexOptions.Compiled);
}
