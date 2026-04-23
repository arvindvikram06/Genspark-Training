using System.Text.Json;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IOperatorService
{
    Task<IEnumerable<object>> GetDistrictsAsync();
    Task<(bool Success, string Message)> SubmitOfficesAsync(int userId, IEnumerable<OfficeSubmissionRequest> offices);
    Task<IEnumerable<OfficeResponse>> GetOfficesAsync(int userId);
    Task<(bool Success, string Message, BusResponse? Bus)> CreateBusAsync(int userId, CreateBusRequest request);
    Task<IEnumerable<BusResponse>> GetBusesAsync(int userId);
    Task<(bool Success, string Message)> UpdateBusAsync(int userId, int busId, CreateBusRequest request);
    Task<bool> DeleteBusAsync(int userId, int busId);
    Task<bool> DisableBusAsync(int userId, int busId, DisableBusRequest request);
    Task<IEnumerable<BookingSummaryDto>> GetBusBookingsAsync(int userId, int busId, int? scheduleId = null);
    Task<ScheduleBookingSummary?> GetScheduleBookingsAsync(int userId, int scheduleId);
}

public class OperatorService : IOperatorService
{
    private readonly BusBookingDbContext _context;
    private readonly string _districtDataPath;

    public OperatorService(BusBookingDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _districtDataPath = Path.Combine(env.ContentRootPath, "districtdata.json");
    }

    public async Task<IEnumerable<object>> GetDistrictsAsync()
    {
        if (!File.Exists(_districtDataPath)) return Enumerable.Empty<object>();
        var json = await File.ReadAllTextAsync(_districtDataPath);
        return JsonSerializer.Deserialize<IEnumerable<object>>(json) ?? Enumerable.Empty<object>();
    }

    public async Task<(bool Success, string Message)> SubmitOfficesAsync(int userId, IEnumerable<OfficeSubmissionRequest> offices)
    {
        var op = await _context.Operators.Include(o => o.Offices).FirstOrDefaultAsync(o => o.UserId == userId);
        if (op == null) return (false, "Operator not found");

        foreach (var officeRequest in offices)
        {
            var existingOffice = op.Offices.FirstOrDefault(o => o.District.Equals(officeRequest.District, StringComparison.OrdinalIgnoreCase));
            
            if (existingOffice != null)
            {
                existingOffice.Address = officeRequest.Address; // Update existing
            }
            else
            {
                _context.OperatorOffices.Add(new OperatorOffice
                {
                    OperatorId = op.Id,
                    District = officeRequest.District,
                    Address = officeRequest.Address
                });
            }
        }

        await _context.SaveChangesAsync();
        return (true, "Offices updated successfully");
    }

    public async Task<IEnumerable<OfficeResponse>> GetOfficesAsync(int userId)
    {
        return await _context.OperatorOffices
            .Where(oo => oo.Operator.UserId == userId)
            .Select(oo => new OfficeResponse(oo.Id, oo.District, oo.Address))
            .ToListAsync();
    }

    public async Task<(bool Success, string Message, BusResponse? Bus)> CreateBusAsync(int userId, CreateBusRequest request)
    {
        var op = await _context.Operators.Include(o => o.Offices).FirstOrDefaultAsync(o => o.UserId == userId);
        if (op == null) return (false, "Operator not found", null);
        if (!op.Offices.Any()) return (false, "Please submit your offices before creating a bus", null);

        // Validate SeatLayout
        if (!ValidateSeatLayout(request.TotalSeats, request.SeatLayout))
            return (false, "TotalSeats does not match SeatLayout count", null);

        var bus = new Bus
        {
            OperatorId = op.Id,
            Name = request.Name,
            TotalSeats = request.TotalSeats,
            SeatLayout = JsonDocument.Parse(request.SeatLayout.GetRawText()),
            Status = BusStatus.Pending
        };

        _context.Buses.Add(bus);
        await _context.SaveChangesAsync();

        return (true, "Bus created successfully", MapToBusResponse(bus));
    }

