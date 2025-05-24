using AvaTerminal3.Helpers;
using AvaTerminal3.Services;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.ViewModels;
using AvaTerminal3.ViewModels.CLT;
using AvaTerminal3.Views.CLT;
using AvaTerminal3.Views.CLT.SubViews;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AvaTerminal3;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddHttpClient("AvaAPI", client =>
        {
            client.BaseAddress = new Uri("https://dev.ava-api.uzhv.com");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddHttpClient("DevAvaAPI", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5165");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddHttpClient("ExternalAPI", client =>
        {
            client.BaseAddress = new Uri("https://api.external.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddHttpClient("GithubCDN", client =>
        {
            client.BaseAddress = new Uri("https://raw.githubusercontent.com/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("AvaTerminal2GithubCDNTool/1.0 (+https://github.com/repasscloud/AvaTerminal2)");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register services
        builder.Services.AddTransient<IAvaApiService, AvaApiService>();
        builder.Services.AddTransient<IExternalApiService, ExternalApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>(); // your implementation
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddTransient<Views.LoginPage>();

        // pages with DI
        builder.Services.AddTransient<ClientManagementPage>();
        builder.Services.AddTransient<ClientManagementViewModel>();
        builder.Services.AddTransient<NewAvaClientPage>();
        builder.Services.AddTransient<NewAvaClientViewModel>();
        builder.Services.AddTransient<ExistingAvaClientPage>();
        builder.Services.AddTransient<ExistingAvaClientViewModel>();

        builder.Services.AddSingleton<IPopupService, Services.PopupService>();
        builder.Services.AddSingleton<ISharedStateService, SharedStateService>();

        //return builder.Build();
        var app = builder.Build();

        // after Build() the container is ready:
        var factory = app.Services.GetRequiredService<IHttpClientFactory>();
        ApiClient.Initialize(factory);

        return app;
    }
}
