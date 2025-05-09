namespace AvaTerminal3.Services.Interfaces;

public interface IExternalApiService
{
    Task<string> GetWeatherAsync(string city);
}
