namespace AvaTerminal3.Helpers;

public static class CountryCodeHelper
{
    private static readonly Dictionary<string, string> CountryToDialCode = new()
    {
        { "Australia", "61" },
        { "New Zealand", "64" },
        { "United States", "1" },
        { "United Kingdom", "44" }
    };

    // Used to populate the Picker list
    public static List<string> GetCountryDisplayList() =>
        CountryToDialCode.Select(kv => $"{kv.Key} (+{kv.Value})").ToList();

    // Extracts just the dial code as string (e.g., "61") from the selected display text
    public static string GetDialCodeFromDisplay(string display)
    {
        var start = display.IndexOf("(+") + 2;
        var end = display.IndexOf(")", start);
        return display.Substring(start, end - start);
    }

    // Optionally: Get country name from dial code
    public static string? GetCountryName(string dialCode)
    {
        return CountryToDialCode.FirstOrDefault(kv => kv.Value == dialCode).Key;
    }
}
