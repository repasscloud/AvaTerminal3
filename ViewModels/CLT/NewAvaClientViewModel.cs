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

    // billing copy check box
    private bool _isBillingPersonSameAsContact;
    public bool IsBillingPersonSameAsContact
    {
        get => _isBillingPersonSameAsContact;
        set
        {
            if (_isBillingPersonSameAsContact != value)
            {
                _isBillingPersonSameAsContact = value;
                OnPropertyChanged();

                if (value)
                {
                    CopyContactToBilling();
                }
            }
        }
    }

    // admin copy check box
    private bool _isAdminPersonSameAsContact;
    public bool IsAdminPersonSameAsContact
    {
        get => _isAdminPersonSameAsContact;
        set
        {
            if (_isAdminPersonSameAsContact != value)
            {
                _isAdminPersonSameAsContact = value;
                OnPropertyChanged();

                if (value)
                {
                    CopyContactToAdmin();
                }
            }
        }
    }

    public string? LastUpdatedLocalTime =>
        Client?.LastUpdated.HasValue == true
            ? Client.LastUpdated.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
            : null;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

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

    public string? SelectedContactCountryDisplay
    {
        get => CountryDisplayList.FirstOrDefault(item =>
            item.Contains(Client.ContactPersonCountryCode ?? ""));
        set
        {
            var dialCode = CountryCodeHelper.GetDialCodeFromDisplay(value ?? "");
            if (Client.ContactPersonCountryCode != dialCode)
            {
                Client.ContactPersonCountryCode = dialCode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedContactCountryDisplay)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
            }
        }
    }

    public string? SelectedBillingCountryDisplay
    {
        get => CountryDisplayList.FirstOrDefault(item =>
            item.Contains(Client.BillingPersonCountryCode ?? ""));
        set
        {
            var dialCode = CountryCodeHelper.GetDialCodeFromDisplay(value ?? "");
            if (Client.BillingPersonCountryCode != dialCode)
            {
                Client.BillingPersonCountryCode = dialCode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedBillingCountryDisplay)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
            }
        }
    }

    public string? SelectedAdminCountryDisplay
    {
        get => CountryDisplayList.FirstOrDefault(item =>
            item.Contains(Client.AdminPersonCountryCode ?? ""));
        set
        {
            var dialCode = CountryCodeHelper.GetDialCodeFromDisplay(value ?? "");
            if (Client.AdminPersonCountryCode != dialCode)
            {
                Client.AdminPersonCountryCode = dialCode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAdminCountryDisplay)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
            }
        }
    }


    public NewAvaClientViewModel(IAvaApiService avaApiService)
    {
        _avaApiService = avaApiService;
        SaveCommand = new Command(async () => await SaveClientAsync());
        CancelCommand = new Command(async () =>
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
                await Shell.Current.Navigation.PopAsync(); // smoother back animation
            else
                await Shell.Current.GoToAsync(".."); // fallback in case it's a root
        });
    }

    private void CopyContactToBilling()
    {
        Client.BillingPersonFirstName = Client.ContactPersonFirstName;
        Client.BillingPersonLastName = Client.ContactPersonLastName;
        Client.BillingPersonJobTitle = Client.ContactPersonJobTitle;
        Client.BillingPersonEmail = Client.ContactPersonEmail;
        Client.BillingPersonCountryCode = Client.ContactPersonCountryCode;
        Client.BillingPersonPhone = Client.ContactPersonPhone;

        // Add more mappings here if needed...
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
    }

    private void CopyContactToAdmin()
    {
        Client.AdminPersonFirstName = Client.ContactPersonFirstName;
        Client.AdminPersonLastName = Client.ContactPersonLastName;
        Client.AdminPersonJobTitle = Client.ContactPersonJobTitle;
        Client.AdminPersonEmail = Client.ContactPersonEmail;
        Client.AdminPersonCountryCode = Client.ContactPersonCountryCode;
        Client.AdminPersonPhone = Client.ContactPersonPhone;

        // Add more mappings here if needed...
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
    }


    // creating a new AvaClientDto will automatically assign:
    //  - .ClientId
    //  - .LicenseAgreementId (this will be set to pending)
    //  - .DefaultTravelPolicyId (this will be set to all defaults, takes details from the AvaClient record too)
    public void NewClient()
    {
        Client = new AvaClientDto
        {
            ContactPersonCountryCode = CountryCodeHelper.GetDialCodeFromDisplay(CountryDisplayList.FirstOrDefault() ?? ""),
            BillingPersonCountryCode = CountryCodeHelper.GetDialCodeFromDisplay(CountryDisplayList.FirstOrDefault() ?? ""),
            AdminPersonCountryCode = CountryCodeHelper.GetDialCodeFromDisplay(CountryDisplayList.FirstOrDefault() ?? "")
        };

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedContactCountryDisplay)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedBillingCountryDisplay)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAdminCountryDisplay)));
    }

    public async Task SaveClientAsync()
    {
        // response is bool, true is OK - navigate to main page again
        var response = await _avaApiService.CreateClientAsync(Client);
        if (response)
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
                await Shell.Current.Navigation.PopAsync(); // smoother back animation
            else
                await Shell.Current.GoToAsync(".."); // fallback in case it's a root
        }
        else
        {
            var currentApp = Application.Current;
            var window = currentApp?.Windows.FirstOrDefault();
            if (window is not null && window.Page is not null)
            {
                await window.Page.DisplayAlert(
                    "New Client Error",
                    "Unable to create new Ava Client. Try again or contact support.",
                    "OK"
                );
            }
        }
        // var dumpFilePath = await LogSinkService.ExportToTempJsonAsync(Client);

        // var currentApp = Application.Current;
        // var window = currentApp?.Windows.FirstOrDefault();

        // if (window is not null && window.Page is not null)
        // {
        //     bool confirmed = await window.Page.DisplayAlert(
        //         "Dump Created",
        //         $"{dumpFilePath}",
        //         "DO NOT CLICK",
        //         "OK");

        //     if (confirmed)
        //     {
        //         if (Shell.Current.Navigation.NavigationStack.Count > 1)
        //             await Shell.Current.Navigation.PopAsync(); // smoother back animation
        //         else
        //             await Shell.Current.GoToAsync(".."); // fallback in case it's a root
        //     }
        // }
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
