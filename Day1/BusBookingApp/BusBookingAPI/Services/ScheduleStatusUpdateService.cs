using BusBookingAPI.Data;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IScheduleStatusUpdateService
{
    Task UpdateScheduleStatusesAsync();
}

public class ScheduleStatusUpdateService : BackgroundService, IScheduleStatusUpdateService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduleStatusUpdateService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public ScheduleStatusUpdateService(IServiceProvider serviceProvider, ILogger<ScheduleStatusUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Schedule Status Update Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateScheduleStatusesAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Schedule Status Update Service execution");
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        _logger.LogInformation("Schedule Status Update Service stopped");
    }

    public async Task UpdateScheduleStatusesAsync()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<BusBookingDbContext>();
                var now = DateTime.UtcNow;

                // Update schedules that should be Ongoing (departure time passed but not yet arrived)
                var schedulesToOngoing = await context.Schedules
                    .Where(s => s.Status == ScheduleStatus.Approved && 
                               s.DepartureTime <= now && 
                               s.ArrivalTime > now)
                    .ToListAsync();

                foreach (var schedule in schedulesToOngoing)
                {
                    schedule.Status = ScheduleStatus.Ongoing;
                    _logger.LogInformation("Schedule {ScheduleId} marked as Ongoing", schedule.Id);
                }

                // Update schedules that should be Completed (arrival time passed)
                var schedulesToCompleted = await context.Schedules
                    .Where(s => (s.Status == ScheduleStatus.Approved || s.Status == ScheduleStatus.Ongoing) && 
                               s.ArrivalTime <= now)
                    .ToListAsync();

                foreach (var schedule in schedulesToCompleted)
                {
                    schedule.Status = ScheduleStatus.Completed;
                    _logger.LogInformation("Schedule {ScheduleId} marked as Completed", schedule.Id);
                }

                if (schedulesToOngoing.Count > 0 || schedulesToCompleted.Count > 0)
                {
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update schedule statuses");
            }
        }
    }
}
