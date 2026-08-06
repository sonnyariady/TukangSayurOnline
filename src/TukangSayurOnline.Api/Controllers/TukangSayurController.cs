using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TukangSayurOnline.Api.Models;
using TukangSayurOnline.Api.Services;

namespace TukangSayurOnline.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TukangSayurController : ControllerBase
{
    private readonly ITukangSayurService _tukangSayurService;

    public TukangSayurController(ITukangSayurService tukangSayurService)
    {
        _tukangSayurService = tukangSayurService;
    }

    [HttpGet("{tukangSayurId}/stocks")]
    public async Task<IActionResult> GetMyStocks(int tukangSayurId)
    {
        var stocks = await _tukangSayurService.GetMyStocksAsync(tukangSayurId);
        return Ok(stocks);
    }

    [HttpPost("{tukangSayurId}/stocks/update")]
    public async Task<IActionResult> UpdateStock(int tukangSayurId, [FromBody] UpdateStockRequest request)
    {
        var result = await _tukangSayurService.UpdateStockAsync(tukangSayurId, request);
        return Ok(result);
    }

    [HttpPost("{tukangSayurId}/restock-in")]
    public async Task<IActionResult> RestockIn(int tukangSayurId, [FromBody] RestockInRequest request)
    {
        try
        {
            var result = await _tukangSayurService.RestockInAsync(tukangSayurId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("{tukangSayurId}/sale-out")]
    public async Task<IActionResult> RecordSaleOut(int tukangSayurId, [FromBody] RecordSaleRequest request)
    {
        try
        {
            var result = await _tukangSayurService.RecordSaleOutAsync(tukangSayurId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("{tukangSayurId}/income-summary")]
    public async Task<IActionResult> GetIncomeSummary(int tukangSayurId)
    {
        try
        {
            var summary = await _tukangSayurService.GetIncomeSummaryAsync(tukangSayurId);
            return Ok(summary);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    public record ToggleOnlineRequest(bool IsOnline, double Latitude, double Longitude, string LocationName);

    [HttpPost("{tukangSayurId}/toggle-online")]
    public async Task<IActionResult> ToggleOnline(int tukangSayurId, [FromBody] ToggleOnlineRequest request)
    {
        var success = await _tukangSayurService.ToggleOnlineAsync(tukangSayurId, request.IsOnline, request.Latitude, request.Longitude, request.LocationName);
        if (!success) return NotFound("Tukang sayur tidak ditemukan.");
        return Ok(new { Message = "Status online berhasil diperbarui.", IsOnline = request.IsOnline });
    }
}
