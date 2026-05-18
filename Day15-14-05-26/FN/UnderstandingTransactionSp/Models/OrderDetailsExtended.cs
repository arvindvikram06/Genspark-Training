using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class OrderDetailsExtended
{
    [Column("orderid")]
    public int? Orderid { get; set; }

    [Column("productid")]
    public int? Productid { get; set; }

    [Column("productname")]
    [StringLength(40)]
    public string? Productname { get; set; }

    [Column("unitprice")]
    [Precision(19, 4)]
    public decimal? Unitprice { get; set; }

    [Column("quantity")]
    public short? Quantity { get; set; }

    [Column("discount")]
    public float? Discount { get; set; }

    [Column("extendedprice")]
    [Precision(19, 4)]
    public decimal? Extendedprice { get; set; }
}
