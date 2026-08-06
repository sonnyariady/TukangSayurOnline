using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TukangSayurOnline.Api.Data;
using TukangSayurOnline.Api.Data.Entities;
using TukangSayurOnline.Api.Models;

namespace TukangSayurOnline.Api.Services;

public interface ITukangSayurService
{
    Task<TukangSayurProfile?> GetProfileByUserIdAsync(int userId);
    Task<List<StockItemDto>> GetMyStocksAsync(int tukangSayurId);
    Task<StockItemDto> UpdateStockAsync(int tukangSayurId, UpdateStockRequest request);
    Task<StockTransactionDto> RestockInAsync(int tukangSayurId, RestockInRequest request);
    Task<StockTransactionDto> RecordSaleOutAsync(int tukangSayurId, RecordSaleRequest request);
    Task<IncomeSummaryDto> GetIncomeSummaryAsync(int tukangSayurId);
    Task<bool> ToggleOnlineAsync(int tukangSayurId, bool isOnline, double latitude, double longitude, string locationName);
}

public class TukangSayurService : ITukangSayurService
{
    private readonly AppDbContext _context;

    public TukangSayurService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TukangSayurProfile?> GetProfileByUserIdAsync(int userId)
    {
        return await _context.TukangSayurProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<List<StockItemDto>> GetMyStocksAsync(int tukangSayurId)
    {
        var stocks = await _context.TukangSayurStocks
            .Include(s => s.Product)
            .Where(s => s.TukangSayurId == tukangSayurId)
            .ToListAsync();

        return stocks.Select(s => new StockItemDto(
            s.Id,
            s.ProductId,
            s.Product.Name,
            s.Product.Category,
            s.Product.Unit,
            s.Product.ImageUrl,
            s.StockQuantity,
            s.PricePerUnit,
            s.UpdatedAt
        )).ToList();
    }

    public async Task<StockItemDto> UpdateStockAsync(int tukangSayurId, UpdateStockRequest request)
    {
        var stock = await _context.TukangSayurStocks
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.TukangSayurId == tukangSayurId && s.ProductId == request.ProductId);

        if (stock == null)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null) throw new InvalidOperationException("Produk tidak ditemukan.");

