using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TukangSayurOnline.Api.Data;
using TukangSayurOnline.Api.Data.Entities;
using TukangSayurOnline.Api.Models;

namespace TukangSayurOnline.Api.Services;

public interface IPelangganService
{
    Task<List<NearbyVendorProductDto>> SearchNearbyProductsAsync(double userLat, double userLng, string? query, string? category, double maxDistanceKm = 25);
    Task<OrderDto> CreateOrderAsync(int customerId, CreateOrderRequest request);
    Task<List<OrderDto>> GetMyOrdersAsync(int customerId);
    Task<bool> UpdateCustomerLocationAsync(int customerId, double lat, double lng);
}

public class PelangganService : IPelangganService
{
    private readonly AppDbContext _context;

    public PelangganService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NearbyVendorProductDto>> SearchNearbyProductsAsync(double userLat, double userLng, string? query, string? category, double maxDistanceKm = 25)
    {
        // Default to Jakarta central coordinates if 0
        if (userLat == 0 && userLng == 0)
        {
            userLat = -6.2088;
            userLng = 106.8456;
        }

        var stocks = await _context.TukangSayurStocks
            .Include(s => s.TukangSayur).ThenInclude(v => v.User)
            .Include(s => s.Product)
            .Where(s => s.TukangSayur.IsOnline && s.StockQuantity > 0)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(category))
        {
            stocks = stocks.Where(s => s.Product.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            stocks = stocks.Where(s => s.Product.Name.ToLower().Contains(q) || s.Product.Description.ToLower().Contains(q) || s.TukangSayur.ShopName.ToLower().Contains(q)).ToList();
        }

        var result = new List<NearbyVendorProductDto>();

        foreach (var s in stocks)
        {
            var distance = CalculateDistanceKm(userLat, userLng, s.TukangSayur.Latitude, s.TukangSayur.Longitude);
            if (distance <= maxDistanceKm)
            {
                result.Add(new NearbyVendorProductDto(
                    s.TukangSayurId,
                    s.TukangSayur.ShopName,
                    s.TukangSayur.User.FullName,
                    s.TukangSayur.User.Phone,
                    s.TukangSayur.Latitude,
                    s.TukangSayur.Longitude,
                    Math.Round(distance, 2),
                    s.TukangSayur.IsOnline,
                    s.ProductId,
                    s.Product.Name,
                    s.Product.Category,
                    s.Product.Unit,
                    s.Product.ImageUrl,
                    s.StockQuantity,
                    s.PricePerUnit
                ));
            }
        }

        return result.OrderBy(r => r.DistanceKm).ThenBy(r => r.ProductName).ToList();
    }

    public async Task<OrderDto> CreateOrderAsync(int customerId, CreateOrderRequest request)
    {
        var customer = await _context.Users.FindAsync(customerId);
        if (customer == null) throw new InvalidOperationException("Pelanggan tidak ditemukan.");

        var vendor = await _context.TukangSayurProfiles.Include(v => v.User).FirstOrDefaultAsync(v => v.Id == request.TukangSayurId);
        if (vendor == null) throw new InvalidOperationException("Tukang sayur tidak ditemukan.");

        decimal totalAmount = 0m;
        var orderItems = new List<OrderItem>();

        foreach (var itemReq in request.Items)
        {
            var stock = await _context.TukangSayurStocks
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.TukangSayurId == request.TukangSayurId && s.ProductId == itemReq.ProductId);

            if (stock == null || stock.StockQuantity < itemReq.Quantity)
            {
                var prodName = stock?.Product.Name ?? $"ID {itemReq.ProductId}";
                throw new InvalidOperationException($"Stok '{prodName}' pada {vendor.ShopName} tidak mencukupi.");
            }

            // Deduct stock
            stock.StockQuantity -= itemReq.Quantity;
            stock.UpdatedAt = DateTime.UtcNow;

            var subtotal = (decimal)itemReq.Quantity * itemReq.UnitPrice;
            totalAmount += subtotal;

            orderItems.Add(new OrderItem
            {
                ProductId = itemReq.ProductId,
                Quantity = itemReq.Quantity,
                UnitPrice = itemReq.UnitPrice,
                SubTotal = subtotal
            });

            // Add SaleOut Transaction for Tukang Sayur
            _context.StockTransactions.Add(new StockTransaction
            {
                TukangSayurId = request.TukangSayurId,
                ProductId = itemReq.ProductId,
                Type = StockTransactionType.SaleOut,
                Quantity = itemReq.Quantity,
                UnitPrice = itemReq.UnitPrice,
                TotalAmount = subtotal,
                Notes = $"Penjualan kepada {customer.FullName} (Order #Direct)",
                TransactionDate = DateTime.UtcNow
            });
        }

        // Add income balance to vendor
        vendor.Balance += totalAmount;

        var order = new Order
        {
            CustomerId = customerId,
            TukangSayurId = request.TukangSayurId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Completed,
            MeetLatitude = request.MeetLatitude != 0 ? request.MeetLatitude : vendor.Latitude,
            MeetLongitude = request.MeetLongitude != 0 ? request.MeetLongitude : vendor.Longitude,
            MeetAddress = string.IsNullOrWhiteSpace(request.MeetAddress) ? vendor.CurrentLocationName : request.MeetAddress,
            OrderDate = DateTime.UtcNow,
            Items = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var loadedOrder = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.TukangSayur)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstAsync(o => o.Id == order.Id);

        return MapToOrderDto(loadedOrder);
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync(int customerId)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.TukangSayur)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(MapToOrderDto).ToList();
    }

    private static OrderDto MapToOrderDto(Order o)
    {
        return new OrderDto(
            o.Id,
            o.CustomerId,
            o.Customer?.FullName ?? "",
            o.TukangSayurId,
            o.TukangSayur?.ShopName ?? "",
            o.TotalAmount,
            o.Status.ToString(),
            o.MeetLatitude,
            o.MeetLongitude,
            o.MeetAddress,
            o.OrderDate,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.Product?.Name ?? "",
                i.Product?.Unit ?? "",
                i.Quantity,
                i.UnitPrice,
                i.SubTotal
            )).ToList()
        );
    }

    public async Task<bool> UpdateCustomerLocationAsync(int customerId, double lat, double lng)
    {
        var user = await _context.Users.FindAsync(customerId);
        if (user == null) return false;

        user.Latitude = lat;
        user.Longitude = lng;
        await _context.SaveChangesAsync();
        return true;
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double deg) => deg * (Math.PI / 180.0);
}
