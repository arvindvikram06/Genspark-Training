using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BusBookingAPI.Services;

public interface IScheduleService
{
    Task<IEnumerable<RouteDto>> GetApprovedRoutesAsync();
    Task<(bool Success, string Message, ScheduleResponse? Data)> CreateScheduleAsync(int userId, CreateScheduleRequest request);
    Task<IEnumerable<ScheduleResponse>> GetOperatorSchedulesAsync(int userId);
    Task<(bool Success, string Message)> UpdateScheduleAsync(int userId, int scheduleId, CreateScheduleRequest request);
    Task<(bool Success, string Message)> CancelScheduleAsync(int userId, int scheduleId);
    Task<object?> GetScheduleSeatsAsync(int userId, int scheduleId);
}

public class ScheduleService : IScheduleService
{
    private readonly BusBookingDbContext _context;

    public ScheduleService(BusBookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RouteDto>> GetApprovedRoutesAsync()
    {
        return await _context.Routes
            .Where(r => !r.IsDeleted)
            .Select(r => new RouteDto(r.Id, r.Source, r.Destination, r.CreatedAt))
            .ToListAsync();
    }

    public async Task<(bool Success, string Message, ScheduleResponse? Data)> CreateScheduleAsync(int userId, CreateScheduleRequest request)
    {
        // Convert to UTC for PostgreSQL compatibility while preserving the time value
        var departureTime = request.DepartureTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.DepartureTime, DateTimeKind.Utc)
            : request.DepartureTime.ToUniversalTime();

        var arrivalTime = request.ArrivalTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.ArrivalTime, DateTimeKind.Utc)
            : request.ArrivalTime.ToUniversalTime();

        var op = await _context.Operators
            .Include(o => o.Offices)
            .FirstOrDefaultAsync(o => o.UserId == userId);
        
        if (op == null) return (false, "Operator not found", null);

