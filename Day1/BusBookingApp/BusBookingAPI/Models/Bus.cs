using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BusBookingAPI.Models;

public class Bus
{
    public int Id { get; set; }
    
    public int OperatorId { get; set; }
    
    [ForeignKey("OperatorId")]
    public virtual Operator Operator { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int TotalSeats { get; set; }
    
    [Column(TypeName = "jsonb")]
    public JsonDocument? SeatLayout { get; set; }
    
    public BusStatus Status { get; set; } = BusStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsDeleted { get; set; } = false;
    
    public DateTime? DisabledFrom { get; set; }
    
    public DateTime? DisabledTo { get; set; }

    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
