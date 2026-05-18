using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("shippers")]
public partial class Shipper
{
    [Key]
    [Column("shipperid")]
    public int Shipperid { get; set; }

    [Column("companyname")]
    [StringLength(40)]
    public string Companyname { get; set; } = null!;

    [Column("phone")]
    [StringLength(24)]
    public string? Phone { get; set; }

    [InverseProperty("ShipviaNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
