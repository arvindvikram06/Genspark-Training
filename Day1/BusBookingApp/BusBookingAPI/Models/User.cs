using System.ComponentModel.DataAnnotations;

namespace BusBookingAPI.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    public UserRole Role { get; set; } = UserRole.User;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Operator? Operator { get; set; }
    public virtual ICollection<SeatHold> SeatHolds { get; set; } = new List<SeatHold>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
