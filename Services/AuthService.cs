using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Models.Kernel;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.Services;

public class AuthService : IAuthService
{
    private const string TokenKey = "jwt_token";
    private readonly IHttpClientFactory _factory;
    private readonly IEnvironmentService _env;

    public AuthService(IHttpClientFactory factory, IEnvironmentService env)
    {
        _factory = factory;
        _env     = env;
    }

    private HttpClient GetClient()
    => _factory.CreateClient(_env.IsDev ? "DevAvaAPI" : "AvaAPI");


    public async Task<AvaEmployeeLoginResponseDto> LoginAvaUserAsync(AvaEmployeeLoginDto dto)
    {
        var client = GetClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", dto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AvaEmployeeLoginResponseDto>();
        return result!;
    }

    public async Task SaveTokenAsync(string token)
    {
#if MACCATALYST
        try
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }
        catch
        {
            Preferences.Set(TokenKey, token);
        }
#else
        await SecureStorage.SetAsync(TokenKey, token);
#endif
    }

    public async Task<string?> GetTokenAsync()
    {
#if MACCATALYST
        try
        {
            return await SecureStorage.GetAsync(TokenKey)
                ?? Preferences.Get(TokenKey, null);
        }
        catch
        {
            return Preferences.Get(TokenKey, null);
        }
#else
        return await SecureStorage.GetAsync(TokenKey);
#endif
    }

    public void ClearToken()
    {
#if MACCATALYST
        SecureStorage.Remove(TokenKey);
        Preferences.Remove(TokenKey);
#else
        SecureStorage.Remove(TokenKey);
#endif
    }

    public async Task<bool> HasTokenAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task<JwtClaims?> GetClaimsAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token)) return null;

        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1];
            var json = DecodeBase64Url(payload);
            var root = JsonDocument.Parse(json).RootElement;

            return new JwtClaims
            {
                Subject = root.GetProperty("sub").GetString(),
                UniqueName = root.GetProperty("unique_name").GetString(),
                Role = root.GetProperty("role").GetString(),
                Jti = root.GetProperty("jti").GetString(),
                IssuedAt = ToDateTime(root, "iat"),
                NotBefore = ToDateTime(root, "nbf"),
                ExpiresAt = ToDateTime(root, "exp")
            };
        }
        catch
        {
            return null;
        }
    }

    private static string DecodeBase64Url(string input)
    {
        input = input.Replace('-', '+').Replace('_', '/');
        switch (input.Length % 4)
        {
            case 2: input += "=="; break;
            case 3: input += "="; break;
        }

        var bytes = Convert.FromBase64String(input);
        return Encoding.UTF8.GetString(bytes);
    }

    private static DateTime? ToDateTime(JsonElement root, string prop)
    {
        if (root.TryGetProperty(prop, out var val) && val.ValueKind == JsonValueKind.Number)
            return DateTimeOffset.FromUnixTimeSeconds(val.GetInt64()).UtcDateTime;

        return null;
    }
}