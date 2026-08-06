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
        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000/")
        });
        builder.Services.AddScoped<ApiService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
