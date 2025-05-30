// ViewModels/CLT/NewAvaClientViewModel.cs
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Services.Interfaces;
using System.Text.Json;
using AvaTerminal3.Helpers;

namespace AvaTerminal3.ViewModels.CLT;

public partial class NewAvaClientViewModel : ObservableObject, INotifyPropertyChanged
{
    private readonly IAvaApiService _avaApiService;
    private readonly IPopupService _popupService;

    public AvaClientDto Client { get; private set; } = new();

    // — lists fetched from API —
    public List<string> TaxIdList { get; private set; } = new();
    public List<string> CountryList { get; private set; } = new();
    public List<string> DialCodeList { get; private set; } = new();
    public List<string> CurrencyList { get; private set; } = new();

    public NewAvaClientViewModel(
        IAvaApiService avaApiService,
        IPopupService popupService)
    {
        _avaApiService = avaApiService;
        _popupService = popupService;

        // fire-and-forget load
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        TaxIdList = await _avaApiService.GetTaxIdsAsync();
        CountryList = await _avaApiService.GetAvailableCountriesAsync();
        DialCodeList = await _avaApiService.GetCountryDialCodesAsync();
        CurrencyList = await _avaApiService.GetAvailableCurrencyCodesAsync();

        OnPropertyChanged(nameof(TaxIdList));
        OnPropertyChanged(nameof(CountryList));
        OnPropertyChanged(nameof(DialCodeList));
        OnPropertyChanged(nameof(CurrencyList));
    }

    // — commands to pop up each selector —

    [RelayCommand]
    private async Task SelectTaxIdAsync()
    {
        var sel = await _popupService.ShowSelectAsync(
            "Select Tax ID", TaxIdList, Client.TaxId);
        if (sel is not null)
        {
            Client.TaxIdType = sel;
            OnPropertyChanged(nameof(Client));
        }
    }

    [RelayCommand]
    private async Task SelectCountryAsync()
    {
        var sel = await _popupService.ShowSelectAsync(
            "Select Country", CountryList, Client.Country);
        if (sel is not null)
        {
            Client.Country = sel;
            OnPropertyChanged(nameof(Client));
        }
    }

    [RelayCommand]
    private async Task SelectContactDialCodeAsync()
    {
        var sel = await _popupService.ShowSelectAsync(
            "Contact Dial Code", DialCodeList, Client.ContactPersonCountryCode);
        if (sel is not null)
        {
            Client.ContactPersonCountryCode = sel;
            OnPropertyChanged(nameof(Client));
        }
    }

    [RelayCommand]
    private async Task SelectBillingDialCodeAsync()
    {
        var sel = await _popupService.ShowSelectAsync(
            "Billing Dial Code", DialCodeList, Client.BillingPersonCountryCode);
        if (sel is not null)
        {
            Client.BillingPersonCountryCode = sel;
            OnPropertyChanged(nameof(Client));
        }
    }

    [RelayCommand]
    private async Task SelectAdminDialCodeAsync()
    {
        var sel = await _popupService.ShowSelectAsync(
            "Admin Dial Code", DialCodeList, Client.AdminPersonCountryCode);
        if (sel is not null)
        {
            Client.AdminPersonCountryCode = sel;
            OnPropertyChanged(nameof(Client));
        }
    }

    [RelayCommand]
    private async Task SelectCurrencyAsync()
    {
        var sel = await _popupService.ShowSelectAsync(
            "Select Currency", CurrencyList, Client.DefaultCurrency);
        if (sel is not null)
        {
            Client.DefaultCurrency = sel;
            OnPropertyChanged(nameof(Client));
            OnPropertyChanged(nameof(SelectedCurrencyFlag));
        }
    }

    // — save & cancel commands —

