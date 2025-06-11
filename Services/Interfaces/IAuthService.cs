using AvaTerminal3.Models.Dto;
using AvaTerminal3.Models.Kernel;

namespace AvaTerminal3.Services.Interfaces;

public interface IAuthService
{
    Task<AvaEmployeeLoginResponseDto> LoginAvaUserAsync(AvaEmployeeLoginDto dto);


    // JWT Token
    Task SaveTokenAsync(string token);
    Task<JwtClaims?> GetClaimsAsync();
    Task<bool> HasTokenAsync();
    Task<string?> GetTokenAsync();
    void ClearToken();

    // user specific
    Task<string> GetLoggedInUserAsync();
}
