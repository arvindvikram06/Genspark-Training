using BusBookingAPI.Data;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public class SeatHoldCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SeatHoldCleanupService> _logger;

    public SeatHoldCleanupService(IServiceProvider serviceProvider, ILogger<SeatHoldCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<BusBookingDbContext>();
                    var now = DateTime.UtcNow;

                    // Find expired holds
                    var expiredHolds = await context.SeatHolds
                        .Where(h => h.ExpiresAt < now)
                        .ToListAsync(stoppingToken);

                    if (expiredHolds.Any())
                    {
                        _logger.LogInformation($"Found {expiredHolds.Count} expired seat holds. Cleaning up...");

                        foreach (var hold in expiredHolds)
                        {
                            var seats = await context.Seats
                                .Where(s => s.ScheduleId == hold.ScheduleId && hold.SeatNumbers.Contains(s.SeatNumber))
                                .ToListAsync(stoppingToken);

                            foreach (var seat in seats)
                            {
                                if (seat.Status == SeatStatus.Held)
                                {
                                    seat.Status = SeatStatus.Available;
                                }
                            }
                            context.SeatHolds.Remove(hold);
                        }

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired seat holds.");
            }

            // Wait for 60 seconds
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }
}
