using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IBusService
{
    Task<IEnumerable<BusSearchResponse>> SearchBusesAsync(
        string source, 
        string destination, 
        DateTime date, 
        decimal? minPrice = null, 
        decimal? maxPrice = null, 
        TimeSpan? departureAfter = null);
        
    Task<IEnumerable<BusSearchResponse>> GetAllUpcomingBusesAsync();
        
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
        string source, 
        string destination, 
        DateTime date, 
        decimal? minPrice = null, 
        decimal? maxPrice = null, 
        TimeSpan? departureAfter = null)
    {
        var startOfDay = date.Date.ToUniversalTime();
        var endOfDay = startOfDay.AddDays(1);

        var query = _context.Schedules
            .Include(s => s.Bus)
                .ThenInclude(b => b.Operator)
                    .ThenInclude(o => o.User)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .Where(s => s.Status == ScheduleStatus.Approved && !s.Bus.IsDeleted)
            .Where(s => s.Route.Source.ToLower() == source.ToLower() && s.Route.Destination.ToLower() == destination.ToLower())
            .Where(s => s.DepartureTime >= startOfDay && s.DepartureTime < endOfDay);

        if (minPrice.HasValue)
            query = query.Where(s => s.PricePerSeat >= minPrice.Value);
        
        if (maxPrice.HasValue)
            query = query.Where(s => s.PricePerSeat <= maxPrice.Value);

        if (departureAfter.HasValue)
        {
            // Note: In Postgres, we might need a more complex time check if DepartureTime is UTC
            // But for search, we can just compare the DateTime
            var afterDateTime = startOfDay.Add(departureAfter.Value);
            query = query.Where(s => s.DepartureTime >= afterDateTime);
        }

        return await query.Select(s => new BusSearchResponse(
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
