using AvaTerminal3.Models.Dto;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.Services;

public class AvaApiService : IAvaApiService
{
    private readonly HttpClient _http;

    public AvaApiService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("AvaAPI");
    }

    public async Task<string> GetClientAsync(string clientId)
    {
        var response = await _http.GetAsync($"clients/{clientId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    Task IAvaApiService.CreateClientAsync(AvaClientDto client)
    {
        throw new NotImplementedException();
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
