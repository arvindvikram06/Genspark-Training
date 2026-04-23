using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class Schedule
{
    public int Id { get; set; }
    
    public int BusId { get; set; }
    
    [ForeignKey("BusId")]
    public virtual Bus Bus { get; set; } = null!;
    
    public int RouteId { get; set; }
    
    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;
    
    public DateTime DepartureTime { get; set; }
    
    public DateTime ArrivalTime { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerSeat { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string BoardingPoint { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string DropPoint { get; set; } = string.Empty;
    
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Pending;

    // Navigation properties
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public virtual ICollection<SeatHold> SeatHolds { get; set; } = new List<SeatHold>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
