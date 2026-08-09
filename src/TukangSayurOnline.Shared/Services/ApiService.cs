using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace TukangSayurOnline.Shared.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly AppStateService _appState;

    public ApiService(HttpClient http, AppStateService appState)
    {
        _http = http;
        _appState = appState;
        if (_http.BaseAddress == null)
        {
            _http.BaseAddress = new Uri("http://localhost:5000/");
        }
    }

    private void EnsureAuthHeader()
    {
        if (!string.IsNullOrEmpty(_appState.Token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _appState.Token);
        }
    }

    #region Auth
    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("api/auth/register", request);
            return await res.Content.ReadFromJsonAsync<AuthResponseDto>();
        }
        catch (Exception ex)
        {
            return new AuthResponseDto(false, $"Gagal koneksi ke server: {ex.Message}", "", 0, "", "", "", null);
        }
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var res = await _http.PostAsJsonAsync("api/auth/login", request);
            return await res.Content.ReadFromJsonAsync<AuthResponseDto>();
        }
        catch (Exception ex)
        {
            return new AuthResponseDto(false, $"Gagal koneksi ke server: {ex.Message}", "", 0, "", "", "", null);
        }
    }
    #endregion

    #region Admin & Products
    public async Task<List<ProductClientDto>> GetProductsAsync(string? category = null, string? search = null)
    {
        try
        {
            EnsureAuthHeader();
            var url = "api/admin/products?";
            if (!string.IsNullOrEmpty(category)) url += $"category={Uri.EscapeDataString(category)}&";
            if (!string.IsNullOrEmpty(search)) url += $"search={Uri.EscapeDataString(search)}";

            var products = await _http.GetFromJsonAsync<List<ProductClientDto>>(url);
            return products ?? new List<ProductClientDto>();
        }
        catch
        {
            return GetFallbackProducts();
        }
    }

    public async Task<ProductClientDto?> CreateProductAsync(CreateProductClientRequest request)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsJsonAsync("api/admin/products", request);
            return await res.Content.ReadFromJsonAsync<ProductClientDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<ProductClientDto?> UpdateProductAsync(int id, UpdateProductClientRequest request)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PutAsJsonAsync($"api/admin/products/{id}", request);
            return await res.Content.ReadFromJsonAsync<ProductClientDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.DeleteAsync($"api/admin/products/{id}");
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<PopularProductReportClientDto>> GetPopularProductsAsync()
    {
        try
        {
            EnsureAuthHeader();
            var report = await _http.GetFromJsonAsync<List<PopularProductReportClientDto>>("api/admin/reports/popular-products");
            return report ?? new List<PopularProductReportClientDto>();
        }
        catch
        {
            return new List<PopularProductReportClientDto>
            {
                new(1, "Bayam Hijau Segar", "Sayuran Hijau", 55, 192500m),
                new(4, "Tomat Buah Merah", "Sayuran Buah", 32, 448000m),
                new(5, "Cabai Rawit Merah", "Bumbu & Rempah", 18, 810000m)
            };
        }
    }

    public async Task<List<EmptyStockReportClientDto>> GetEmptyStockReportsAsync()
    {
        try
        {
            EnsureAuthHeader();
            var report = await _http.GetFromJsonAsync<List<EmptyStockReportClientDto>>("api/admin/reports/empty-stocks");
            return report ?? new List<EmptyStockReportClientDto>();
        }
        catch
        {
            return new List<EmptyStockReportClientDto>
            {
                new(1, "Sayur Segar Mang Udin", "Mang Udin Sutarman", 5, "Cabai Rawit Merah", "Bumbu & Rempah"),
                new(2, "Lapak Sayur Bang Budi", "Bang Budi Santoso", 7, "Kentang Dieng Super", "Sayuran Umbi")
            };
        }
    }
    #endregion

    #region Tukang Sayur
    public async Task<List<StockItemClientDto>> GetMyStocksAsync(int tukangSayurId)
    {
        try
        {
            EnsureAuthHeader();
            var stocks = await _http.GetFromJsonAsync<List<StockItemClientDto>>($"api/tukangsayur/{tukangSayurId}/stocks");
            return stocks ?? new List<StockItemClientDto>();
        }
        catch
        {
            return GetFallbackStocks();
        }
    }

    public async Task<bool> UpdateStockAsync(int tukangSayurId, int productId, double quantity, decimal price)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsJsonAsync($"api/tukangsayur/{tukangSayurId}/stocks/update", new { ProductId = productId, StockQuantity = quantity, PricePerUnit = price });
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<StockTransactionClientDto?> RestockInAsync(int tukangSayurId, RestockInClientRequest request)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsJsonAsync($"api/tukangsayur/{tukangSayurId}/restock-in", request);
            return await res.Content.ReadFromJsonAsync<StockTransactionClientDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<StockTransactionClientDto?> RecordSaleOutAsync(int tukangSayurId, RecordSaleClientRequest request)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsJsonAsync($"api/tukangsayur/{tukangSayurId}/sale-out", request);
            return await res.Content.ReadFromJsonAsync<StockTransactionClientDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<IncomeSummaryClientDto?> GetIncomeSummaryAsync(int tukangSayurId)
    {
        try
        {
            EnsureAuthHeader();
            return await _http.GetFromJsonAsync<IncomeSummaryClientDto>($"api/tukangsayur/{tukangSayurId}/income-summary");
        }
        catch
        {
            return new IncomeSummaryClientDto(
                350000m,
                420000m,
                180000m,
                14,
                new List<StockTransactionClientDto>
                {
                    new(1, 1, "Bayam Hijau Segar", "RestockIn", 30, 3000m, 90000m, "Kulakan Pasar Induk", DateTime.Now.AddHours(-5)),
                    new(2, 1, "Bayam Hijau Segar", "SaleOut", 5, 4000m, 20000m, "Penjualan Pelanggan", DateTime.Now.AddHours(-2))
                }
            );
        }
    }

    public async Task<bool> ToggleOnlineAsync(int tukangSayurId, bool isOnline, double lat, double lng, string locationName)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsJsonAsync($"api/tukangsayur/{tukangSayurId}/toggle-online", new { IsOnline = isOnline, Latitude = lat, Longitude = lng, LocationName = locationName });
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return true;
        }
    }

    public async Task<List<OrderClientDto>> GetVendorOrdersAsync(int tukangSayurId)
    {
        try
        {
            EnsureAuthHeader();
            var orders = await _http.GetFromJsonAsync<List<OrderClientDto>>($"api/tukangsayur/{tukangSayurId}/orders");
            return orders ?? new List<OrderClientDto>();
        }
        catch
        {
            return new List<OrderClientDto>();
        }
    }

    public async Task<bool> CompleteVendorOrderAsync(int tukangSayurId, int orderId)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsync($"api/tukangsayur/{tukangSayurId}/orders/{orderId}/complete", null);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CancelVendorOrderAsync(int tukangSayurId, int orderId)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsync($"api/tukangsayur/{tukangSayurId}/orders/{orderId}/cancel", null);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region Pelanggan
    public async Task<List<NearbyVendorSummaryClientDto>> GetNearbyVendorsAsync(double lat, double lng)
    {
        try
        {
            EnsureAuthHeader();
            var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lngStr = lng.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"api/pelanggan/nearby-vendors?userLat={latStr}&userLng={lngStr}";
            var result = await _http.GetFromJsonAsync<List<NearbyVendorSummaryClientDto>>(url);
            return result ?? new List<NearbyVendorSummaryClientDto>();
        }
        catch
        {
            return GetFallbackNearbyVendors();
        }
    }

    public async Task<VendorCatalogClientDto?> GetVendorCatalogAsync(int vendorId, double lat, double lng)
    {
        try
        {
            EnsureAuthHeader();
            var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lngStr = lng.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"api/pelanggan/vendors/{vendorId}/catalog?userLat={latStr}&userLng={lngStr}";
            return await _http.GetFromJsonAsync<VendorCatalogClientDto>(url);
        }
        catch
        {
            return GetFallbackVendorCatalog(vendorId);
        }
    }

    public async Task<List<NearbyVendorProductClientDto>> SearchNearbyProductsAsync(double lat, double lng, string? query, string? category)
    {
        try
        {
            EnsureAuthHeader();
            var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lngStr = lng.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"api/pelanggan/search-nearby?userLat={latStr}&userLng={lngStr}&";
            if (!string.IsNullOrEmpty(query)) url += $"query={Uri.EscapeDataString(query)}&";
            if (!string.IsNullOrEmpty(category)) url += $"category={Uri.EscapeDataString(category)}";

            var result = await _http.GetFromJsonAsync<List<NearbyVendorProductClientDto>>(url);
            return result ?? new List<NearbyVendorProductClientDto>();
        }
        catch
        {
            return GetFallbackNearbyProducts();
        }
    }

    public async Task<OrderClientDto?> CreateOrderAsync(int customerId, CreateOrderClientRequest request)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsJsonAsync($"api/pelanggan/orders?customerId={customerId}", request);
            return await res.Content.ReadFromJsonAsync<OrderClientDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<OrderClientDto>> GetMyOrdersAsync(int customerId)
    {
        try
        {
            EnsureAuthHeader();
            var orders = await _http.GetFromJsonAsync<List<OrderClientDto>>($"api/pelanggan/orders/my-orders?customerId={customerId}");
            return orders ?? new List<OrderClientDto>();
        }
        catch
        {
            return new List<OrderClientDto>();
        }
    }

    public async Task<bool> UpdateCustomerLocationAsync(int customerId, double lat, double lng)
    {
        try
        {
            EnsureAuthHeader();
            var res = await _http.PostAsJsonAsync($"api/pelanggan/{customerId}/update-location", new { Latitude = lat, Longitude = lng });
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region Fallback Data Helpers
    private List<NearbyVendorSummaryClientDto> GetFallbackNearbyVendors()
    {
        return new List<NearbyVendorSummaryClientDto>
        {
            new(1, "Sayur Segar Mang Udin", "Mang Udin Sutarman", "081987654321", -6.1550, 106.9020, 1.2, true, "Kelapa Gading Permai", 6),
            new(2, "Lapak Sayur Bang Budi", "Bang Budi Santoso", "081777888999", -6.2250, 106.8550, 3.5, true, "Tebet Eco Park", 6)
        };
    }

    private VendorCatalogClientDto GetFallbackVendorCatalog(int vendorId)
    {
        var stocks = GetFallbackStocks();
        return new VendorCatalogClientDto(
            1, "Sayur Segar Mang Udin", "Mang Udin Sutarman", "081987654321", -6.1550, 106.9020, 1.2, true, "Kelapa Gading Permai", stocks
        );
    }

    private List<ProductClientDto> GetFallbackProducts()
    {
        return new List<ProductClientDto>
        {
            new(1, "Bayam Hijau Segar", "Sayuran Hijau", "ikat", "Bayam petik baru segar kaya zat besi", "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400", 3500m),
            new(2, "Kangkung Darat", "Sayuran Hijau", "ikat", "Kangkung segar daun muda", "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400", 3000m),
            new(3, "Wortel Manis Dieng", "Sayuran Umbi", "kg", "Wortel Dieng segar manis renyah", "https://images.unsplash.com/photo-1598170845058-12ef4a457939?w=400", 12000m),
            new(4, "Tomat Buah Merah", "Sayuran Buah", "kg", "Tomat merah segar cocok untuk masakan & jus", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=400", 14000m),
            new(5, "Cabai Rawit Merah", "Bumbu & Rempah", "kg", "Cabai rawit super pedas segar", "https://images.unsplash.com/photo-1588252303782-cb80119abd6d?w=400", 45000m),
            new(6, "Bawang Merah Brebes", "Bumbu & Rempah", "kg", "Bawang merah olahan Brebes harum", "https://images.unsplash.com/photo-1618512496248-a07fe83aa8cf?w=400", 38000m)
        };
    }

    private List<StockItemClientDto> GetFallbackStocks()
    {
        return new List<StockItemClientDto>
        {
            new(1, 1, "Bayam Hijau Segar", "Sayuran Hijau", "ikat", "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400", 20, 4000m, DateTime.Now),
            new(2, 2, "Kangkung Darat", "Sayuran Hijau", "ikat", "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400", 15, 3500m, DateTime.Now),
            new(3, 3, "Wortel Manis Dieng", "Sayuran Umbi", "kg", "https://images.unsplash.com/photo-1598170845058-12ef4a457939?w=400", 10, 13000m, DateTime.Now),
            new(4, 4, "Tomat Buah Merah", "Sayuran Buah", "kg", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=400", 8, 15000m, DateTime.Now)
        };
    }

    private List<NearbyVendorProductClientDto> GetFallbackNearbyProducts()
    {
        return new List<NearbyVendorProductClientDto>
        {
            new(1, "Sayur Segar Mang Udin", "Mang Udin Sutarman", "081987654321", -6.1550, 106.9020, 1.2, true, 1, "Bayam Hijau Segar", "Sayuran Hijau", "ikat", "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400", 20, 4000m),
            new(1, "Sayur Segar Mang Udin", "Mang Udin Sutarman", "081987654321", -6.1550, 106.9020, 1.2, true, 3, "Wortel Manis Dieng", "Sayuran Umbi", "kg", "https://images.unsplash.com/photo-1598170845058-12ef4a457939?w=400", 10, 13000m),
            new(2, "Lapak Sayur Bang Budi", "Bang Budi Santoso", "081777888999", -6.2250, 106.8550, 3.5, true, 4, "Tomat Buah Merah", "Sayuran Buah", "kg", "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=400", 15, 14500m),
            new(2, "Lapak Sayur Bang Budi", "Bang Budi Santoso", "081777888999", -6.2250, 106.8550, 3.5, true, 5, "Cabai Rawit Merah", "Bumbu & Rempah", "kg", "https://images.unsplash.com/photo-1588252303782-cb80119abd6d?w=400", 5, 47000m)
        };
    }
    #endregion
}

#region Client DTO Definitions
public record RegisterRequestDto(string FullName, string Email, string Phone, string Password, int Role, string ShopName, string Address, double Latitude, double Longitude);
public record LoginRequestDto(string Email, string Password);
public record AuthResponseDto(bool Success, string Message, string Token, int UserId, string FullName, string Email, string Role, int? TukangSayurId);

public record ProductClientDto(int Id, string Name, string Category, string Unit, string Description, string ImageUrl, decimal DefaultPrice);
public record CreateProductClientRequest(string Name, string Category, string Unit, string Description, string ImageUrl, decimal DefaultPrice);
public record UpdateProductClientRequest(int Id, string Name, string Category, string Unit, string Description, string ImageUrl, decimal DefaultPrice);

public record StockItemClientDto(int Id, int ProductId, string ProductName, string Category, string Unit, string ImageUrl, double StockQuantity, decimal PricePerUnit, DateTime UpdatedAt);
public record RestockInClientRequest(int ProductId, double Quantity, decimal UnitPrice, string Notes);
public record RecordSaleClientRequest(int ProductId, double Quantity, decimal UnitPrice, string Notes);

public record StockTransactionClientDto(int Id, int ProductId, string ProductName, string Type, double Quantity, decimal UnitPrice, decimal TotalAmount, string Notes, DateTime TransactionDate);
public record IncomeSummaryClientDto(decimal CurrentBalance, decimal TotalIncomeAllTime, decimal TotalRestockExpenseAllTime, int TotalSalesCount, List<StockTransactionClientDto> RecentTransactions);

public record PopularProductReportClientDto(int ProductId, string ProductName, string Category, double TotalQuantitySold, decimal TotalRevenue);
public record EmptyStockReportClientDto(int TukangSayurId, string ShopName, string VendorName, int ProductId, string ProductName, string Category);

public record NearbyVendorSummaryClientDto(
    int TukangSayurId, string ShopName, string OwnerName, string Phone,
    double Latitude, double Longitude, double DistanceKm, bool IsOnline,
    string LocationName, int TotalProductsCount
);

public record VendorCatalogClientDto(
    int TukangSayurId, string ShopName, string OwnerName, string Phone,
    double Latitude, double Longitude, double DistanceKm, bool IsOnline,
    string LocationName, List<StockItemClientDto> Products
);

public record NearbyVendorProductClientDto(
    int TukangSayurId, string ShopName, string VendorName, string VendorPhone,
    double VendorLatitude, double VendorLongitude, double DistanceKm, bool IsOnline,
    int ProductId, string ProductName, string Category, string Unit, string ImageUrl,
    double AvailableStock, decimal PricePerUnit
);

public record CreateOrderClientRequest(int TukangSayurId, double MeetLatitude, double MeetLongitude, string MeetAddress, List<OrderItemClientRequest> Items);
public record OrderItemClientRequest(int ProductId, double Quantity, decimal UnitPrice);
public record OrderClientDto(int Id, int CustomerId, string CustomerName, int TukangSayurId, string ShopName, decimal TotalAmount, string Status, double MeetLatitude, double MeetLongitude, string MeetAddress, DateTime OrderDate, List<OrderItemClientDto> Items);
public record OrderItemClientDto(int ProductId, string ProductName, string Unit, double Quantity, decimal UnitPrice, decimal SubTotal);
#endregion