    public async Task<IEnumerable<BusResponse>> GetBusesAsync(int userId)
    {
        return await _context.Buses
            .Where(b => b.Operator.UserId == userId && !b.IsDeleted)
            .Select(b => MapToBusResponse(b))
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> UpdateBusAsync(int userId, int busId, CreateBusRequest request)
    {
        var bus = await _context.Buses
            .Include(b => b.Schedules)
            .ThenInclude(s => s.Bookings)
            .FirstOrDefaultAsync(b => b.Id == busId && b.Operator.UserId == userId && !b.IsDeleted);

        if (bus == null) return (false, "Bus not found");

        bool hasActiveBookings = bus.Schedules.Any(s => s.Bookings.Any(bk => bk.Status == BookingStatus.Confirmed));
        
        if (bus.Status == BusStatus.Approved && hasActiveBookings)
            return (false, "Cannot update bus with active bookings");

        if (!ValidateSeatLayout(request.TotalSeats, request.SeatLayout))
            return (false, "TotalSeats does not match SeatLayout count");

        bus.Name = request.Name;
        bus.TotalSeats = request.TotalSeats;
        bus.SeatLayout = JsonDocument.Parse(request.SeatLayout.GetRawText());
        
        await _context.SaveChangesAsync();
        return (true, "Bus updated successfully");
    }

    public async Task<bool> DeleteBusAsync(int userId, int busId)
    {
        var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == busId && b.Operator.UserId == userId);
        if (bus == null) return false;
        
        bus.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DisableBusAsync(int userId, int busId, DisableBusRequest request)
    {
        var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == busId && b.Operator.UserId == userId);
        if (bus == null) return false;

        bus.DisabledFrom = request.From;
        bus.DisabledTo = request.To;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BookingSummaryDto>> GetBusBookingsAsync(int userId, int busId, int? scheduleId = null)
    {
        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Schedule)
            .Include(b => b.BookingSeats)
            .ThenInclude(bs => bs.Seat)
            .Where(b => b.Schedule.BusId == busId && b.Schedule.Bus.Operator.UserId == userId);

        if (scheduleId.HasValue)
            query = query.Where(b => b.ScheduleId == scheduleId.Value);

        return await query.Select(b => new BookingSummaryDto(
            b.Id,
            b.User.Name,
            b.User.Email,
            b.Schedule.DepartureTime,
            b.TotalAmount,
            b.Status,
            b.BookingSeats.Select(bs => bs.Seat.SeatNumber)
        )).ToListAsync();
    }

    public async Task<ScheduleBookingSummary?> GetScheduleBookingsAsync(int userId, int scheduleId)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Bus)
            .Include(s => s.Bookings)
                .ThenInclude(b => b.User)
            .Include(s => s.Bookings)
                .ThenInclude(b => b.BookingSeats)
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.Bus.Operator.UserId == userId);

        if (schedule == null) return null;

        var bookings = schedule.Bookings.Select(b => new BookingDetailDto(
            b.Id,
            b.User.Name,
            b.CreatedAt,
            b.Status,
            b.BookingSeats.Select(bs => bs.Seat.SeatNumber)
        ));

        return new ScheduleBookingSummary(
            schedule.Id,
            schedule.Bookings.Count,
            schedule.Bookings.SelectMany(b => b.BookingSeats).Count(),
            schedule.Bookings.Where(b => b.Status == BookingStatus.Confirmed).Sum(b => b.TotalAmount),
            bookings
        );
    }

    private bool ValidateSeatLayout(int totalSeats, JsonElement layout)
    {
        try
        {
            if (layout.TryGetProperty("seats", out JsonElement seats) && seats.ValueKind == JsonValueKind.Array)
            {
                return seats.GetArrayLength() == totalSeats;
            }
            return false;
        }
        catch { return false; }
    }

    private static BusResponse MapToBusResponse(Bus b)
    {
        return new BusResponse(
            b.Id,
            b.Name,
            b.TotalSeats,
            b.SeatLayout != null ? JsonDocument.Parse(b.SeatLayout.RootElement.GetRawText()).RootElement : null,
            b.Status,
            b.CreatedAt,
            b.IsDeleted,
            b.DisabledFrom,
            b.DisabledTo
        );
    }
}
