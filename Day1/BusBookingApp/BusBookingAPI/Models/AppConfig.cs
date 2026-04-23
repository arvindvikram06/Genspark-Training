using System.ComponentModel.DataAnnotations;

namespace BusBookingAPI.Models;

public class AppConfig
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Value { get; set; } = string.Empty;
}
