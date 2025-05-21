using AvaTerminal3.Models.Dto;

namespace AvaTerminal3.Services.Interfaces;

public interface IAvaApiService
{
    
    Task<string> GetClientAsync(string clientId);
    
    
    Task<bool> CreateClientAsync(AvaClientDto client);
    Task<AvaClientDto> GetClientByIdAsync(string clientId);
    Task UpdateClientAsync(string clientId, AvaClientDto client);
}
