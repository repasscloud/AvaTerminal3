namespace AvaTerminal3.Models.Dto;

public class SupportedDialCodeDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string DisplayName => $"(+{CountryCode}) {CountryName}";
}