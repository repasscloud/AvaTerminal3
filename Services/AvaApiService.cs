using System.Net.Http.Headers;
using System.Net.Http.Json;
using AvaTerminal3.Helpers;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Models.Kernel.Client.Attribs;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.Services;

public class AvaApiService : IAvaApiService
{
    private readonly HttpClient _http;
    private readonly IAuthService _authService;

    public AvaApiService(IHttpClientFactory factory, IAuthService authService)
    {
        _http = factory.CreateClient("AvaAPI");
        _authService = authService;
    }

    public async Task<string> GetClientAsync(string clientId)
    {
        var response = await _http.GetAsync($"clients/{clientId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<bool> CreateClientAsync(AvaClientDto client)
    {
        string loggingPrefix = $"[AvaApiService.CreateClientAsync]";

        await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Starting client creation process.");

        // retrieve token from service
        string jwtToken = await _authService.GetTokenAsync() ?? string.Empty;

        if (!string.IsNullOrEmpty(jwtToken))
        {
            await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Obtained JWT token.");

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/avaclient/new-or-update")
                {
                    Content = JsonContent.Create(client)
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Sending POST request to /api/v1/avaclient/new-or-update with client ID: {client.ClientId}");

                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Failed to create client: {response.StatusCode} - {errorContent}");
                    return false;
                }

                await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Client created/updated successfully. Response status: {response.StatusCode}");
                return true;
            }
            catch (Exception ex)
            {
                await LogSinkService.WriteAsync(LogLevel.Fatal, $"{loggingPrefix} Exception during client creation: {ex.Message}");
                return false;
            }
        }

        await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Missing or invalid JWT Token with value: '{jwtToken}'.");
        return false;
    }

    public async Task<AvaClientDto> GetAvaClientBySearchEverythingAsync(string searchValue)
    {
        string loggingPrefix = $"[AvaApiService.GetAvaClientBySearchEverythingAsync]";
        string apiEndpoint = $"/api/v1/avaclient/search-everything/{searchValue}";

        await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Starting ava client (ge) get process.");

        // retrieve token from service
        string jwtToken = await _authService.GetTokenAsync() ?? string.Empty;

        if (!string.IsNullOrEmpty(jwtToken))
        {
            await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Obtained JWT token.");

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);
                // no request.Content = …, so we send an empty POST

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Sending POST request to {apiEndpoint} with search value: {searchValue}");

                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Failed to create client: {response.StatusCode} - {errorContent}");

                    throw new HttpRequestException(
                        $"SearchEverything API error {(int)response.StatusCode}: {errorContent}"
                    );
                }

                await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Client created/updated successfully. Response status: {response.StatusCode}");
                
                var dto = await response.Content
                        .ReadFromJsonAsync<AvaClientDto>()
                        ?? throw new InvalidOperationException("Empty response body");

                return dto;
            }
            catch (Exception ex)
            {
                await LogSinkService.WriteAsync(LogLevel.Fatal, $"{loggingPrefix} Exception during client creation: {ex.Message}");
                throw new HttpRequestException(
                    $"SearchEverything API call failed: {ex.Message}", 
                    ex              // preserve original as InnerException
                );
            }
        }

        await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Missing or invalid JWT Token with value: '{jwtToken}'.");

        throw new HttpRequestException(
            $"SearchEverything API call failed: {loggingPrefix} Missing or invalid JWT Token with value: '{jwtToken}'."
        );
    }

    Task IAvaApiService.UpdateClientAsync(string clientId, AvaClientDto client)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>> GetTaxIdsAsync()
    {
        string loggingPrefix = $"[AvaApiService.GetTaxIdsAsync]";
        await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Starting tax-ID retrieval.");

        // get JWT
        string jwtToken = await _authService.GetTokenAsync() ?? string.Empty;
        if (string.IsNullOrEmpty(jwtToken))
        {
            await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Missing or invalid JWT token.");
            return new List<string> { "ERROR" };
        }

        await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Obtained JWT token.");

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/attrib/taxids");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Sending GET to /api/v1/attrib/taxids");
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await LogSinkService.WriteAsync(LogLevel.Error,
                    $"{loggingPrefix} GET failed: {response.StatusCode} – {errorContent}");
                return new List<string> { "ERROR" };
            }

            // deserialize into your model
            var items = await response.Content
                                    .ReadFromJsonAsync<List<SupportedTaxId>>() 
                        ?? new List<SupportedTaxId>();

            // filter out any null/empty TaxIdType, and cast away the nullable
            var taxTypes = items
                .Where(x => !string.IsNullOrWhiteSpace(x.TaxIdType))
                .Select(x => x.TaxIdType!)      
                .ToList();

            // if we got at least one, return it
            if (taxTypes.Count > 0)
            {
                await LogSinkService.WriteAsync(
                    LogLevel.Info,
                    $"{loggingPrefix} Retrieved {taxTypes.Count} tax ID types successfully.");
                return taxTypes;
            }

            // otherwise, fallback to an error indicator (or empty list, up to you)
            await LogSinkService.WriteAsync(
                LogLevel.Error,
                $"{loggingPrefix} No tax ID types returned from API.");
            return new List<string> { "ERROR" };
        }
        catch (Exception ex)
        {
            await LogSinkService.WriteAsync(LogLevel.Fatal,
                $"{loggingPrefix} Exception during GET: {ex.Message}");
            return new List<string> { "ERROR" };
        }
    }

    public async Task<List<string>> GetAvailableCountriesAsync()
    {
        string loggingPrefix = $"[AvaApiService.GetAvailableCountriesAsync]";
        await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Starting available country retrieval.");

        // get JWT
        string jwtToken = await _authService.GetTokenAsync() ?? string.Empty;
        if (string.IsNullOrEmpty(jwtToken))
        {
            await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Missing or invalid JWT token.");
            return new List<string> { "ERROR" };
        }

        await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Obtained JWT token.");

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/attrib/countries");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Sending GET to /api/v1/attrib/countries");
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await LogSinkService.WriteAsync(LogLevel.Error,
                    $"{loggingPrefix} GET failed: {response.StatusCode} – {errorContent}");
                return new List<string> { "ERROR" };
            }

            // deserialize into your model
            var items = await response.Content
                                    .ReadFromJsonAsync<List<SupportedCountry>>() 
                        ?? new List<SupportedCountry>();

            // filter out any null/empty Country, and cast away the nullable
            var countries = items
                .Where(x => !string.IsNullOrWhiteSpace(x.Country))
                .Select(x => x.Country!)      
                .ToList();

            // if we got at least one, return it
            if (countries.Count > 0)
            {
                await LogSinkService.WriteAsync(
                    LogLevel.Info,
                    $"{loggingPrefix} Retrieved {countries.Count} countries successfully.");
                return countries;
            }

            // otherwise, fallback to an error indicator (or empty list, up to you)
            await LogSinkService.WriteAsync(
                LogLevel.Error,
                $"{loggingPrefix} No countries returned from API.");
            return new List<string> { "ERROR" };
        }
        catch (Exception ex)
        {
            await LogSinkService.WriteAsync(LogLevel.Fatal,
                $"{loggingPrefix} Exception during GET: {ex.Message}");
            return new List<string> { "ERROR" };
        }
    }

    public async Task<List<string>> GetCountryDialCodesAsync()
    {
        string loggingPrefix = $"[AvaApiService.GetCountryDialCodesAsync]";
        await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Starting available dial code retrieval.");

        // get JWT
        string jwtToken = await _authService.GetTokenAsync() ?? string.Empty;
        if (string.IsNullOrEmpty(jwtToken))
        {
            await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Missing or invalid JWT token.");
            return new List<string> { "ERROR" };
        }

        await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Obtained JWT token.");

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/attrib/dialcodes");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            await LogSinkService.WriteAsync(LogLevel.Debug,
                $"{loggingPrefix} Sending GET to /api/v1/attrib/dialcodes");
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await LogSinkService.WriteAsync(LogLevel.Error,
                    $"{loggingPrefix} GET failed: {response.StatusCode} – {errorContent}");
                return new List<string> { "ERROR" };
            }

            // deserialize into your model
            var items = await response.Content
                                    .ReadFromJsonAsync<List<SupportedDialCode>>() 
                        ?? new List<SupportedDialCode>();

            // format each as "(+<code>) <name>"
            var dialList = items
                .Select(x => $"(+{x.CountryCode}) {x.CountryName}")
                .ToList();

            if (dialList.Count > 0)
            {
                await LogSinkService.WriteAsync(LogLevel.Info,
                    $"{loggingPrefix} Retrieved {dialList.Count} dial codes successfully.");
                return dialList;
            }

            await LogSinkService.WriteAsync(LogLevel.Error,
                $"{loggingPrefix} No dial codes returned from API.");
            return new List<string> { "ERROR" };
        }
        catch (Exception ex)
        {
            await LogSinkService.WriteAsync(LogLevel.Fatal,
                $"{loggingPrefix} Exception during GET: {ex.Message}");
            return new List<string> { "ERROR" };
        }
    }

    public async Task<List<string>> GetAvailableCurrencyCodesAsync()
    {
        string loggingPrefix = $"[AvaApiService.GetAvailableCurrencyCodesAsync";
        await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Starting currency code retrieval.");

        // get JWT
        string jwtToken = await _authService.GetTokenAsync() ?? string.Empty;
        if (string.IsNullOrEmpty(jwtToken))
        {
            await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Missing or invalid JWT token.");
            return new List<string> { "ERROR" };
        }

        await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Obtained JWT token.");

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/attrib/currencies");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Sending GET to /api/v1/attrib/currencies");
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await LogSinkService.WriteAsync(LogLevel.Error,
                    $"{loggingPrefix} GET failed: {response.StatusCode} – {errorContent}");
                return new List<string> { "ERROR" };
            }

            // deserialize into your model
            var items = await response.Content
                                    .ReadFromJsonAsync<List<SupportedCurrency>>() 
                        ?? new List<SupportedCurrency>();

            // filter out any null/empty Iso4217, and cast away the nullable
            var currencyCodes = items
                .Where(x => !string.IsNullOrWhiteSpace(x.Iso4217))
                .Select(x => x.Iso4217!)      
                .ToList();

            // if we got at least one, return it
            if (currencyCodes.Count > 0)
            {
                await LogSinkService.WriteAsync(
                    LogLevel.Info,
                    $"{loggingPrefix} Retrieved {currencyCodes.Count} currency codes successfully.");
                return currencyCodes;
            }

            // otherwise, fallback to an error indicator (or empty list, up to you)
            await LogSinkService.WriteAsync(
                LogLevel.Error,
                $"{loggingPrefix} No currency codes returned from API.");
            return new List<string> { "ERROR" };
        }
        catch (Exception ex)
        {
            await LogSinkService.WriteAsync(LogLevel.Fatal,
                $"{loggingPrefix} Exception during GET: {ex.Message}");
            return new List<string> { "ERROR" };
        }
    }

    public async Task<List<SupportedDialCodeDto>> GetCountryDialCodes2Async()
    {
        string loggingPrefix = $"[AvaApiService.GetCountryDialCodes2Async]";
        await LogSinkService.WriteAsync(LogLevel.Info, $"{loggingPrefix} Starting available dial code retrieval.");

        // get JWT
        string jwtToken = await _authService.GetTokenAsync() ?? string.Empty;
        if (string.IsNullOrEmpty(jwtToken))
        {
            await LogSinkService.WriteAsync(LogLevel.Error, $"{loggingPrefix} Missing or invalid JWT token.");
            throw new HttpRequestException(
                $"{loggingPrefix} Missing or invalid JWT token."
            );
        }

        await LogSinkService.WriteAsync(LogLevel.Debug, $"{loggingPrefix} Obtained JWT token.");

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/attrib/dialcodes");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            await LogSinkService.WriteAsync(LogLevel.Debug,
                $"{loggingPrefix} Sending GET to /api/v1/attrib/dialcodes");
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await LogSinkService.WriteAsync(LogLevel.Error,
                    $"{loggingPrefix} GET failed: {response.StatusCode} – {errorContent}");
                throw new HttpRequestException(
                    $"{loggingPrefix} GET failed: {response.StatusCode} – {errorContent}"
                );
            }

            // deserialize into your model
            var dialList = await response.Content
                                .ReadFromJsonAsync<List<SupportedDialCodeDto>>() 
                            ?? new List<SupportedDialCodeDto>();

            if (dialList.Count > 0)
            {
                await LogSinkService.WriteAsync(LogLevel.Info,
                    $"{loggingPrefix} Retrieved {dialList.Count} dial codes successfully.");
                return dialList;
            }

            await LogSinkService.WriteAsync(LogLevel.Error,
                $"{loggingPrefix} No dial codes returned from API.");
            throw new HttpRequestException(
                $"{loggingPrefix} No dial codes returned from API."
            );
        }
        catch (Exception ex)
        {
            await LogSinkService.WriteAsync(LogLevel.Fatal,
                $"{loggingPrefix} Exception during GET: {ex.Message}");
            throw new HttpRequestException(
                $"{loggingPrefix} Exception during GET: {ex.Message}"
            );
        }
    }
}
