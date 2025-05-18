namespace AvaTerminal3.Models.Kernel;

public class JwtClaims
{
    public string? Subject { get; set; }
    public string? UniqueName { get; set; }
    public string? Role { get; set; }
    public string? Jti { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? NotBefore { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
}