            stock = new TukangSayurStock
            {
                TukangSayurId = tukangSayurId,
                ProductId = request.ProductId,
                StockQuantity = request.StockQuantity,
                PricePerUnit = request.PricePerUnit,
                UpdatedAt = DateTime.UtcNow
            };
            _context.TukangSayurStocks.Add(stock);
        }
        else
        {
            stock.StockQuantity = request.StockQuantity;
            stock.PricePerUnit = request.PricePerUnit;
            stock.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var reloadedProduct = await _context.Products.FindAsync(request.ProductId);
        return new StockItemDto(
            stock.Id,
            stock.ProductId,
            reloadedProduct?.Name ?? "",
            reloadedProduct?.Category ?? "",
            reloadedProduct?.Unit ?? "",
            reloadedProduct?.ImageUrl ?? "",
            stock.StockQuantity,
            stock.PricePerUnit,
            stock.UpdatedAt
        );
    }

    public async Task<StockTransactionDto> RestockInAsync(int tukangSayurId, RestockInRequest request)
    {
        var vendor = await _context.TukangSayurProfiles.FindAsync(tukangSayurId);
        if (vendor == null) throw new InvalidOperationException("Tukang sayur tidak ditemukan.");

        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null) throw new InvalidOperationException("Produk tidak ditemukan.");

        // Update or create stock entry
        var stock = await _context.TukangSayurStocks
            .FirstOrDefaultAsync(s => s.TukangSayurId == tukangSayurId && s.ProductId == request.ProductId);

        if (stock == null)
        {
            stock = new TukangSayurStock
            {
                TukangSayurId = tukangSayurId,
                ProductId = request.ProductId,
                StockQuantity = request.Quantity,
                PricePerUnit = product.DefaultPrice > 0 ? product.DefaultPrice : request.UnitPrice * 1.2m,
                UpdatedAt = DateTime.UtcNow
            };
            _context.TukangSayurStocks.Add(stock);
        }
        else
        {
            stock.StockQuantity += request.Quantity;
            stock.UpdatedAt = DateTime.UtcNow;
        }

        var totalCost = (decimal)request.Quantity * request.UnitPrice;

        var transaction = new StockTransaction
        {
            TukangSayurId = tukangSayurId,
            ProductId = request.ProductId,
            Type = StockTransactionType.RestockIn,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = totalCost,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? "Restock Pembelian Barang" : request.Notes,
            TransactionDate = DateTime.UtcNow
        };

        _context.StockTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new StockTransactionDto(
            transaction.Id,
            product.Id,
            product.Name,
            transaction.Type.ToString(),
            transaction.Quantity,
            transaction.UnitPrice,
            transaction.TotalAmount,
            transaction.Notes,
            transaction.TransactionDate
        );
    }

    public async Task<StockTransactionDto> RecordSaleOutAsync(int tukangSayurId, RecordSaleRequest request)
    {
        var vendor = await _context.TukangSayurProfiles.FindAsync(tukangSayurId);
        if (vendor == null) throw new InvalidOperationException("Tukang sayur tidak ditemukan.");

        var stock = await _context.TukangSayurStocks
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.TukangSayurId == tukangSayurId && s.ProductId == request.ProductId);

        if (stock == null || stock.StockQuantity < request.Quantity)
        {
            throw new InvalidOperationException("Stok tidak mencukupi untuk transaksi ini.");
        }

        // Reduce stock & Increase balance (income)
        stock.StockQuantity -= request.Quantity;
        stock.UpdatedAt = DateTime.UtcNow;

        var totalIncome = (decimal)request.Quantity * request.UnitPrice;
        vendor.Balance += totalIncome;

        var transaction = new StockTransaction
        {
            TukangSayurId = tukangSayurId,
            ProductId = request.ProductId,
            Type = StockTransactionType.SaleOut,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = totalIncome,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? "Penjualan Langsung Ke Pelanggan" : request.Notes,
            TransactionDate = DateTime.UtcNow
        };

        _context.StockTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return new StockTransactionDto(
            transaction.Id,
            stock.Product.Id,
            stock.Product.Name,
            transaction.Type.ToString(),
            transaction.Quantity,
            transaction.UnitPrice,
            transaction.TotalAmount,
            transaction.Notes,
            transaction.TransactionDate
        );
    }

    public async Task<IncomeSummaryDto> GetIncomeSummaryAsync(int tukangSayurId)
    {
        var vendor = await _context.TukangSayurProfiles.FindAsync(tukangSayurId);
        if (vendor == null) throw new InvalidOperationException("Tukang sayur tidak ditemukan.");

        var transactions = await _context.StockTransactions
            .Include(t => t.Product)
            .Where(t => t.TukangSayurId == tukangSayurId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        var totalIncome = transactions
            .Where(t => t.Type == StockTransactionType.SaleOut)
            .Sum(t => t.TotalAmount);

        var totalRestockExpense = transactions
            .Where(t => t.Type == StockTransactionType.RestockIn)
            .Sum(t => t.TotalAmount);

        var salesCount = transactions.Count(t => t.Type == StockTransactionType.SaleOut);

        var recentList = transactions.Take(20).Select(t => new StockTransactionDto(
            t.Id,
            t.ProductId,
            t.Product.Name,
            t.Type.ToString(),
            t.Quantity,
            t.UnitPrice,
            t.TotalAmount,
            t.Notes,
            t.TransactionDate
        )).ToList();

        return new IncomeSummaryDto(
            vendor.Balance,
            totalIncome,
            totalRestockExpense,
            salesCount,
            recentList
        );
    }

    public async Task<bool> ToggleOnlineAsync(int tukangSayurId, bool isOnline, double latitude, double longitude, string locationName)
    {
        var vendor = await _context.TukangSayurProfiles.FindAsync(tukangSayurId);
        if (vendor == null) return false;

        vendor.IsOnline = isOnline;
        if (latitude != 0 && longitude != 0)
        {
            vendor.Latitude = latitude;
            vendor.Longitude = longitude;
        }
        if (!string.IsNullOrWhiteSpace(locationName))
        {
            vendor.CurrentLocationName = locationName;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
