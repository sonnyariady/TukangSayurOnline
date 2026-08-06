using System.Reflection;
using Microsoft.Extensions.Configuration;
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

        // Load embedded appsettings.json stream into Configuration
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("TukangSayurOnline.Mobile.appsettings.json");
        if (stream != null)
        {
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            builder.Configuration.AddConfiguration(config);
        }

        builder.Services.AddMauiBlazorWebView();

        // Add MudBlazor Services 7.4.0
        builder.Services.AddMudServices();

        // Register App State and API Service from Shared RCL
        builder.Services.AddSingleton<AppStateService>();

        // Dynamic Platform-Aware API Base URL Resolution
        var apiBaseUrl = GetPlatformApiBaseUrl(builder.Configuration);
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

    private static string GetPlatformApiBaseUrl(IConfiguration config)
    {
        var customUrl = config["ApiBaseUrl"];

#if ANDROID
        var emulatorUrl = config["AndroidEmulatorBaseUrl"];
        return !string.IsNullOrEmpty(emulatorUrl) ? emulatorUrl : "http://10.0.2.2:5000/";
#else
        return !string.IsNullOrEmpty(customUrl) ? customUrl : "http://localhost:5000/";
#endif
    }
}
