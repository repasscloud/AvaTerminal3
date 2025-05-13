using System.Text.Json;

namespace AvaTerminal3.Helpers;

public static class LogSinkService
{
    private static readonly string LogPath = Path.Combine(FileSystem.AppDataDirectory, "log.txt");
    private static readonly string DumpFilePath = Path.Combine(FileSystem.AppDataDirectory, "dumpfile.txt");

    public static void Write(string message)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LogSinkService] Failed to write log: {ex}");
        }
    }

    public static void DumpJson(object jsonObj)
    {
        // Serialize whatever object you pass in
        var jsonString = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DumpFilePath)!);
            File.AppendAllText(DumpFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{Environment.NewLine}");
            File.AppendAllText(DumpFilePath, $"{jsonString}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LogSinkService] Failed to write dumpfile: {ex}");
        }
    }

    public static string GetLogPath() => LogPath;
    public static string GetDumpFilePath() => DumpFilePath;
}
