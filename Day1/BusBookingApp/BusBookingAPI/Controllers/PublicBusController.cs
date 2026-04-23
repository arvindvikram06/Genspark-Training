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
    /// Searches for approved scheduled buses matching the route and date.
    /// </summary>
    /// <param name="source">Departure district (e.g. Chennai)</param>
    /// <param name="destination">Arrival district (e.g. Madurai)</param>
    /// <param name="date">Date of travel (YYYY-MM-DD)</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="departureAfter">Filter for buses departing after this time (e.g. 14:00:00)</param>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string source, 
        [FromQuery] string destination, 
        [FromQuery] DateTime date,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] TimeSpan? departureAfter)
    {
        var results = await _busService.SearchBusesAsync(source, destination, date, minPrice, maxPrice, departureAfter);
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
