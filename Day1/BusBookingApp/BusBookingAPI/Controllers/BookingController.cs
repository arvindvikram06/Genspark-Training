using System.Security.Claims;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost("hold")]
    public async Task<IActionResult> HoldSeats(SeatHoldRequest request)
    {
        var userId = GetUserId();
        var result = await _bookingService.HoldSeatsAsync(userId, request);
        
        if (!result.Success) return Conflict(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<SeatHoldResponse>(true, result.Data, result.Message));
    }

    [HttpDelete("hold/{holdId}")]
    public async Task<IActionResult> ReleaseHold(int holdId)
    {
        var userId = GetUserId();
        var success = await _bookingService.ReleaseHoldAsync(userId, holdId);
        
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Hold not found"));
        
        return Ok(new ApiResponse<object>(true, null, "Hold released successfully"));
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmBooking(BookingConfirmRequest request)
    {
        var userId = GetUserId();
        var result = await _bookingService.ConfirmBookingAsync(userId, request);
        
        if (!result.Success)
        {
            if (result.Message == "Hold expired")
                return StatusCode(409, new ApiResponse<object>(false, null, result.Message));
            
            return BadRequest(new ApiResponse<object>(false, null, result.Message));
        }
        
        return Ok(new ApiResponse<BookingResponse>(true, result.Data, result.Message));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = GetUserId();
        var bookings = await _bookingService.GetMyBookingsAsync(userId);
        return Ok(new ApiResponse<IEnumerable<BookingSummaryResponse>>(true, bookings, "Bookings retrieved successfully"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingDetail(int id)
    {
        var userId = GetUserId();
        var booking = await _bookingService.GetBookingDetailAsync(userId, id);
        
        if (booking == null) return NotFound(new ApiResponse<object>(false, null, "Booking not found"));
        
        return Ok(new ApiResponse<BookingResponse>(true, booking, "Booking detail retrieved successfully"));
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = GetUserId();
        var result = await _bookingService.CancelBookingAsync(userId, id);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<object>(true, null, result.Message));
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}
