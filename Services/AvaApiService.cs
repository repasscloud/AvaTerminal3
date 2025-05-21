using System.Net.Http.Headers;
using System.Net.Http.Json;
using AvaTerminal3.Helpers;
using AvaTerminal3.Models.Dto;
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


    Task<AvaClientDto> IAvaApiService.GetClientByIdAsync(string clientId)
    {
        throw new NotImplementedException();
    }

    Task IAvaApiService.UpdateClientAsync(string clientId, AvaClientDto client)
    {
        throw new NotImplementedException();
    }
}
