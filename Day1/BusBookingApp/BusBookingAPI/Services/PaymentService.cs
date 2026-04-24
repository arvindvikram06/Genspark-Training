using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IPaymentService
{
    Task<(bool Success, string Message, BookingResponse? Booking)> ProcessPaymentAsync(int userId, int paymentId, string transactionRef);
    Task<(bool Success, string Message)> AbortPendingPaymentAsync(int userId, int paymentId);
}

public class PaymentService : IPaymentService
{
    private readonly BusBookingDbContext _context;
    private readonly IBookingService _bookingService;

    public PaymentService(BusBookingDbContext context, IBookingService bookingService)
    {
        _context = context;
        _bookingService = bookingService;
    }

    public async Task<(bool Success, string Message, BookingResponse? Booking)> ProcessPaymentAsync(int userId, int paymentId, string transactionRef)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

            if (payment == null) return (false, "Payment record not found", null);
            if (payment.Status != PaymentStatus.Pending) return (false, "Payment already processed", null);

            // Simulate External Payment Success
            payment.Status = PaymentStatus.Success;
            payment.TransactionReference = transactionRef;

            // Update Booking
            payment.Booking.Status = BookingStatus.Confirmed;

            // Mark seats as Booked
            foreach (var bs in payment.Booking.BookingSeats)
            {
                bs.Seat.Status = SeatStatus.Booked;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var bookingDetail = await _bookingService.GetBookingDetailAsync(userId, payment.BookingId);
            return (true, "Payment successful and booking confirmed", bookingDetail);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Payment processing failed: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message)> AbortPendingPaymentAsync(int userId, int paymentId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

            if (payment == null) return (false, "Payment record not found");
            if (payment.Status != PaymentStatus.Pending) return (false, "Only pending payments can be cancelled");

            payment.Status = PaymentStatus.Failed;
            payment.Booking.Status = BookingStatus.Cancelled;

            foreach (var bs in payment.Booking.BookingSeats)
            {
                bs.Seat.Status = SeatStatus.Available;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Booking flow cancelled and seats released");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Failed to cancel booking flow: {ex.Message}");
        }
    }
}
