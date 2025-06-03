// using System.Net.Http.Json;
// using AvaTerminal3.Models.Kernel.Components;

// namespace AvaTerminal3.Helpers;

// public static class AttributeApi
// {
//     private const string BaseRoute = "/api/v1/avaterminal3version";

//     public static async Task<string> GetCurrentSupportedClientVersionAsync()
//     {
//         var client = ApiClient.CreateClient("DevAvaAPI");
//         var resp   = await client.GetAsync(BaseRoute);
//         resp.EnsureSuccessStatusCode();

//         var data = await resp.Content.ReadFromJsonAsync<AvaTerminal3VersionResponse>()
//                    ?? throw new InvalidOperationException("Empty response");

//         return data.AvaTerminal3Version;
//     }
// }
