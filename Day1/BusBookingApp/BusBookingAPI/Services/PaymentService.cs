using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IPaymentService
{
    Task<(bool Success, string Message, BookingResponse? Booking)> ProcessPaymentAsync(int userId, int paymentId, string transactionRef, string? idempotencyKey = null);
    Task<(bool Success, string Message)> AbortPendingPaymentAsync(int userId, int paymentId);
    Task<(bool Success, string Message, PaymentStatusResponse? Data)> GetPaymentStatusAsync(int userId, int paymentId);
    Task<(bool Success, string Message)> RetryPaymentAsync(int userId, int paymentId);
}

public class PaymentService : IPaymentService
{
    private readonly BusBookingDbContext _context;
    private readonly IBookingService _bookingService;
    private readonly IEmailService _emailService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(BusBookingDbContext context, IBookingService bookingService, IEmailService emailService, ILogger<PaymentService> logger)
    {
        _context = context;
        _bookingService = bookingService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, BookingResponse? Booking)> ProcessPaymentAsync(int userId, int paymentId, string transactionRef, string? idempotencyKey = null)
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
            
            if (payment.Status == PaymentStatus.Success)
            {
                var existingBooking = await _bookingService.GetBookingDetailAsync(userId, payment.BookingId);
                return (true, "Payment already processed", existingBooking);
            }
            
            if (payment.Status != PaymentStatus.Pending && payment.Status != PaymentStatus.Failed)
            {
                return (false, $"Cannot process payment with status: {payment.Status}", null);
            }

            if (payment.ExpiresAt.HasValue && payment.ExpiresAt < DateTime.UtcNow)
            {
                payment.Status = PaymentStatus.Timeout;
                payment.FailureReason = "Payment timeout - expired before processing";
                payment.ProcessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (false, "Payment has expired. Please retry booking.", null);
            }

            if (!string.IsNullOrEmpty(idempotencyKey) && payment.IdempotencyKey == idempotencyKey)
            {
                _logger.LogInformation("Duplicate payment attempt detected with idempotency key: {IdempotencyKey}", idempotencyKey);
                var idempotentBooking = await _bookingService.GetBookingDetailAsync(userId, payment.BookingId);
                return (true, "Payment already processed (idempotent)", idempotentBooking);
            }

            payment.Status = PaymentStatus.Processing;
            payment.RetryCount++;
            await _context.SaveChangesAsync();

            payment.Status = PaymentStatus.Success;
            payment.TransactionReference = transactionRef;
            payment.ProcessedAt = DateTime.UtcNow;

            payment.Booking.Status = BookingStatus.Confirmed;

            foreach (var bs in payment.Booking.BookingSeats)
            {
                bs.Seat.Status = SeatStatus.Booked;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var bookingDetail = await _bookingService.GetBookingDetailAsync(userId, payment.BookingId);
            
            // Send confirmation email
            if (bookingDetail != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    var schedule = await _context.Schedules
                        .Include(s => s.Bus)
                        .Include(s => s.Route)
                        .FirstOrDefaultAsync(s => s.Id == bookingDetail.ScheduleId);
                    
                    if (schedule != null)
                    {
                        var route = $"{schedule.Route.Source} → {schedule.Route.Destination}";
                        // Convert UTC to local time for email display
                        var localDepartureTime = schedule.DepartureTime.ToLocalTime();
                        var departureTime = localDepartureTime.ToString("f");
                        await _emailService.SendBookingConfirmationEmailAsync(
                            user.Email,
                            user.Name,
                            bookingDetail.Id,
                            schedule.Bus.Name,
                            route,
                            departureTime,
                            payment.Amount
                        );
                    }
                }
            }
            
            _logger.LogInformation("Payment processed successfully for PaymentId: {PaymentId}, BookingId: {BookingId}", paymentId, payment.BookingId);
            return (true, "Payment successful and booking confirmed", bookingDetail);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Payment processing failed for PaymentId: {PaymentId}", paymentId);
            
            try
            {
                var failedPayment = await _context.Payments.FindAsync(paymentId);
                if (failedPayment != null && failedPayment.Status == PaymentStatus.Processing)
                {
                    failedPayment.Status = PaymentStatus.Failed;
                    failedPayment.FailureReason = $"Payment processing error: {ex.Message}";
                    failedPayment.ProcessedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "Failed to update payment status after error");
            }
            
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
            if (payment.Status != PaymentStatus.Pending && payment.Status != PaymentStatus.Failed)
                return (false, $"Cannot cancel payment with status: {payment.Status}");

            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = "User cancelled payment";
            payment.ProcessedAt = DateTime.UtcNow;

            // Release seats
            foreach (var bs in payment.Booking.BookingSeats)
            {
                bs.Seat.Status = SeatStatus.Available;
            }

            // Delete booking seats
            _context.BookingSeats.RemoveRange(payment.Booking.BookingSeats);

            // Delete booking
            _context.Bookings.Remove(payment.Booking);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Payment aborted for PaymentId: {PaymentId}", paymentId);
            return (true, "Booking flow cancelled and seats released");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to abort payment for PaymentId: {PaymentId}", paymentId);
            return (false, $"Failed to cancel booking flow: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message, PaymentStatusResponse? Data)> GetPaymentStatusAsync(int userId, int paymentId)
    {
        try
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

            if (payment == null) return (false, "Payment record not found", null);

            var response = new PaymentStatusResponse(
                payment.Id,
                payment.BookingId,
                payment.Status,
                payment.Amount,
                payment.CreatedAt,
                payment.ExpiresAt,
                payment.FailureReason,
                payment.RetryCount
            );

            return (true, "Payment status retrieved", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get payment status for PaymentId: {PaymentId}", paymentId);
            return (false, $"Failed to retrieve payment status: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message)> RetryPaymentAsync(int userId, int paymentId)
    {
        try
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

            if (payment == null) return (false, "Payment record not found");
            
            if (payment.Status == PaymentStatus.Success)
                return (false, "Payment already successful");
            
            if (payment.Status != PaymentStatus.Failed && payment.Status != PaymentStatus.Timeout)
                return (false, $"Cannot retry payment with status: {payment.Status}");

            if (payment.RetryCount >= 3)
                return (false, "Maximum retry attempts exceeded. Please book again.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            payment.Status = PaymentStatus.Pending;
            payment.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
            payment.FailureReason = null;
            payment.Booking.Status = BookingStatus.PendingPayment;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Payment retry initiated for PaymentId: {PaymentId}, RetryCount: {RetryCount}", paymentId, payment.RetryCount + 1);
            return (true, "Payment retry successful. Please complete payment within 10 minutes.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retry payment for PaymentId: {PaymentId}", paymentId);
            return (false, $"Failed to retry payment: {ex.Message}");
        }
    }
}
