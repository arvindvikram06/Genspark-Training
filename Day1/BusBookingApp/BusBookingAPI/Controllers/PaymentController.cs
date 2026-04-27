using System.Security.Claims;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("pay/{paymentId}")]
    public async Task<IActionResult> Pay(int paymentId, [FromQuery] string transactionRef = "SIMULATED_REF", [FromQuery] string? idempotencyKey = null)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _paymentService.ProcessPaymentAsync(userId, paymentId, transactionRef, idempotencyKey);
        
        if (!result.Success)
        {
            if (result.Message.Contains("expired", StringComparison.OrdinalIgnoreCase))
                return StatusCode(409, new ApiResponse<object>(false, null, result.Message));
            
            return BadRequest(new ApiResponse<object>(false, null, result.Message));
        }
        
        return Ok(new ApiResponse<BookingResponse>(true, result.Booking, result.Message));
    }

    [HttpPost("abort/{paymentId}")]
    public async Task<IActionResult> Abort(int paymentId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _paymentService.AbortPendingPaymentAsync(userId, paymentId);

        if (!result.Success)
        {
            if (result.Message.Contains("Cannot cancel", StringComparison.OrdinalIgnoreCase))
                return StatusCode(409, new ApiResponse<object>(false, null, result.Message));
            
            return BadRequest(new ApiResponse<object>(false, null, result.Message));
        }

        return Ok(new ApiResponse<object>(true, null, result.Message));
    }

    [HttpGet("status/{paymentId}")]
    public async Task<IActionResult> GetStatus(int paymentId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _paymentService.GetPaymentStatusAsync(userId, paymentId);

        if (!result.Success) return NotFound(new ApiResponse<object>(false, null, result.Message));

        return Ok(new ApiResponse<PaymentStatusResponse>(true, result.Data, result.Message));
    }

    [HttpPost("retry/{paymentId}")]
    public async Task<IActionResult> Retry(int paymentId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _paymentService.RetryPaymentAsync(userId, paymentId);

        if (!result.Success)
        {
            if (result.Message.Contains("Maximum retry", StringComparison.OrdinalIgnoreCase))
                return StatusCode(429, new ApiResponse<object>(false, null, result.Message));
            
            if (result.Message.Contains("already successful", StringComparison.OrdinalIgnoreCase))
                return StatusCode(409, new ApiResponse<object>(false, null, result.Message));
            
            return BadRequest(new ApiResponse<object>(false, null, result.Message));
        }

        return Ok(new ApiResponse<object>(true, null, result.Message));
    }
}
