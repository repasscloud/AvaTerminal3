// File: ViewModels/CLT/NewAvaClientViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AvaTerminal3.Helpers;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.ViewModels.CLT;

public class NewAvaClientViewModel : INotifyPropertyChanged
{
    private readonly IAvaApiService _avaApiService;

    public event PropertyChangedEventHandler? PropertyChanged;

    private AvaClientDto _client = new();
    public AvaClientDto Client
    {
        get => _client;
        set => SetField(ref _client, value);
    }

    public string? LastUpdatedLocalTime =>
        Client?.LastUpdated.HasValue == true
            ? Client.LastUpdated.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
            : null;

    public ICommand SaveCommand { get; }

    public bool IsEditMode { get; private set; }


    // Address.Country
    public List<string> CountryList { get; } = CountryHelper.GetCountryList();
    public string? SelectedCountry
    {
        get => Client.Country;
        set
        {
            if (Client.Country != value)
            {
                Client.Country = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCountry)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
            }
        }
    }

    // Tax.Registration
    public List<string> TaxRegistrationList { get; } = TaxRegistrationHelper.GetTaxRegistrationList();
    public string? SelectedTaxRegistration
    {
        get => Client.TaxIdType;
        set
        {
            if (Client.TaxIdType != value)
            {
                Client.TaxIdType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTaxRegistration)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
            }
        }
    }


    public List<string> CurrencyDisplayList { get; } = CurrencyFlagHelper.GetCurrencyDisplayList();

    public string? SelectedCurrencyDisplay
    {
        get => CurrencyDisplayList.FirstOrDefault(item =>
            item.StartsWith(Client.DefaultCurrency ?? "", StringComparison.OrdinalIgnoreCase));
        set
        {
            var currencyCode = CurrencyFlagHelper.GetCurrencyCodeFromDisplay(value ?? "");
            if (Client.DefaultCurrency != currencyCode)
            {
                Client.DefaultCurrency = currencyCode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCurrencyDisplay)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCurrencyFlag)));
            }
        }
    }

    public string? SelectedCurrencyFlag =>
        CurrencyFlagHelper.GetFlagImage(Client.DefaultCurrency ?? "");

    public List<string> CountryDisplayList { get; } = CountryCodeHelper.GetCountryDisplayList();

    public string? SelectedCountryDisplay
    {
        get => CountryDisplayList.FirstOrDefault(item =>
            item.Contains($"(+{Client.ContactPersonCountryCode ?? ""})"));
        set
        {
            var dialCode = CountryCodeHelper.GetDialCodeFromDisplay(value ?? "");
            if (Client.ContactPersonCountryCode != dialCode)
            {
                Client.ContactPersonCountryCode = dialCode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCountryDisplay)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
            }
        }
    }

    public NewAvaClientViewModel(IAvaApiService avaApiService)
    {
        _avaApiService = avaApiService;
        SaveCommand = new Command(async () => await SaveClientAsync());
    }

    // creating a new AvaClientDto will automatically assign:
    //  - .ClientId
    //  - .LicenseAgreementId (this will be set to pending)
    //  - .DefaultTravelPolicyId (this will be set to all defaults, takes details from the AvaClient record too)
    public void NewClient()
    {
        Client = new AvaClientDto();
    }

    public async Task SaveClientAsync()
    {
        // response is bool, true is OK - navigate to main page again
        var response = await _avaApiService.CreateClientAsync(Client);

    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
