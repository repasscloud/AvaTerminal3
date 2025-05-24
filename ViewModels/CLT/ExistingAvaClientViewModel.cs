// ViewModels/CLT/ExistingAvaClientViewModel.cs
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Services.Interfaces;
using System.ComponentModel;
using AvaTerminal3.Helpers;

namespace AvaTerminal3.ViewModels.CLT;

public partial class ExistingAvaClientViewModel : ObservableObject, INotifyPropertyChanged
{
    readonly ISharedStateService _state;
    readonly IAvaApiService _api;
    readonly IPopupService _popup;

    public AvaClientDto Client { get; private set; }

    // — lists fetched from API —
    public List<string> TaxIdList { get; private set; } = new();
    public List<string> CountryList { get; private set; } = new();
    public List<string> DialCodeList { get; private set; } = new();
    public List<string> CurrencyList { get; private set; } = new();

    [ObservableProperty]
    private bool isLocked = true;

    public ExistingAvaClientViewModel(
        ISharedStateService sharedStateService,
        IAvaApiService avaApiService,
        IPopupService popupService)
    {
        _state = sharedStateService;
        _api = avaApiService;
        _popup = popupService;

        // pull in the DTO from memory
        Client = _state.ReadAvaClientDto()
            ?? throw new InvalidOperationException("No client in shared state.");

        // fire-and-forget the lookups
        _ = LoadAsync();
    }

    public bool IsEditable => !IsLocked;
    public bool IsSavable => IsLocked;
    public string LockButtonText => IsLocked ? "🔓 Unlock" : "🔐 Lock";

    private async Task LoadAsync()
    {
        TaxIdList = await _api.GetTaxIdsAsync();
        OnPropertyChanged(nameof(TaxIdList));

        CountryList = await _api.GetAvailableCountriesAsync();
        OnPropertyChanged(nameof(CountryList));

        DialCodeList = await _api.GetCountryDialCodesAsync();
        OnPropertyChanged(nameof(DialCodeList));

        CurrencyList = await _api.GetAvailableCurrencyCodesAsync();
        OnPropertyChanged(nameof(CurrencyList));
    }

    [RelayCommand]
    private void ToggleLock()
    {
        IsLocked = !IsLocked;
        OnPropertyChanged(nameof(IsEditable));
        OnPropertyChanged(nameof(IsSavable));
        OnPropertyChanged(nameof(LockButtonText));
    }

    // — pop-up selectors (no change from New) :contentReference[oaicite:0]{index=0} :contentReference[oaicite:1]{index=1}

    [RelayCommand]
    private async Task SelectTaxIdAsync()
    {
        var sel = await _popup.ShowSelectAsync(
            "Select Tax ID", TaxIdList, Client.TaxIdType);

        if (!string.IsNullOrWhiteSpace(sel))
        {
            Client.TaxIdType = sel;
            OnPropertyChanged(nameof(Client));
        }
    }

    [RelayCommand]
    private async Task SelectCountryAsync()
    {
        var sel = await _popup.ShowSelectAsync(
            "Select Country", CountryList, Client.Country);

        if (!string.IsNullOrWhiteSpace(sel))
        {
            Client.Country = sel;
            OnPropertyChanged(nameof(Client));
        }
    }

