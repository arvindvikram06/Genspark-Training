using System.Security.Claims;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

/// <summary>
/// Schedule management endpoints for Operators.
/// </summary>
[ApiController]
[Route("api/operator/schedules")]
public class OperatorScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IOperatorService _operatorService;

    public OperatorScheduleController(IScheduleService scheduleService, IOperatorService operatorService)
    {
        _scheduleService = scheduleService;
        _operatorService = operatorService;
    }

    /// <summary>
    /// Lists all admin-approved routes. Publicly accessible.
    /// </summary>
    [HttpGet("routes")]
    public async Task<IActionResult> GetRoutes()
    {
        var routes = await _scheduleService.GetApprovedRoutesAsync();
        return Ok(new ApiResponse<IEnumerable<RouteDto>>(true, routes, "Routes retrieved successfully"));
    }

    /// <summary>
    /// Creates a new bus schedule. 
    /// Note: BoardingPoint and DropPoint are auto-resolved from your registered office addresses based on the Route's Source and Destination.
    /// Requires an office in both locations.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> CreateSchedule(CreateScheduleRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _scheduleService.CreateScheduleAsync(userId, request);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<ScheduleResponse>(true, result.Data, result.Message));
    }

    /// <summary>
    /// Lists all schedules created by the operator.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> GetSchedules()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var schedules = await _scheduleService.GetOperatorSchedulesAsync(userId);
        return Ok(new ApiResponse<IEnumerable<ScheduleResponse>>(true, schedules, "Schedules retrieved successfully"));
    }

    /// <summary>
    /// Updates a pending schedule.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> UpdateSchedule(int id, CreateScheduleRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _scheduleService.UpdateScheduleAsync(userId, id, request);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<object>(true, null, result.Message));
    }

    /// <summary>
    /// Cancels a schedule. Only possible if there are no confirmed bookings.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> CancelSchedule(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _scheduleService.CancelScheduleAsync(userId, id);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<object>(true, null, result.Message));
    }

    /// <summary>
    /// Dashboard query: Lists all bookings for a specific schedule with revenue and passenger summaries.
    /// </summary>
    [HttpGet("{scheduleId}/bookings")]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> GetScheduleBookings(int scheduleId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _operatorService.GetScheduleBookingsAsync(userId, scheduleId);
        
        if (result == null) return NotFound(new ApiResponse<object>(false, null, "Schedule not found or unauthorized"));
        
        return Ok(new ApiResponse<ScheduleBookingSummary>(true, result, "Schedule bookings retrieved successfully"));
    }
}
