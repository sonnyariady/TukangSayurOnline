using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace TukangSayurOnline.Shared.Services;

public static class MapLauncher
{
    public static async Task OpenNavigationAsync(IJSRuntime js, double originLat, double originLng, double destLat, double destLng)
    {
        var originStr = originLat.ToString(CultureInfo.InvariantCulture);
        var originLngStr = originLng.ToString(CultureInfo.InvariantCulture);
        var destStr = destLat.ToString(CultureInfo.InvariantCulture);
        var destLngStr = destLng.ToString(CultureInfo.InvariantCulture);

        var url = $"https://www.google.com/maps/dir/?api=1&origin={originStr},{originLngStr}&destination={destStr},{destLngStr}&travelmode=driving";

        try
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var launcherType = asm.GetType("Microsoft.Maui.ApplicationModel.Launcher");
                if (launcherType != null)
                {
                    var defaultProp = launcherType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
                    var launcherInstance = defaultProp?.GetValue(null);
                    if (launcherInstance != null)
                    {
                        var openMethod = launcherInstance.GetType().GetMethod("OpenAsync", new[] { typeof(Uri) });
                        if (openMethod != null)
                        {
                            var task = openMethod.Invoke(launcherInstance, new object[] { new Uri(url) }) as Task;
                            if (task != null)
                            {
                                await task;
                                return;
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // Fallback
        }

        try
        {
            await js.InvokeVoidAsync("openMapUrl", url);
        }
        catch
        {
            // Fallback
        }
    }
}
