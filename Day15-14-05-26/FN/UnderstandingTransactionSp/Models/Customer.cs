using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("customers")]
[Index("City", Name = "idx_customers_city")]
[Index("Companyname", Name = "idx_customers_companyname")]
[Index("Postalcode", Name = "idx_customers_postalcode")]
[Index("Region", Name = "idx_customers_region")]
public partial class Customer
{
    [Key]
    [Column("customerid")]
    [StringLength(5)]
    public string Customerid { get; set; } = null!;

    [Column("companyname")]
    [StringLength(40)]
    public string Companyname { get; set; } = null!;

    [Column("contactname")]
    [StringLength(30)]
    public string? Contactname { get; set; }

    [Column("contacttitle")]
    [StringLength(30)]
    public string? Contacttitle { get; set; }

    [Column("address")]
    [StringLength(60)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(15)]
    public string? City { get; set; }

    [Column("region")]
    [StringLength(15)]
    public string? Region { get; set; }

    [Column("postalcode")]
    [StringLength(10)]
    public string? Postalcode { get; set; }

    [Column("country")]
    [StringLength(15)]
    public string? Country { get; set; }

    [Column("phone")]
    [StringLength(24)]
    public string? Phone { get; set; }

    [Column("fax")]
    [StringLength(24)]
    public string? Fax { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("Customerid")]
    [InverseProperty("Customers")]
    public virtual ICollection<Customerdemographic> Customertypes { get; set; } = new List<Customerdemographic>();
}
