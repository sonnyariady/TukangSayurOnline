using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TukangSayurOnline.Shared.Services;

namespace TukangSayurOnline.Mobile;

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
            });

        builder.Services.AddMauiBlazorWebView();

        // Add MudBlazor Services 7.4.0
        builder.Services.AddMudServices();

        // Register App State and API Service from Shared RCL
        builder.Services.AddSingleton<AppStateService>();

        // Dynamic Platform-Aware API Base URL Resolution
        var apiBaseUrl = GetPlatformApiBaseUrl();
        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
        });
        builder.Services.AddScoped<ApiService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static string GetPlatformApiBaseUrl()
    {
#if ANDROID
        // Android Emulator uses 10.0.2.2 to access host machine localhost:5000
        return "http://10.0.2.2:5000/";
#else
        // Windows Desktop, MacCatalyst, iOS Simulator
        return "http://localhost:5000/";
#endif
    }
}
