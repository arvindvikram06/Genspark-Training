using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("suppliers")]
[Index("Companyname", Name = "idx_suppliers_companyname")]
[Index("Postalcode", Name = "idx_suppliers_postalcode")]
public partial class Supplier
{
    [Key]
    [Column("supplierid")]
    public int Supplierid { get; set; }

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

    [Column("homepage")]
    public string? Homepage { get; set; }

    [InverseProperty("Supplier")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
