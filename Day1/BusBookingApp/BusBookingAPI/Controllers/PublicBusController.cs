using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingAPI.Controllers;

/// <summary>
/// Public endpoints for browsing buses and seat availability.
/// </summary>
[ApiController]
[Route("api/buses")]
public class PublicBusController : ControllerBase
{
    private readonly IBusService _busService;

    public PublicBusController(IBusService busService)
    {
        _busService = busService;
    }

    /// <summary>
    /// Lists all upcoming approved scheduled buses across all routes.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllBuses()
    {
        var results = await _busService.GetAllUpcomingBusesAsync();
        return Ok(new ApiResponse<IEnumerable<BusSearchResponse>>(true, results, "Upcoming buses retrieved successfully"));
    }

    /// <summary>
    /// Lists all approved schedules without date filtering.
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllApprovedBuses()
    {
        var results = await _busService.GetAllApprovedBusesAsync();
        return Ok(new ApiResponse<IEnumerable<BusSearchResponse>>(true, results, "All approved buses retrieved successfully"));
    }

    /// <summary>
    /// Searches for approved scheduled buses with flexible filters (fuzzy search).
    /// All parameters are optional - provide any combination to filter results.
    /// </summary>
    /// <param name="query">Fuzzy search across destination and operator name</param>
    /// <param name="source">Departure district (fuzzy match, e.g. "Chennai")</param>
    /// <param name="destination">Arrival district (fuzzy match, e.g. "Madurai")</param>
    /// <param name="date">Date of travel (YYYY-MM-DD, optional - shows upcoming if not provided)</param>
    /// <param name="operatorName">Operator/travels name (fuzzy match)</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="departureAfter">Filter for buses departing after this time (e.g. 14:00:00)</param>
    /// <param name="departureBefore">Filter for buses departing before this time (e.g. 18:00:00)</param>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? query = null,
        [FromQuery] string? source = null,
        [FromQuery] string? destination = null,
        [FromQuery] DateTime? date = null,
        [FromQuery] string? operatorName = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] TimeSpan? departureAfter = null,
        [FromQuery] TimeSpan? departureBefore = null)
    {
        var results = await _busService.SearchBusesAsync(query, source, destination, date, operatorName, minPrice, maxPrice, departureAfter, departureBefore);
        return Ok(new ApiResponse<IEnumerable<BusSearchResponse>>(true, results, "Buses retrieved successfully"));
    }

    /// <summary>
    /// Returns the seat map and availability for a specific schedule.
    /// </summary>
    [HttpGet("{scheduleId}/seats")]
    public async Task<IActionResult> GetSeats(int scheduleId)
    {
        var result = await _busService.GetSeatMapAsync(scheduleId);
        if (result == null) return NotFound(new ApiResponse<object>(false, null, "Schedule not found"));
        return Ok(new ApiResponse<ScheduleSeatMapResponse>(true, result, "Seats retrieved successfully"));
    }
}
