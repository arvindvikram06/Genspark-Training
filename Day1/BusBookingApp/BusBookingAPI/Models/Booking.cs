using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class Booking
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    public int ScheduleId { get; set; }
    
    [ForeignKey("ScheduleId")]
    public virtual Schedule Schedule { get; set; } = null!;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal ConvenienceFee { get; set; }
    
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}
