using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("products")]
[Index("Categoryid", Name = "idx_products_categoriesproducts")]
[Index("Categoryid", Name = "idx_products_categoryid")]
[Index("Productname", Name = "idx_products_productname")]
[Index("Supplierid", Name = "idx_products_supplierid")]
[Index("Supplierid", Name = "idx_products_suppliersproducts")]
public partial class Product
{
    [Key]
    [Column("productid")]
    public int Productid { get; set; }

    [Column("productname")]
    [StringLength(40)]
    public string Productname { get; set; } = null!;

    [Column("supplierid")]
    public int? Supplierid { get; set; }

    [Column("categoryid")]
    public int? Categoryid { get; set; }

    [Column("quantityperunit")]
    [StringLength(20)]
    public string? Quantityperunit { get; set; }

    [Column("unitprice")]
    [Precision(19, 4)]
    public decimal? Unitprice { get; set; }

    [Column("unitsinstock")]
    public short? Unitsinstock { get; set; }

    [Column("unitsonorder")]
    public short? Unitsonorder { get; set; }

    [Column("reorderlevel")]
    public short? Reorderlevel { get; set; }

    [Column("discontinued")]
    public short Discontinued { get; set; }

    [ForeignKey("Categoryid")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("Supplierid")]
    [InverseProperty("Products")]
    public virtual Supplier? Supplier { get; set; }
}
