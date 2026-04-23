using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class SeatHold
{
    public int Id { get; set; }
    
    public int ScheduleId { get; set; }
    
    [ForeignKey("ScheduleId")]
    public virtual Schedule Schedule { get; set; } = null!;
    
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    public string[] SeatNumbers { get; set; } = Array.Empty<string>();
    
    public DateTime HeldAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpiresAt { get; set; }
}
