using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

/// <summary>
/// Administrative endpoints for system management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// Lists all operators with their status, user info, bus count, and total revenue.
    /// </summary>
    [HttpGet("operators")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OperatorSummaryDto>>>> GetOperators()
    {
        var data = await _adminService.GetOperatorsAsync();
        return Ok(new ApiResponse<IEnumerable<OperatorSummaryDto>>(true, data, "Operators retrieved successfully"));
    }

    /// <summary>
    /// Approves a pending operator.
    /// </summary>
    [HttpPut("operators/{id}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveOperator(int id)
    {
        var success = await _adminService.ApproveOperatorAsync(id);
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Operator not found"));
        
        return Ok(new ApiResponse<object>(true, new { redirectToOfficeSetup = true }, "Operator approved successfully"));
    }

    /// <summary>
    /// Disables an operator.
    /// </summary>
    [HttpPut("operators/{id}/disable")]
    public async Task<ActionResult<ApiResponse<object>>> DisableOperator(int id)
    {
        var success = await _adminService.DisableOperatorAsync(id);
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Operator not found"));
        
        return Ok(new ApiResponse<object>(true, null, "Operator disabled successfully"));
    }

    /// <summary>
    /// Lists all buses for a specific operator.
    /// </summary>
    [HttpGet("operators/{id}/buses")]
    public async Task<ActionResult<ApiResponse<IEnumerable<BusSummaryDto>>>> GetOperatorBuses(int id)
    {
        var data = await _adminService.GetOperatorBusesAsync(id);
        return Ok(new ApiResponse<IEnumerable<BusSummaryDto>>(true, data, "Buses retrieved successfully"));
    }

    /// <summary>
    /// Lists all buses from all operators.
    /// </summary>
    [HttpGet("buses")]
    public async Task<ActionResult<ApiResponse<IEnumerable<BusSummaryDto>>>> GetAllBuses()
    {
        var data = await _adminService.GetAllBusesAsync();
        return Ok(new ApiResponse<IEnumerable<BusSummaryDto>>(true, data, "All buses retrieved successfully"));
    }

    /// <summary>
    /// Approves a pending bus.
    /// </summary>
    [HttpPut("buses/{id}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveBus(int id)
    {
        var success = await _adminService.ApproveBusAsync(id);
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Bus not found"));
        return Ok(new ApiResponse<object>(true, null, "Bus approved successfully"));
    }

    /// <summary>
    /// Disables an active bus.
    /// </summary>
    [HttpPut("buses/{id}/disable")]
    public async Task<ActionResult<ApiResponse<object>>> DisableBus(int id)
    {
        var success = await _adminService.DisableBusAsync(id);
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Bus not found"));
        return Ok(new ApiResponse<object>(true, null, "Bus disabled successfully"));
    }

    /// <summary>
    /// Gets total revenue for a specific operator.
    /// </summary>
    [HttpGet("operators/{id}/revenue")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetOperatorRevenue(int id)
    {
        var revenue = await _adminService.GetOperatorRevenueAsync(id);
        return Ok(new ApiResponse<decimal>(true, revenue, "Revenue calculated successfully"));
    }

    /// <summary>
    /// Lists all active routes.
    /// </summary>
    [HttpGet("routes")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RouteDto>>>> GetRoutes()
    {
        var data = await _adminService.GetRoutesAsync();
        return Ok(new ApiResponse<IEnumerable<RouteDto>>(true, data, "Routes retrieved successfully"));
    }

    /// <summary>
    /// Creates a new route.
    /// </summary>
    [HttpPost("routes")]
    public async Task<ActionResult<ApiResponse<RouteDto>>> CreateRoute(CreateRouteRequest request)
    {
        var data = await _adminService.CreateRouteAsync(request);
        return Ok(new ApiResponse<RouteDto>(true, data, "Route created successfully"));
    }

    /// <summary>
    /// Updates an existing route.
    /// </summary>
    [HttpPut("routes/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateRoute(int id, CreateRouteRequest request)
    {
        var success = await _adminService.UpdateRouteAsync(id, request);
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Route not found"));
        
        return Ok(new ApiResponse<object>(true, null, "Route updated successfully"));
    }

    /// <summary>
    /// Deletes a route (soft delete if schedules exist).
    /// </summary>
    [HttpDelete("routes/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteRoute(int id)
    {
        var success = await _adminService.DeleteRouteAsync(id);
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Route not found"));
        
        return Ok(new ApiResponse<object>(true, null, "Route deleted successfully"));
    }

    /// <summary>
    /// Lists all pending schedules across the platform.
    /// </summary>
    [HttpGet("schedules/pending")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ScheduleSummaryDto>>>> GetPendingSchedules()
    {
        var data = await _adminService.GetPendingSchedulesAsync();
        return Ok(new ApiResponse<IEnumerable<ScheduleSummaryDto>>(true, data, "Pending schedules retrieved successfully"));
    }

    /// <summary>
    /// Approves a pending schedule.
    /// </summary>
    [HttpPut("schedules/{id}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> ApproveSchedule(int id)
    {
        var success = await _adminService.ApproveScheduleAsync(id);
        if (!success) return NotFound(new ApiResponse<object>(false, null, "Schedule not found"));
        
        return Ok(new ApiResponse<object>(true, null, "Schedule approved successfully"));
    }

    /// <summary>
    /// Gets current convenience fee from configuration.
    /// </summary>
    [HttpGet("config")]
    public async Task<ActionResult<ApiResponse<string>>> GetConfig()
    {
        var fee = await _adminService.GetConvenienceFeeAsync();
        return Ok(new ApiResponse<string>(true, fee, "Config retrieved successfully"));
    }

    /// <summary>
    /// Updates the convenience fee per seat.
    /// </summary>
    [HttpPut("config/convenience-fee")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateConvenienceFee(UpdateConvenienceFeeRequest request)
    {
        var success = await _adminService.UpdateConvenienceFeeAsync(request.Fee);
        return Ok(new ApiResponse<object>(true, null, "Convenience fee updated successfully"));
    }
}
