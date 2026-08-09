using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using TukangSayurOnline.Shared.Services;

namespace TukangSayurOnline.Web.Services;

public class WebGpsService : IGpsService
{
    private readonly IJSRuntime _js;

    public WebGpsService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync()
    {
        try
        {
            var pos = await _js.InvokeAsync<GpsPosDto>("getGPSCoordinates");
            if (pos != null && pos.Latitude != 0)
            {
                return (pos.Latitude, pos.Longitude);
            }
        }
        catch
        {
            // Ignore web fallback
        }

        return null;
    }

    private class GpsPosDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
