using AvaTerminal3.Models.Dto;

namespace AvaTerminal3.Services.Interfaces;

public interface IAvaApiService
{
    Task<string> GetClientAsync(string clientId);


    Task<bool> CreateClientAsync(AvaClientDto client);
    Task<AvaClientDto> GetAvaClientBySearchEverythingAsync(string searchValue);
    Task<bool> UpdateClientAsync(AvaClientDto client);


    // for the NewAvaClientViewModel
    Task<List<string>> GetTaxIdsAsync();
    Task<List<string>> GetAvailableCountriesAsync();
    Task<List<string>> GetCountryDialCodesAsync();
    Task<List<string>> GetAvailableCurrencyCodesAsync();

    // for the ExistingAvaClientViewModel
    Task<List<SupportedDialCodeDto>> GetCountryDialCodes2Async();

    Task<string> MatchCountryDialCodeStringAsync(string? dialCode);

    // DBG
    Task<bool> IsApiHealthyAsync();

    Task<string> GetApiVersionStringAsync();
}
