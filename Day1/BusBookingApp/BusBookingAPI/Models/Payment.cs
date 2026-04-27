using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class Payment
{
    public int Id { get; set; }
    
    public int BookingId { get; set; }
    
    [ForeignKey("BookingId")]
    public virtual Booking Booking { get; set; } = null!;
    
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    public decimal Amount { get; set; }
    
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    public string? TransactionReference { get; set; }
    
    public string? IdempotencyKey { get; set; }
    
    public int RetryCount { get; set; } = 0;
    
    public string? FailureReason { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
}
