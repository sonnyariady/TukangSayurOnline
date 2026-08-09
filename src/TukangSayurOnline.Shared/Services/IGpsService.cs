using System.Threading.Tasks;

namespace TukangSayurOnline.Shared.Services;

public interface IGpsService
{
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
}
