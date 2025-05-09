using AvaTerminal3.Services.Interfaces;
using System.Net.Http.Json;

namespace AvaTerminal3.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("AvaAPI");
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var payload = new
        {
            username,
            password
        };

        try
        {
            var res = await _http.PostAsJsonAsync("auth/login", payload);

            if (!res.IsSuccessStatusCode)
                return false;

            var token = await res.Content.ReadAsStringAsync();

            // Store it securely - later use SecureStorage or Preferences
            Preferences.Set("access_token", token);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
