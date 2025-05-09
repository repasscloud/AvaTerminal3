using AvaTerminal3.Services;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.ViewModels;
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
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddHttpClient("AvaAPI", client =>
		{
			client.BaseAddress = new Uri("https://api.avatools.internal/");
			client.DefaultRequestHeaders.Add("Accept", "application/json");
		});

		builder.Services.AddHttpClient("ExternalAPI", client =>
		{
			client.BaseAddress = new Uri("https://api.external.com/");
			client.DefaultRequestHeaders.Add("Accept", "application/json");
		});

		// Register services
		builder.Services.AddTransient<IAvaApiService, AvaApiService>();
		builder.Services.AddTransient<IExternalApiService, ExternalApiService>();
		builder.Services.AddSingleton<IAuthService, AuthService>(); // your implementation
		builder.Services.AddSingleton<LoginViewModel>();
		builder.Services.AddTransient<Views.LoginPage>();

		return builder.Build();
	}
}
