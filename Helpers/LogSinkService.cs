using System.Text.Json;

namespace AvaTerminal3.Helpers;

public static class LogSinkService
{
    private static readonly string LogPath = Path.Combine(FileSystem.AppDataDirectory, "log.txt");
    private static readonly string DumpFilePath = Path.Combine(FileSystem.AppDataDirectory, "dumpfile.txt");

    public static async Task WriteAsync(LogLevel level, string message)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level.ToString().ToUpper()}] {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(LogPath, logEntry);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LogSinkService] Failed to write log: {ex}");
        }
    }

    public static async Task DumpJsonAsync(LogLevel level, object jsonObj)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DumpFilePath)!);

            var timestamp = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level.ToString().ToUpper()}]{Environment.NewLine}";
            var jsonString = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions
            {
                WriteIndented = true
            }) + Environment.NewLine;

            await File.AppendAllTextAsync(DumpFilePath, timestamp);
            await File.AppendAllTextAsync(DumpFilePath, jsonString);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LogSinkService] Failed to write dumpfile: {ex}");
        }
    }

    public static async Task<string> ExportToTempJsonAsync<T>(T data, string? filename = null)
    {
        var fileName = filename ?? $"export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        var tempPath = Path.Combine(Path.GetTempPath(), fileName);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await using var stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, data, options);

        return tempPath;
    }

    public static void DeleteLogFile() => File.Delete(GetLogPath());

    public static string GetLogPath() => LogPath;
    public static string GetDumpFilePath() => DumpFilePath;
}
