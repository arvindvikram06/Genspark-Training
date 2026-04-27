using System.Security.Claims;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

/// <summary>
/// Onboarding endpoints for Operators.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OperatorController : ControllerBase
{
    private readonly IOperatorService _operatorService;

    public OperatorController(IOperatorService operatorService)
    {
        _operatorService = operatorService;
    }

    /// <summary>
    /// Lists all 38 districts of Tamil Nadu.
    /// </summary>
    [HttpGet("districts")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDistricts()
    {
        var districts = await _operatorService.GetDistrictsAsync();
        return Ok(new ApiResponse<IEnumerable<object>>(true, districts, "Districts retrieved successfully"));
    }

    /// <summary>
    /// Submits selected districts and addresses for operator offices. 
    /// If an office already exists in a district, its address will be updated.
    /// Otherwise, a new office will be added. Use this to expand into new districts.
    /// </summary>
    [HttpPost("offices")]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> SubmitOffices(IEnumerable<OfficeSubmissionRequest> request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _operatorService.SubmitOfficesAsync(userId, request);
        
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        
        return Ok(new ApiResponse<object>(true, null, result.Message));
    }

    /// <summary>
    /// Gets the list of offices for the current operator.
    /// </summary>
    [HttpGet("offices")]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> GetOffices()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var offices = await _operatorService.GetOfficesAsync(userId);
        return Ok(new ApiResponse<IEnumerable<OfficeResponse>>(true, offices, "Offices retrieved successfully"));
    }

    /// <summary>
    /// Gets the profile and status of the current operator.
    /// </summary>
    [HttpGet("profile")]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _operatorService.GetProfileAsync(userId);
        if (!result.Success) return NotFound(new ApiResponse<object>(false, null, result.Message));
        return Ok(new ApiResponse<OperatorSummaryDto>(true, result.Operator, result.Message));
    }

    /// <summary>
    /// Disables a bus for a specific time range.
    /// </summary>
    [HttpPost("buses/disable")]
    [Authorize(Roles = "Operator")]
    public async Task<IActionResult> DisableBus([FromBody] DisableBusRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _operatorService.DisableBusAsync(userId, request);
        if (!result.Success) return BadRequest(new ApiResponse<object>(false, null, result.Message));
        return Ok(new ApiResponse<object>(true, null, result.Message));
    }
}