    [RelayCommand]
    private async Task SaveAsync()
    {
        // do client validation first, else return
        var errors = DataValidator.ValidateAvaClientDto(Client);
        var firstError = errors.FirstOrDefault(e => !e.isValid);

        if (!string.IsNullOrEmpty(firstError.Title))
        {
            // a real error
            await LogSinkService.WriteAsync(LogLevel.Warn, $"[NewClient.SaveAsync] Error saving record '{firstError.Title}' with message '{firstError.Message}'.");
            await _popupService.ShowNoticeAsync(firstError.Title, firstError.Message);
            return;
        }

        await LogSinkService.WriteAsync(LogLevel.Debug, "[NewClient.SaveAsync] Client passed validations.");

        Client.ContactPersonEmail = Client.ContactPersonEmail.SetLowerCase();
        Client.BillingPersonEmail = Client.BillingPersonEmail.SetLowerCase();
        Client.AdminPersonEmail = Client.AdminPersonEmail.SetLowerCase();

        await LogSinkService.WriteAsync(LogLevel.Debug, "[NewClient.SaveAsync] Sanitize email addresses.");

        // sanitize country‐code and phone fields (only before saving)
        // Client.ContactPersonCountryCode = DataValidator.ReturnOnlyCountryCode(Client.ContactPersonCountryCode);
        // Client.BillingPersonCountryCode = DataValidator.ReturnOnlyCountryCode(Client.BillingPersonCountryCode);
        // Client.AdminPersonCountryCode = DataValidator.ReturnOnlyCountryCode(Client.AdminPersonCountryCode);

        Client.ContactPersonPhone = DataValidator.CleanPhoneNumber(Client.ContactPersonPhone);
        Client.BillingPersonPhone = DataValidator.CleanPhoneNumber(Client.BillingPersonPhone);
        Client.AdminPersonPhone = DataValidator.CleanPhoneNumber(Client.AdminPersonPhone);

        await LogSinkService.WriteAsync(LogLevel.Debug, "[NewClient.SaveAsync] Sanitize the phone number values.");

        // save the data to a tmp file (debug)
        // if (File.Exists(Path.Combine(FileSystem.AppDataDirectory, "new_client.json")))
        //     File.Delete(Path.Combine(FileSystem.AppDataDirectory, "new_client.json"));
        // await SaveAsJsonAsync(Client, Path.Combine(FileSystem.AppDataDirectory, "new_client.json"));

        // await LogSinkService.WriteAsync(LogLevel.Debug, "[NewClient.SaveAsync] Create json dump 'new_client.json'.");

        var ok = await _avaApiService.CreateClientAsync(Client);

        if (ok)
        {
            await LogSinkService.WriteAsync(LogLevel.Info, "[NewClient.SaveAsync] Record saved successfully to API.");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await _popupService.ShowNoticeAsync(
                "Error", "Unable to save client. Please try again.");
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // - copy/unset billing contact

    [ObservableProperty]
    private bool isBillingPersonSameAsContact;

    // partial method is called whenever that bool changes:
    partial void OnIsBillingPersonSameAsContactChanged(bool value)
    {
        if (value)
        {
            // copy from contact
            Client.BillingPersonFirstName = Client.ContactPersonFirstName;
            Client.BillingPersonLastName = Client.ContactPersonLastName;
            Client.BillingPersonCountryCode = Client.ContactPersonCountryCode;
            Client.BillingPersonPhone = Client.ContactPersonPhone;
            Client.BillingPersonEmail = Client.ContactPersonEmail;
            Client.BillingPersonJobTitle = Client.ContactPersonJobTitle;
        }
        else
        {
            // clear out or leave as-is
            Client.BillingPersonFirstName = string.Empty;
            Client.BillingPersonLastName = string.Empty;
            Client.BillingPersonCountryCode = null;
            Client.BillingPersonPhone = string.Empty;
            Client.BillingPersonEmail = string.Empty;
            Client.BillingPersonJobTitle = string.Empty;
        }

        // notify the UI that the Client data has changed:
        OnPropertyChanged(nameof(Client));
    }

    [ObservableProperty]
    private bool isAdminPersonSameAsContact;

    // partial method is called whenever that bool changes:
    partial void OnIsAdminPersonSameAsContactChanged(bool value)
    {
        if (value)
        {
            // copy from contact
            Client.AdminPersonFirstName = Client.ContactPersonFirstName;
            Client.AdminPersonLastName = Client.ContactPersonLastName;
            Client.AdminPersonCountryCode = Client.ContactPersonCountryCode;
            Client.AdminPersonPhone = Client.ContactPersonPhone;
            Client.AdminPersonEmail = Client.ContactPersonEmail;
            Client.AdminPersonJobTitle = Client.ContactPersonJobTitle;
        }
        else
        {
            // clear out or leave as-is
            Client.AdminPersonFirstName = string.Empty;
            Client.AdminPersonLastName = string.Empty;
            Client.AdminPersonCountryCode = null;
            Client.AdminPersonPhone = string.Empty;
            Client.AdminPersonEmail = string.Empty;
            Client.AdminPersonJobTitle = string.Empty;
        }

        // notify the UI that the Client data has changed:
        OnPropertyChanged(nameof(Client));
    }

    // - other stuff -
    public string SelectedCurrencyFlag
        => $"{Client?.DefaultCurrency?.ToLower()}.png";

    public string LastUpdatedLocalTime =>
        Client.LastUpdated?
            .ToLocalTime()
            .ToString("f")
        ?? DateTime.UtcNow
            .ToLocalTime()
            .ToString("f");

    public static Task SaveAsJsonAsync<T>(T obj, string path)
        => File.WriteAllTextAsync(path, JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));

}
