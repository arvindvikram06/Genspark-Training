using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class Seat
{
    public int Id { get; set; }
    
    public int ScheduleId { get; set; }
    
    [ForeignKey("ScheduleId")]
    public virtual Schedule Schedule { get; set; } = null!;
    
    [Required]
    [MaxLength(10)]
    public string SeatNumber { get; set; } = string.Empty;
    
    public SeatStatus Status { get; set; } = SeatStatus.Available;

    // Navigation properties
    public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}
