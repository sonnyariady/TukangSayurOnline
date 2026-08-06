using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TukangSayurOnline.Api.Data;
using TukangSayurOnline.Api.Data.Entities;
using TukangSayurOnline.Api.Models;

namespace TukangSayurOnline.Api.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync(string? category, string? search);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto?> UpdateProductAsync(UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);
    Task<List<PopularProductReportDto>> GetPopularProductsAsync();
    Task<List<EmptyStockReportDto>> GetEmptyStockReportsAsync();
}

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync(string? category, string? search)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category.ToLower() == category.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()) || p.Description.ToLower().Contains(search.ToLower()));
        }

        var products = await query.OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync();
        return products.Select(p => new ProductDto(p.Id, p.Name, p.Category, p.Unit, p.Description, p.ImageUrl, p.DefaultPrice)).ToList();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var p = await _context.Products.FindAsync(id);
        if (p == null) return null;
        return new ProductDto(p.Id, p.Name, p.Category, p.Unit, p.Description, p.ImageUrl, p.DefaultPrice);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Category = request.Category,
            Unit = request.Unit,
            Description = request.Description,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400" : request.ImageUrl,
            DefaultPrice = request.DefaultPrice
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto(product.Id, product.Name, product.Category, product.Unit, product.Description, product.ImageUrl, product.DefaultPrice);
    }

    public async Task<ProductDto?> UpdateProductAsync(UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null) return null;

        product.Name = request.Name;
        product.Category = request.Category;
        product.Unit = request.Unit;
        product.Description = request.Description;
        product.ImageUrl = request.ImageUrl;
        product.DefaultPrice = request.DefaultPrice;

        await _context.SaveChangesAsync();
        return new ProductDto(product.Id, product.Name, product.Category, product.Unit, product.Description, product.ImageUrl, product.DefaultPrice);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<PopularProductReportDto>> GetPopularProductsAsync()
    {
        var sales = await _context.StockTransactions
            .Where(t => t.Type == StockTransactionType.SaleOut)
            .GroupBy(t => new { t.ProductId, t.Product.Name, t.Product.Category })
            .Select(g => new PopularProductReportDto(
                g.Key.ProductId,
                g.Key.Name,
                g.Key.Category,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.TotalAmount)
            ))
            .OrderByDescending(r => r.TotalQuantitySold)
            .ToListAsync();

        return sales;
    }

    public async Task<List<EmptyStockReportDto>> GetEmptyStockReportsAsync()
    {
        var emptyStocks = await _context.TukangSayurStocks
            .Include(s => s.TukangSayur).ThenInclude(v => v.User)
            .Include(s => s.Product)
            .Where(s => s.StockQuantity <= 0)
            .Select(s => new EmptyStockReportDto(
                s.TukangSayurId,
                s.TukangSayur.ShopName,
                s.TukangSayur.User.FullName,
                s.ProductId,
                s.Product.Name,
                s.Product.Category
            ))
            .ToListAsync();

        return emptyStocks;
    }
}
