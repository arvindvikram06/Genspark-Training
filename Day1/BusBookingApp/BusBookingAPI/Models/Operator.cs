using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingAPI.Models;

public class Operator
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    public OperatorStatus Status { get; set; } = OperatorStatus.Pending;
    
    [Required]
    [MaxLength(100)]
    public string HeadOfficeDistrict { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<OperatorOffice> Offices { get; set; } = new List<OperatorOffice>();
    public virtual ICollection<Bus> Buses { get; set; } = new List<Bus>();
}
