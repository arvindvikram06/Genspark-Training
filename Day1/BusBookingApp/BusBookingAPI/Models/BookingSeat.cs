using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class BookingSeat
{
    public int Id { get; set; }
    
    public int BookingId { get; set; }
    
    [ForeignKey("BookingId")]
    public virtual Booking Booking { get; set; } = null!;
    
    public int SeatId { get; set; }
    
    [ForeignKey("SeatId")]
    public virtual Seat Seat { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string PassengerName { get; set; } = string.Empty;
    
    public int PassengerAge { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string PassengerGender { get; set; } = string.Empty;
}
