using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class OperatorOffice
{
    public int Id { get; set; }
    
    public int OperatorId { get; set; }
    
    [ForeignKey("OperatorId")]
    public virtual Operator Operator { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string District { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;
}