    [RelayCommand]
    private async Task SelectContactDialCodeAsync()
    {
        var sel = await _popup.ShowSelectAsync(
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
        var sel = await _popup.ShowSelectAsync(
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
        var sel = await _popup.ShowSelectAsync(
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
        var sel = await _popup.ShowSelectAsync(
            "Select Currency", CurrencyList, Client.DefaultCurrency);

        if (!string.IsNullOrWhiteSpace(sel))
        {
            Client.DefaultCurrency = sel;
            OnPropertyChanged(nameof(Client));
            OnPropertyChanged(nameof(SelectedCurrencyFlag));
        }
    }

    // — save / cancel :contentReference[oaicite:2]{index=2}

    [RelayCommand]
    private async Task SaveAsync()
    {
        string loggingPrefix = "[ExistingAvaClient.SaveAsync]";

        var errors = DataValidator.ValidateAvaClientDto(Client);
        var firstError = errors.FirstOrDefault(e => !e.isValid);

        if (!string.IsNullOrEmpty(firstError.Title))
        {
            // a real error
            await LogSinkService.WriteAsync(LogLevel.Warn, $"{loggingPrefix} Error saving record '{firstError.Title}' with message '{firstError.Message}'.");
            await _popup.ShowNoticeAsync(firstError.Title, firstError.Message);
            return;
        }

        await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Client passed validations.");

        Client.ContactPersonEmail = Client.ContactPersonEmail.SetLowerCase();
        Client.BillingPersonEmail = Client.BillingPersonEmail.SetLowerCase();
        Client.AdminPersonEmail = Client.AdminPersonEmail.SetLowerCase();

        await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Sanitize email addresses.");

        var ok = await _api.UpdateClientAsync(Client);

        if (ok)
        {
            await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Record updated successfully to API.");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await _popup.ShowNoticeAsync(
                "Error", "Unable to save client. Please try again.");
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // — copy/unset billing = new ObservableProperties :contentReference[oaicite:3]{index=3}

    [ObservableProperty]
    private bool isBillingPersonSameAsContact;
    partial void OnIsBillingPersonSameAsContactChanged(bool value)
    {
        if (value)
        {
            Client.BillingPersonFirstName = Client.ContactPersonFirstName;
            Client.BillingPersonLastName = Client.ContactPersonLastName;
            Client.BillingPersonCountryCode = Client.ContactPersonCountryCode;
            Client.BillingPersonPhone = Client.ContactPersonPhone;
            Client.BillingPersonEmail = Client.ContactPersonEmail;
            Client.BillingPersonJobTitle = Client.ContactPersonJobTitle;
        }
        else
        {
            Client.BillingPersonFirstName = string.Empty;
            Client.BillingPersonLastName = string.Empty;
            Client.BillingPersonCountryCode = null;
            Client.BillingPersonPhone = string.Empty;
            Client.BillingPersonEmail = string.Empty;
            Client.BillingPersonJobTitle = string.Empty;
        }
        OnPropertyChanged(nameof(Client));
    }

    [ObservableProperty]
    private bool isAdminPersonSameAsContact;
    partial void OnIsAdminPersonSameAsContactChanged(bool value)
    {
        if (value)
        {
            Client.AdminPersonFirstName = Client.ContactPersonFirstName;
            Client.AdminPersonLastName = Client.ContactPersonLastName;
            Client.AdminPersonCountryCode = Client.ContactPersonCountryCode;
            Client.AdminPersonPhone = Client.ContactPersonPhone;
            Client.AdminPersonEmail = Client.ContactPersonEmail;
            Client.AdminPersonJobTitle = Client.ContactPersonJobTitle;
        }
        else
        {
            Client.AdminPersonFirstName = string.Empty;
            Client.AdminPersonLastName = string.Empty;
            Client.AdminPersonCountryCode = null;
            Client.AdminPersonPhone = string.Empty;
            Client.AdminPersonEmail = string.Empty;
            Client.AdminPersonJobTitle = string.Empty;
        }
        OnPropertyChanged(nameof(Client));
    }

    // — helper properties :contentReference[oaicite:4]{index=4}

    public string SelectedCurrencyFlag =>
        $"{Client?.DefaultCurrency?.ToLower()}.png";

    public string LastUpdatedLocalTime =>
        Client?.LastUpdated?.ToLocalTime().ToString("f")
        ?? DateTime.UtcNow.ToLocalTime().ToString("f");

    /// <summary> Debug: dump any object as JSON </summary>
    public static Task SaveAsJsonAsync<T>(T obj, string path) =>
        File.WriteAllTextAsync(
            path,
            JsonSerializer.Serialize(obj,
                new JsonSerializerOptions { WriteIndented = true }));
}
