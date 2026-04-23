using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IBookingService
{
    Task<(bool Success, string Message, SeatHoldResponse? Data)> HoldSeatsAsync(int userId, SeatHoldRequest request);
    Task<bool> ReleaseHoldAsync(int userId, int holdId);
    Task<(bool Success, string Message, BookingResponse? Data)> ConfirmBookingAsync(int userId, BookingConfirmRequest request);
    Task<IEnumerable<BookingSummaryResponse>> GetMyBookingsAsync(int userId);
    Task<BookingResponse?> GetBookingDetailAsync(int userId, int bookingId);
    Task<(bool Success, string Message)> CancelBookingAsync(int userId, int bookingId);
}

public class BookingService : IBookingService
{
    private readonly BusBookingDbContext _context;

    public BookingService(BusBookingDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message, SeatHoldResponse? Data)> HoldSeatsAsync(int userId, SeatHoldRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Lock the seats for update to prevent race conditions
            var seats = await _context.Seats
                .FromSqlInterpolated($"SELECT * FROM \"Seats\" WHERE \"ScheduleId\" = {request.ScheduleId} AND \"SeatNumber\" = ANY({request.SeatNumbers}) FOR UPDATE")
                .ToListAsync();

            if (seats.Count != request.SeatNumbers.Length)
                return (false, "One or more seats were not found", null);

            if (seats.Any(s => s.Status != SeatStatus.Available))
                return (false, "One or more seats are already booked or held", null);

            var expiresAt = DateTime.UtcNow.AddMinutes(10);
            var hold = new SeatHold
            {
                UserId = userId,
                ScheduleId = request.ScheduleId,
                SeatNumbers = request.SeatNumbers,
                ExpiresAt = expiresAt
            };

            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Held;
            }

            _context.SeatHolds.Add(hold);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Seats held successfully", new SeatHoldResponse(hold.Id, expiresAt));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error holding seats: {ex.Message}", null);
        }
    }

    public async Task<bool> ReleaseHoldAsync(int userId, int holdId)
    {
        var hold = await _context.SeatHolds.FirstOrDefaultAsync(h => h.Id == holdId && h.UserId == userId);
        if (hold == null) return false;

        var seats = await _context.Seats
            .Where(s => s.ScheduleId == hold.ScheduleId && hold.SeatNumbers.Contains(s.SeatNumber))
            .ToListAsync();

        foreach (var seat in seats)
        {
            if (seat.Status == SeatStatus.Held)
                seat.Status = SeatStatus.Available;
        }

        _context.SeatHolds.Remove(hold);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Success, string Message, BookingResponse? Data)> ConfirmBookingAsync(int userId, BookingConfirmRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hold = await _context.SeatHolds.FindAsync(request.HoldId);
            if (hold == null || hold.UserId != userId)
                return (false, "Hold not found", null);

            if (hold.ExpiresAt < DateTime.UtcNow)
                return (false, "Hold expired", null);

            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == hold.ScheduleId);

            if (schedule == null) return (false, "Schedule not found", null);

            var convenienceFeeStr = (await _context.AppConfigs.FirstOrDefaultAsync(c => c.Key == "ConvenienceFeePerSeat"))?.Value ?? "10";
            decimal convenienceFee = decimal.Parse(convenienceFeeStr);

            var totalAmount = (schedule.PricePerSeat + convenienceFee) * hold.SeatNumbers.Length;

            var booking = new Booking
            {
                UserId = userId,
                ScheduleId = hold.ScheduleId,
                TotalAmount = totalAmount,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var passenger in request.Passengers)
            {
                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.ScheduleId == hold.ScheduleId && s.SeatNumber == passenger.SeatNumber);
                if (seat != null)
                {
                    seat.Status = SeatStatus.Booked;
                    _context.BookingSeats.Add(new BookingSeat
                    {
                        BookingId = booking.Id,
                        SeatId = seat.Id,
                        PassengerName = passenger.Name,
                        PassengerAge = passenger.Age,
                        PassengerGender = passenger.Gender
                    });
                }
            }

            _context.SeatHolds.Remove(hold);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Booking confirmed", await GetBookingDetailInternal(booking.Id));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Error confirming booking: {ex.Message}", null);
        }
    }

    public async Task<IEnumerable<BookingSummaryResponse>> GetMyBookingsAsync(int userId)
    {
        return await _context.Bookings
            .Include(b => b.Schedule)
            .ThenInclude(s => s.Route)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookingSummaryResponse(
                b.Id,
                b.Schedule.Route.Source,
                b.Schedule.Route.Destination,
                b.Schedule.DepartureTime,
                b.TotalAmount,
                b.Status
            )).ToListAsync();
    }

    public async Task<BookingResponse?> GetBookingDetailAsync(int userId, int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null || booking.UserId != userId) return null;
        return await GetBookingDetailInternal(bookingId);
    }

    private async Task<BookingResponse?> GetBookingDetailInternal(int bookingId)
    {
        return await _context.Bookings
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Bus)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Where(b => b.Id == bookingId)
            .Select(b => new BookingResponse(
                b.Id,
                b.ScheduleId,
                b.Schedule.Bus.Name,
                b.Schedule.Route.Source,
                b.Schedule.Route.Destination,
                b.Schedule.DepartureTime,
                b.TotalAmount,
                b.Status,
                b.BookingSeats.Select(bs => new PassengerDto(bs.Seat.SeatNumber, bs.PassengerName, bs.PassengerAge, bs.PassengerGender)),
                b.Schedule.BoardingPoint,
                b.Schedule.DropPoint
            )).FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message)> CancelBookingAsync(int userId, int bookingId)
    {
        var booking = await _context.Bookings.Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat).FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
        if (booking == null) return (false, "Booking not found");

        if (booking.Status != BookingStatus.Confirmed)
            return (false, "Only confirmed bookings can be cancelled");

        booking.Status = BookingStatus.Cancelled;
        foreach (var bs in booking.BookingSeats)
        {
            bs.Seat.Status = SeatStatus.Available;
        }

        await _context.SaveChangesAsync();
        return (true, "Booking cancelled successfully");
    }
}
