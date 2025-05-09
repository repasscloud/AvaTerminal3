using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.Services;

public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _http;

    public ExternalApiService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ExternalAPI");
    }

    public async Task<string> GetWeatherAsync(string city)
    {
        var response = await _http.GetAsync($"weather?city={city}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
