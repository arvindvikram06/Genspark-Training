using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Services;

public interface IAdminService
{
    Task<IEnumerable<OperatorSummaryDto>> GetOperatorsAsync();
    Task<bool> ApproveOperatorAsync(int operatorId);
    Task<bool> DisableOperatorAsync(int operatorId);
    Task<IEnumerable<BusSummaryDto>> GetOperatorBusesAsync(int operatorId);
    Task<decimal> GetOperatorRevenueAsync(int operatorId);
    Task<IEnumerable<RouteDto>> GetRoutesAsync();
    Task<RouteDto> CreateRouteAsync(CreateRouteRequest request);
    Task<bool> UpdateRouteAsync(int id, CreateRouteRequest request);
    Task<bool> DeleteRouteAsync(int id);
    Task<bool> ApproveScheduleAsync(int scheduleId);
    Task<string> GetConvenienceFeeAsync();
    Task<bool> UpdateConvenienceFeeAsync(decimal fee);
}

public class AdminService : IAdminService
{
    private readonly BusBookingDbContext _context;

    public AdminService(BusBookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OperatorSummaryDto>> GetOperatorsAsync()
    {
        return await _context.Operators
            .Include(o => o.User)
            .Select(o => new OperatorSummaryDto(
                o.Id,
                o.User.Name,
                o.User.Email,
                o.User.Phone,
                o.Status,
                o.HeadOfficeDistrict,
                o.Buses.Count,
                o.Buses.SelectMany(b => b.Schedules).SelectMany(s => s.Bookings).Sum(bk => bk.TotalAmount)
            ))
            .ToListAsync();
    }

    public async Task<bool> ApproveOperatorAsync(int operatorId)
    {
        var op = await _context.Operators.FindAsync(operatorId);
        if (op == null) return false;
        op.Status = OperatorStatus.Approved;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DisableOperatorAsync(int operatorId)
    {
        var op = await _context.Operators.FindAsync(operatorId);
        if (op == null) return false;
        op.Status = OperatorStatus.Disabled;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BusSummaryDto>> GetOperatorBusesAsync(int operatorId)
    {
        return await _context.Buses
            .Where(b => b.OperatorId == operatorId)
            .Select(b => new BusSummaryDto(b.Id, b.Name, b.TotalSeats, b.Status, b.CreatedAt))
            .ToListAsync();
    }

    public async Task<decimal> GetOperatorRevenueAsync(int operatorId)
    {
        return await _context.Buses
            .Where(b => b.OperatorId == operatorId)
            .SelectMany(b => b.Schedules)
            .SelectMany(s => s.Bookings)
            .SumAsync(bk => bk.TotalAmount);
    }

    public async Task<IEnumerable<RouteDto>> GetRoutesAsync()
    {
        return await _context.Routes
            .Where(r => !r.IsDeleted)
            .Select(r => new RouteDto(r.Id, r.Source, r.Destination, r.CreatedAt))
            .ToListAsync();
    }

    public async Task<RouteDto> CreateRouteAsync(CreateRouteRequest request)
    {
        var route = new BusBookingAPI.Models.Route
        {
            Source = request.Source,
            Destination = request.Destination
        };
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
        return new RouteDto(route.Id, route.Source, route.Destination, route.CreatedAt);
    }

    public async Task<bool> UpdateRouteAsync(int id, CreateRouteRequest request)
    {
        var route = await _context.Routes.FindAsync(id);
        if (route == null || route.IsDeleted) return false;
        route.Source = request.Source;
        route.Destination = request.Destination;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRouteAsync(int id)
    {
        var route = await _context.Routes.Include(r => r.Schedules).FirstOrDefaultAsync(r => r.Id == id);
        if (route == null) return false;

        if (route.Schedules.Any())
        {
            route.IsDeleted = true; // Soft delete
        }
        else
        {
            _context.Routes.Remove(route); // Hard delete if no dependencies
        }
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveScheduleAsync(int scheduleId)
    {
        var schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null) return false;
        schedule.Status = ScheduleStatus.Approved;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GetConvenienceFeeAsync()
    {
        var config = await _context.AppConfigs.FirstOrDefaultAsync(c => c.Key == "ConvenienceFeePerSeat");
        return config?.Value ?? "0";
    }

    public async Task<bool> UpdateConvenienceFeeAsync(decimal fee)
    {
        var config = await _context.AppConfigs.FirstOrDefaultAsync(c => c.Key == "ConvenienceFeePerSeat");
        if (config == null)
        {
            config = new AppConfig { Key = "ConvenienceFeePerSeat", Value = fee.ToString() };
            _context.AppConfigs.Add(config);
        }
        else
        {
            config.Value = fee.ToString();
        }
        await _context.SaveChangesAsync();
        return true;
    }
}
