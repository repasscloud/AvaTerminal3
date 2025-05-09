namespace AvaTerminal3.Helpers;

public static class ServiceHelper
{
    public static T Get<T>() => Current.GetService<T>()!;

    public static IServiceProvider Current =>
        Application.Current?.Handler?.MauiContext?.Services!;
}
