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
    public async Task<IActionResult> Pay(int paymentId, [FromQuery] string transactionRef = "SIMULATED_REF")
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _paymentService.ProcessPaymentAsync(userId, paymentId, transactionRef);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<BookingResponse>(true, result.Booking, result.Message));
    }

    [HttpPost("abort/{paymentId}")]
    public async Task<IActionResult> Abort(int paymentId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _paymentService.AbortPendingPaymentAsync(userId, paymentId);

        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));

        return Ok(new ApiResponse<object>(true, null, result.Message));
    }
}
