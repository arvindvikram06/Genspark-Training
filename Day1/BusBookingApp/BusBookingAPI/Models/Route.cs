using System.ComponentModel.DataAnnotations;

namespace BusBookingAPI.Models;

public class Route
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Source { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Destination { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
