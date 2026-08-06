using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TukangSayurOnline.Api.Models;
using TukangSayurOnline.Api.Services;

namespace TukangSayurOnline.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IProductService _productService;

    public AdminController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] string? category, [FromQuery] string? search)
    {
        var products = await _productService.GetAllProductsAsync(category, search);
        return Ok(products);
    }

    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = await _productService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        if (id != request.Id) return BadRequest("ID Mismatch");
        var updated = await _productService.UpdateProductAsync(request);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await _productService.DeleteProductAsync(id);
        if (!success) return NotFound();
        return Ok(new { Message = "Produk berhasil dihapus." });
    }

    [HttpGet("reports/popular-products")]
    public async Task<IActionResult> GetPopularProducts()
    {
        var report = await _productService.GetPopularProductsAsync();
        return Ok(report);
    }

    [HttpGet("reports/empty-stocks")]
    public async Task<IActionResult> GetEmptyStockReports()
    {
        var report = await _productService.GetEmptyStockReportsAsync();
        return Ok(report);
    }
}
