using System.Security.Claims;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

/// <summary>
/// Bus management endpoints for Operators.
/// </summary>
[ApiController]
[Route("api/operator/buses")]
[Authorize(Roles = "Operator")]
public class OperatorBusController : ControllerBase
{
    private readonly IOperatorService _operatorService;

    public OperatorBusController(IOperatorService operatorService)
    {
        _operatorService = operatorService;
    }

    /// <summary>
    /// Creates a new bus. Requires offices to be submitted first.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBus(CreateBusRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _operatorService.CreateBusAsync(userId, request);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return CreatedAtAction(nameof(GetBuses), new { }, new ApiResponse<BusResponse>(true, result.Bus, result.Message));
    }

    /// <summary>
    /// Lists all buses belonging to the operator.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBuses()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var buses = await _operatorService.GetBusesAsync(userId);
        return Ok(new ApiResponse<IEnumerable<BusResponse>>(true, buses, "Buses retrieved successfully"));
    }

    /// <summary>
    /// Updates a bus configuration. Restricted if there are active bookings.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBus(int id, CreateBusRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _operatorService.UpdateBusAsync(userId, id, request);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<object>(true, null, result.Message));
    }

    /// <summary>
    /// Soft deletes a bus.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBus(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var success = await _operatorService.DeleteBusAsync(userId, id);
        
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Bus not found"));
        
        return Ok(new ApiResponse<object>(true, null, "Bus deleted successfully"));
    }

    /// <summary>
    /// Disables a bus for a specific time range (e.g., for maintenance).
    /// </summary>
    [HttpPut("{id}/disable")]
    public async Task<IActionResult> DisableBus(int id, DisableBusRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var success = await _operatorService.DisableBusAsync(userId, id, request);
        
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Bus not found"));
        
        return Ok(new ApiResponse<object>(true, null, "Bus disabled successfully"));
    }

    /// <summary>
    /// Lists all bookings for a specific bus.
    /// </summary>
    [HttpGet("{id}/bookings")]
    public async Task<IActionResult> GetBusBookings(int id, [FromQuery] int? scheduleId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var bookings = await _operatorService.GetBusBookingsAsync(userId, id, scheduleId);
        return Ok(new ApiResponse<IEnumerable<BookingSummaryDto>>(true, bookings, "Bookings retrieved successfully"));
    }
}
