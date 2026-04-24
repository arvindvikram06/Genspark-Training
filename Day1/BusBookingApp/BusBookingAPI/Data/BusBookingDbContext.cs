using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI.Data;

public class BusBookingDbContext : DbContext
{
    public BusBookingDbContext(DbContextOptions<BusBookingDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<OperatorOffice> OperatorOffices { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<BusBookingAPI.Models.Route> Routes { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatHold> SeatHolds { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<AppConfig> AppConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Indexes as requested
        modelBuilder.Entity<Operator>().HasIndex(o => o.UserId);
        modelBuilder.Entity<OperatorOffice>().HasIndex(oo => oo.OperatorId);
        modelBuilder.Entity<Bus>().HasIndex(b => b.OperatorId);
        modelBuilder.Entity<Schedule>().HasIndex(s => s.BusId);
        modelBuilder.Entity<Schedule>().HasIndex(s => s.RouteId);
        modelBuilder.Entity<Seat>().HasIndex(s => s.ScheduleId);
        modelBuilder.Entity<SeatHold>().HasIndex(sh => sh.ScheduleId);
        modelBuilder.Entity<SeatHold>().HasIndex(sh => sh.UserId);
        modelBuilder.Entity<Booking>().HasIndex(b => b.UserId);
        modelBuilder.Entity<Booking>().HasIndex(b => b.ScheduleId);
        modelBuilder.Entity<BookingSeat>().HasIndex(bs => bs.BookingId);
        modelBuilder.Entity<BookingSeat>().HasIndex(bs => bs.SeatId);
        modelBuilder.Entity<Payment>().HasIndex(p => p.BookingId);
        modelBuilder.Entity<Payment>().HasIndex(p => p.UserId);

        // One-to-one User-Operator
        modelBuilder.Entity<User>()
            .HasOne(u => u.Operator)
            .WithOne(o => o.User)
            .HasForeignKey<Operator>(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed Default AppConfig
        modelBuilder.Entity<AppConfig>().HasData(new AppConfig
        {
            Id = 1,
            Key = "ConvenienceFeePerSeat",
            Value = "10"
        });
    }
}
