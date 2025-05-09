namespace AvaTerminal3.Services.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
}
