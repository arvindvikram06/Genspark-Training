using BusBookingAPI.Data;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IPaymentTimeoutService
{
    Task CheckAndProcessExpiredPaymentsAsync();
}

public class PaymentTimeoutService : BackgroundService, IPaymentTimeoutService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentTimeoutService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public PaymentTimeoutService(IServiceProvider serviceProvider, ILogger<PaymentTimeoutService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment Timeout Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndProcessExpiredPaymentsAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Payment Timeout Service execution");
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        _logger.LogInformation("Payment Timeout Service stopped");
    }

    public async Task CheckAndProcessExpiredPaymentsAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<BusBookingDbContext>();
                var expiredPayments = await context.Payments
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                    .Where(p => p.Status == PaymentStatus.Pending && p.ExpiresAt < DateTime.UtcNow)
                    .ToListAsync();

                foreach (var payment in expiredPayments)
                {
                    using var transaction = await context.Database.BeginTransactionAsync();
                    try
                    {
                        payment.Status = PaymentStatus.Timeout;
                        payment.FailureReason = "Payment timeout - not completed within allowed time";
                        payment.ProcessedAt = DateTime.UtcNow;

                        payment.Booking.Status = BookingStatus.Cancelled;

                        foreach (var bs in payment.Booking.BookingSeats)
                        {
                            bs.Seat.Status = SeatStatus.Available;
                        }

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Payment {PaymentId} timed out and booking {BookingId} cancelled", payment.Id, payment.BookingId);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Failed to process timeout for payment {PaymentId}", payment.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check for expired payments");
            }
        }
    }
}
