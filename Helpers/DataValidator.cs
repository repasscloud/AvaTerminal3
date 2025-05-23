using System.Text.RegularExpressions;
using AvaTerminal3.Models.Dto;

namespace AvaTerminal3.Helpers;

public static class DataValidator
{
    public static string? ReturnOnlyCountryCode(string? s)
        => s is null
        ? null
        : Regex.Replace(s ?? "", @"\D", "");

    /// <summary>
    /// Removes all leading zeros from the string. 
    /// If the input is null or empty, returns it unchanged.
    /// </summary>
    public static string? StripLeadingZeros(this string? s)
        => string.IsNullOrEmpty(s) 
            ? s 
            : s.TrimStart('0');

    public static string? CleanPhoneNumber(this string? s)
        => s is null
        ? null
        : StripLeadingZeros(ReturnOnlyCountryCode(s));


    /// <summary>
    /// Validates the essential fields of an AvaClientDto.
    /// Returns an array of (isValid, Title, Message) tuples;
    /// the first tuple where isValid==false should be shown to the user.
    /// </summary>
    public static (bool isValid, string Title, string Message)[] ValidateAvaClientDto(AvaClientDto dto)
    {
        var results = new List<(bool isValid, string Title, string Message)>();

        // 1) Company name is required
        if (string.IsNullOrWhiteSpace(dto.CompanyName))
        {
            results.Add((
                isValid: false,
                Title: "Company Name Required",
                Message: "Please enter the company name before saving."));
        }

        // 2) If a Tax ID was entered, a corresponding type must be selected
        if (!string.IsNullOrWhiteSpace(dto.TaxId) &&
            string.IsNullOrWhiteSpace(dto.TaxIdType))
        {
            results.Add((
                isValid: false,
                Title: "Tax ID Type Missing",
                Message: "You provided a Tax ID but did not select its type."));
        }

        // 3) If a Tax ID was entered, but the type was not selected
        if (dto.TaxIdType is null &&
            !string.IsNullOrWhiteSpace(dto.TaxId))
        {
            results.Add((
                isValid: false,
                Title: "Invalid Tax ID Type",
                Message: "Cannot enter a Tax ID when it's type is not set."
            ));
        }

        // 4) If the type is explicitly 'NONE', no Tax ID should be provided
        if (dto.TaxIdType?.ToUpperInvariant() == "NONE" &&
            !string.IsNullOrWhiteSpace(dto.TaxId))
        {
            results.Add((
                isValid: false,
                Title: "Invalid Tax ID Type",
                Message: "Cannot enter a Tax ID when its type is set to 'NONE'."));
        }

        // 5) Country is mandatory (for regulatory compliance)
        if (string.IsNullOrWhiteSpace(dto.Country))
        {
            results.Add((
                isValid: false,
                Title: "Country Required",
                Message: "Please select a country for this client."));
        }

        // 6) Contact person: first name, last name, phone, country code, email all required
        if (string.IsNullOrWhiteSpace(dto.ContactPersonFirstName))
        {
            results.Add((
                isValid: false,
                Title: "Contact First Name",
                Message: "Contact person's first name cannot be empty."));
        }

        if (string.IsNullOrWhiteSpace(dto.ContactPersonLastName))
        {
            results.Add((
                isValid: false,
                Title: "Contact Last Name",
                Message: "Contact person's last name cannot be empty."));
        }

        if (string.IsNullOrWhiteSpace(dto.ContactPersonCountryCode))
        {
            results.Add((
                isValid: false,
                Title: "Contact Country Code",
                Message: "Please select a country code for the contact person's phone."));
        }

        if (string.IsNullOrWhiteSpace(dto.ContactPersonPhone))
        {
            results.Add((
                isValid: false,
                Title: "Contact Phone",
                Message: "Contact person's phone number is required."));
        }

        if (string.IsNullOrWhiteSpace(dto.ContactPersonEmail))
        {
            results.Add((
                isValid: false,
                Title: "Contact Email",
                Message: "Contact person's email address cannot be empty."));
        }

        if (!EmailValidator.IsValidEmail(dto.ContactPersonEmail))
        {
            results.Add((
                isValid: false,
                Title: "Contact Email",
                Message: "Contact person's email address is not valid."));
        }

        // 7) Default currency must be selected and cannot be changed later
        if (string.IsNullOrWhiteSpace(dto.DefaultCurrency))
        {
            results.Add((
                isValid: false,
                Title: "Default Currency",
                Message: "Please select a default currency for this client."));
        }

        // If we found any errors, return them. Otherwise report success.
        if (results.Any(r => !r.isValid))
            return results.ToArray();

        return new[]
        {
            (isValid: true,
                Title:   "Validation Passed",
                Message: "All required fields are valid.")
        };
    }
}