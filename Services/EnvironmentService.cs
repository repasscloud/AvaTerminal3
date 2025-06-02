using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.Services;

public class EnvironmentService : IEnvironmentService
{
    // default to Production (false) on startup
    public bool IsDev { get; set; } = false;
}
