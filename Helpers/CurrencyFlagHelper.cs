namespace AvaTerminal3.Helpers;

public static class CurrencyFlagHelper
{
    private static readonly Dictionary<string, string> CurrencyToFlagFile = new()
    {
        { "AUD", "flag_au.png" },
        { "USD", "flag_us.png" },
        { "EUR", "flag_eu.png" },
        { "GBP", "flag_gb.png" },
        { "JPY", "flag_jp.png" },
        { "CHF", "flag_ch.png" },
        { "CAD", "flag_ca.png" },
        { "CNY", "flag_cn.png" },
        { "HKD", "flag_hk.png" },
        { "SGD", "flag_sg.png" },
        { "NZD", "flag_nz.png" }
    };

    // Used to populate the Picker list (just currency codes now)
    public static List<string> GetCurrencyDisplayList() =>
        CurrencyToFlagFile.Keys.ToList();

    // Extract currency code from Picker item (now direct match, but kept for safety)
    public static string GetCurrencyCodeFromDisplay(string display) =>
        display.Trim();

    public static string? GetFlagImage(string currencyCode)
    {
        if (CurrencyToFlagFile.TryGetValue(currencyCode, out var filename))
            return filename;

        return null;
    }
}
