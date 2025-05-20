namespace AvaTerminal3.Helpers;

public static class CountryHelper
{
    private static readonly List<string> Countries = new()
    {
        "Australia",
        "New Zealand",
        "United States",
        "United Kingdom"
    };

    public static List<string> GetCountryList() => Countries;
}
