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

    public static string GetLogPath() => LogPath;
    public static string GetDumpFilePath() => DumpFilePath;
}
