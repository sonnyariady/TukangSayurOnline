using System.Globalization;
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
        await js.InvokeVoidAsync("open", url, "_blank");
    }
}
