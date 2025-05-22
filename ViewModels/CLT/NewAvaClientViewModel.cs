// ViewModels/CLT/NewAvaClientViewModel.cs
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using AvaTerminal3.Models.Dto;                  // your DTO
using AvaTerminal3.Services.Interfaces;         // IAvaApiService

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
            Client.TaxId = sel;
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
        var ok = await _avaApiService.CreateClientAsync(Client);
        if (ok)
            await Shell.Current.GoToAsync("..");
        else
            await _popupService.ShowNoticeAsync(
                "Error", "Unable to save client. Please try again.");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // - other stuff -
    public string SelectedCurrencyFlag
        => $"{Client?.DefaultCurrency?.ToLower()}.png";

}