        var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == request.BusId && b.OperatorId == op.Id);
        if (bus == null) return (false, "Bus not found or does not belong to you", null);

        // Check if bus is disabled during the requested time period
        if (bus.DisabledFrom.HasValue && bus.DisabledTo.HasValue)
        {
            if (departureTime >= bus.DisabledFrom.Value && departureTime <= bus.DisabledTo.Value)
                return (false, "Bus is disabled during the requested departure time", null);
            if (arrivalTime >= bus.DisabledFrom.Value && arrivalTime <= bus.DisabledTo.Value)
                return (false, "Bus is disabled during the requested arrival time", null);
            if (departureTime < bus.DisabledFrom.Value && arrivalTime > bus.DisabledTo.Value)
                return (false, "Bus is disabled during the requested time period", null);
        }

        var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == request.RouteId && !r.IsDeleted);
        if (route == null) return (false, "Route not found or inactive", null);

        // Auto-resolve Boarding and Drop points
        var sourceOffice = op.Offices.FirstOrDefault(of => of.District.Equals(route.Source, StringComparison.OrdinalIgnoreCase));
        var destOffice = op.Offices.FirstOrDefault(of => of.District.Equals(route.Destination, StringComparison.OrdinalIgnoreCase));

        if (sourceOffice == null) return (false, $"You must have an office in {route.Source} for Boarding Point resolution", null);
        if (destOffice == null) return (false, $"You must have an office in {route.Destination} for Drop Point resolution", null);

        // Validation: Departure must be before Arrival
        if (departureTime >= arrivalTime)
            return (false, "Departure time must be before arrival time", null);

        // Overlap Check: Ensure bus is not busy during this time
        var hasOverlap = await _context.Schedules
            .AnyAsync(s => s.BusId == request.BusId && 
                           s.Status != ScheduleStatus.Cancelled &&
                           departureTime < s.ArrivalTime && 
                           arrivalTime > s.DepartureTime);

        if (hasOverlap)
            return (false, "This bus is already scheduled for another trip during the selected time period.", null);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var schedule = new Schedule
            {
                BusId = request.BusId,
                RouteId = request.RouteId,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                PricePerSeat = request.PricePerSeat,
                BoardingPoint = sourceOffice.Address,
                DropPoint = destOffice.Address,
                Status = ScheduleStatus.Pending
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Create Seats from Bus Layout
            if (bus.SeatLayout != null)
            {
                var layout = bus.SeatLayout.RootElement;
                if (layout.TryGetProperty("seats", out JsonElement seats) && seats.ValueKind == JsonValueKind.Array)
                {
                    foreach (var seatElement in seats.EnumerateArray())
                    {
                        var seatNumber = seatElement.GetProperty("seatNumber").GetString();
                        if (!string.IsNullOrEmpty(seatNumber))
                        {
                            _context.Seats.Add(new Seat
                            {
                                ScheduleId = schedule.Id,
                                SeatNumber = seatNumber,
                                Status = SeatStatus.Available
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();

            return (true, "Schedule created successfully", await MapToResponse(schedule.Id));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Internal Error: {ex.Message}", null);
        }
    }

    public async Task<IEnumerable<ScheduleResponse>> GetOperatorSchedulesAsync(int userId)
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Where(s => s.Bus.Operator.UserId == userId)
            .Select(s => new ScheduleResponse(
                s.Id, s.BusId, s.Bus.Name, s.RouteId, s.Route.Source, s.Route.Destination,
                s.DepartureTime, s.ArrivalTime, s.PricePerSeat, s.BoardingPoint, s.DropPoint, s.Status,
                s.Bus.TotalSeats,
                s.Seats.Count(seat => seat.Status == SeatStatus.Available)
            )).ToListAsync();
    }

    public async Task<(bool Success, string Message)> UpdateScheduleAsync(int userId, int scheduleId, CreateScheduleRequest request)
    {
        // Ensure all DateTime values are UTC for PostgreSQL compatibility
        var departureTime = request.DepartureTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.DepartureTime, DateTimeKind.Utc)
            : request.DepartureTime.ToUniversalTime();
        
        var arrivalTime = request.ArrivalTime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.ArrivalTime, DateTimeKind.Utc)
            : request.ArrivalTime.ToUniversalTime();

        var schedule = await _context.Schedules
            .Include(s => s.Bus)
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Bus.Operator.UserId == userId);
        
        if (schedule == null) return (false, "Schedule not found");
        if (schedule.Status != ScheduleStatus.Pending) return (false, "Only pending schedules can be updated");

        schedule.BusId = request.BusId;
        schedule.RouteId = request.RouteId;
        schedule.DepartureTime = departureTime;
        schedule.ArrivalTime = arrivalTime;
        schedule.PricePerSeat = request.PricePerSeat;

        await _context.SaveChangesAsync();
        return (true, "Schedule updated successfully");
    }

    public async Task<(bool Success, string Message)> CancelScheduleAsync(int userId, int scheduleId)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Bus.Operator.UserId == userId);

        if (schedule == null) return (false, "Schedule not found");

        if (schedule.Bookings.Any(b => b.Status == BookingStatus.Confirmed))
            return (false, "Cannot cancel schedule with confirmed bookings");

        schedule.Status = ScheduleStatus.Cancelled;
        await _context.SaveChangesAsync();
        return (true, "Schedule cancelled successfully");
    }

    public async Task<object?> GetScheduleSeatsAsync(int userId, int scheduleId)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .ThenInclude(seat => seat.BookingSeats)
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Bus.Operator.UserId == userId);

        if (schedule == null) return null;

        var seats = schedule.Seats.Select(seat => new
        {
            seat.SeatNumber,
            seat.Status,
            passengerName = seat.BookingSeats.FirstOrDefault()?.PassengerName,
            passengerAge = seat.BookingSeats.FirstOrDefault()?.PassengerAge,
            passengerGender = seat.BookingSeats.FirstOrDefault()?.PassengerGender
        }).ToList();

        return new
        {
            scheduleId = schedule.Id,
            busName = schedule.Bus.Name,
            source = schedule.Route.Source,
            destination = schedule.Route.Destination,
            totalSeats = schedule.Bus.TotalSeats,
            seats
        };
    }

    private async Task<ScheduleResponse?> MapToResponse(int id)
    {
        return await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Route)
            .Where(s => s.Id == id)
            .Select(s => new ScheduleResponse(
                s.Id, s.BusId, s.Bus.Name, s.RouteId, s.Route.Source, s.Route.Destination,
                s.DepartureTime, s.ArrivalTime, s.PricePerSeat, s.BoardingPoint, s.DropPoint, s.Status,
                s.Bus.TotalSeats,
                s.Seats.Count(seat => seat.Status == SeatStatus.Available)
            )).FirstOrDefaultAsync();
    }
}
