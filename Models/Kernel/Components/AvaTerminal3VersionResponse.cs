// Models/Kernel/Components/AvaTerminal3VersionResponse.cs
using System.Text.Json.Serialization;
namespace AvaTerminal3.Models.Kernel.Components;
public class AvaTerminal3VersionResponse
{
    [JsonPropertyName("currentVersion")]
    public string AvaTerminal3Version { get; set; } = string.Empty;
}