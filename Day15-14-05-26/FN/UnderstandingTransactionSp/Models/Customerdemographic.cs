using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("customerdemographics")]
public partial class Customerdemographic
{
    [Key]
    [Column("customertypeid")]
    [StringLength(10)]
    public string Customertypeid { get; set; } = null!;

    [Column("customerdesc")]
    public string? Customerdesc { get; set; }

    [ForeignKey("Customertypeid")]
    [InverseProperty("Customertypes")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
