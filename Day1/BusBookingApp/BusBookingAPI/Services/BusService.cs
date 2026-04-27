using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IBusService
{
    Task<IEnumerable<BusSearchResponse>> SearchBusesAsync(
        string? query = null,
        string? source = null,
        string? destination = null,
        DateTime? date = null,
        string? operatorName = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        TimeSpan? departureAfter = null,
        TimeSpan? departureBefore = null);

    Task<IEnumerable<BusSearchResponse>> GetAllUpcomingBusesAsync();
    Task<IEnumerable<BusSearchResponse>> GetAllApprovedBusesAsync();

    Task<ScheduleSeatMapResponse?> GetSeatMapAsync(int scheduleId);
}

public class BusService : IBusService
{
    private readonly BusBookingDbContext _context;

    public BusService(BusBookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BusSearchResponse>> SearchBusesAsync(
        string? query = null,
        string? source = null,
        string? destination = null,
        DateTime? date = null,
        string? operatorName = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        TimeSpan? departureAfter = null,
        TimeSpan? departureBefore = null)
    {
        var queryBuilder = _context.Schedules
            .Include(s => s.Bus)
                .ThenInclude(b => b.Operator)
                    .ThenInclude(o => o.User)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .Where(s => s.Status == ScheduleStatus.Approved && !s.Bus.IsDeleted);

        // Fuzzy search query - searches across source, destination and operator name
        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            queryBuilder = queryBuilder.Where(s =>
                s.Route.Source.ToLower().Contains(lowerQuery) ||
                s.Route.Destination.ToLower().Contains(lowerQuery) ||
                s.Bus.Operator.User.Name.ToLower().Contains(lowerQuery));
        }

        // Specific source filter (if provided separately)
        if (!string.IsNullOrWhiteSpace(source))
        {
            queryBuilder = queryBuilder.Where(s => s.Route.Source.ToLower().Contains(source.ToLower()));
        }

        // Specific destination filter (if provided separately)
        if (!string.IsNullOrWhiteSpace(destination))
        {
            queryBuilder = queryBuilder.Where(s => s.Route.Destination.ToLower().Contains(destination.ToLower()));
        }

        // Filter by date if provided
        if (date.HasValue)
        {
            var dateUtc = date.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
                : date.Value.ToUniversalTime();
            var startOfDay = dateUtc.Date;
            var endOfDay = startOfDay.AddDays(1);
            queryBuilder = queryBuilder.Where(s => s.DepartureTime >= startOfDay && s.DepartureTime < endOfDay);
        }
        else
        {
            // If no date specified, only show upcoming buses
            var now = DateTime.UtcNow;
            queryBuilder = queryBuilder.Where(s => s.DepartureTime >= now);
        }

        // Specific operator name filter (if provided separately)
        if (!string.IsNullOrWhiteSpace(operatorName))
        {
            queryBuilder = queryBuilder.Where(s => s.Bus.Operator.User.Name.ToLower().Contains(operatorName.ToLower()));
        }

        // Price range filters
        if (minPrice.HasValue)
            queryBuilder = queryBuilder.Where(s => s.PricePerSeat >= minPrice.Value);

        if (maxPrice.HasValue)
            queryBuilder = queryBuilder.Where(s => s.PricePerSeat <= maxPrice.Value);

        // Time filters (if date is provided)
        if (date.HasValue)
        {
            var dateUtc = date.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
                : date.Value.ToUniversalTime();
            var startOfDay = dateUtc.Date;

            if (departureAfter.HasValue)
            {
                var afterDateTime = startOfDay.Add(departureAfter.Value);
                queryBuilder = queryBuilder.Where(s => s.DepartureTime >= afterDateTime);
            }

            if (departureBefore.HasValue)
            {
                var beforeDateTime = startOfDay.Add(departureBefore.Value);
                queryBuilder = queryBuilder.Where(s => s.DepartureTime <= beforeDateTime);
            }
        }

        return await queryBuilder
            .OrderBy(s => s.DepartureTime)
            .Select(s => new BusSearchResponse(
                s.Id,
                s.Bus.Name,
                s.Bus.Operator.User.Name,
                s.Route.Source,
                s.Route.Destination,
                s.DepartureTime,
                s.ArrivalTime,
                s.PricePerSeat,
                s.Bus.TotalSeats,
                s.Seats.Count(st => st.Status == SeatStatus.Available),
                s.BoardingPoint,
                s.DropPoint
            )).ToListAsync();
    }

    public async Task<IEnumerable<BusSearchResponse>> GetAllUpcomingBusesAsync()
    {
        var now = DateTime.UtcNow;

        return await _context.Schedules
            .Include(s => s.Bus)
                .ThenInclude(b => b.Operator)
                    .ThenInclude(o => o.User)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .Where(s => s.Status == ScheduleStatus.Approved && !s.Bus.IsDeleted)
            .Where(s => s.DepartureTime >= now)
            .OrderBy(s => s.DepartureTime)
            .Select(s => new BusSearchResponse(
                s.Id,
                s.Bus.Name,
                s.Bus.Operator.User.Name,
                s.Route.Source,
                s.Route.Destination,
                s.DepartureTime,
                s.ArrivalTime,
                s.PricePerSeat,
                s.Bus.TotalSeats,
                s.Seats.Count(st => st.Status == SeatStatus.Available),
                s.BoardingPoint,
                s.DropPoint
            )).ToListAsync();
    }

    public async Task<IEnumerable<BusSearchResponse>> GetAllApprovedBusesAsync()
    {
        return await _context.Schedules
            .Include(s => s.Bus)
                .ThenInclude(b => b.Operator)
                    .ThenInclude(o => o.User)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .Where(s => s.Status == ScheduleStatus.Approved && !s.Bus.IsDeleted)
            .OrderByDescending(s => s.DepartureTime)
            .Select(s => new BusSearchResponse(
                s.Id,
                s.Bus.Name,
                s.Bus.Operator.User.Name,
                s.Route.Source,
                s.Route.Destination,
                s.DepartureTime,
                s.ArrivalTime,
                s.PricePerSeat,
                s.Bus.TotalSeats,
                s.Seats.Count(st => st.Status == SeatStatus.Available),
                s.BoardingPoint,
                s.DropPoint
            )).ToListAsync();
    }

    public async Task<ScheduleSeatMapResponse?> GetSeatMapAsync(int scheduleId)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Seats)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule == null) return null;

        return new ScheduleSeatMapResponse(
            schedule.Id,
            schedule.Bus.SeatLayout?.RootElement,
            schedule.Seats.Select(st => new SeatStatusResponse(st.SeatNumber, st.Status))
        );
    }
}
