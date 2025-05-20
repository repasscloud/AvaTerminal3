namespace AvaTerminal3.Helpers;

public static class TaxRegistrationHelper
{
    private static readonly List<string> TaxRegistrations = new()
    {
        "ABN - Australian Business Number",
        "ACN - Australian Company Number",
        "ARBN - Australian Registered Body Number"
    };

    public static List<string> GetTaxRegistrationList() => TaxRegistrations;
}
