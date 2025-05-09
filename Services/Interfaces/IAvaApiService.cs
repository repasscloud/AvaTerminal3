namespace AvaTerminal3.Services.Interfaces;

public interface IAvaApiService
{
    Task<string> GetClientAsync(string clientId);
}