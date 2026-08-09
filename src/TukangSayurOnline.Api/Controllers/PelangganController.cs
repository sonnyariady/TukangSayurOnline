using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TukangSayurOnline.Api.Models;
using TukangSayurOnline.Api.Services;

namespace TukangSayurOnline.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PelangganController : ControllerBase
{
    private readonly IPelangganService _pelangganService;

    public PelangganController(IPelangganService pelangganService)
    {
        _pelangganService = pelangganService;
    }

    [HttpGet("nearby-vendors")]
    public async Task<IActionResult> GetNearbyVendors(
        [FromQuery] double userLat = 0,
        [FromQuery] double userLng = 0,
        [FromQuery] double maxDistanceKm = 25)
    {
        var result = await _pelangganService.GetNearbyVendorsAsync(userLat, userLng, maxDistanceKm);
        return Ok(result);
    }

    [HttpGet("vendors/{vendorId}/catalog")]
    public async Task<IActionResult> GetVendorCatalog(
        int vendorId,
        [FromQuery] double userLat = 0,
        [FromQuery] double userLng = 0)
    {
        var result = await _pelangganService.GetVendorCatalogAsync(vendorId, userLat, userLng);
        if (result == null) return NotFound("Lapak Tukang Sayur tidak ditemukan.");
        return Ok(result);
    }

    [HttpGet("search-nearby")]
    public async Task<IActionResult> SearchNearby(
        [FromQuery] double userLat = 0,
        [FromQuery] double userLng = 0,
        [FromQuery] string? query = null,
        [FromQuery] string? category = null,
        [FromQuery] double maxDistanceKm = 25)
    {
        var result = await _pelangganService.SearchNearbyProductsAsync(userLat, userLng, query, category, maxDistanceKm);
        return Ok(result);
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromQuery] int customerId, [FromBody] CreateOrderRequest request)
    {
        try
        {
            var result = await _pelangganService.CreateOrderAsync(customerId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("orders/my-orders")]
    public async Task<IActionResult> GetMyOrders([FromQuery] int customerId)
    {
        var result = await _pelangganService.GetMyOrdersAsync(customerId);
        return Ok(result);
    }

    public record UpdateLocationRequest(double Latitude, double Longitude);

    [HttpPost("{customerId}/update-location")]
    public async Task<IActionResult> UpdateLocation(int customerId, [FromBody] UpdateLocationRequest request)
    {
        var success = await _pelangganService.UpdateCustomerLocationAsync(customerId, request.Latitude, request.Longitude);
        return Ok(new { Success = success });
    }
}
