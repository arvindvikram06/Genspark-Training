using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class ProductSalesFor1997
{
    [Column("categoryname")]
    [StringLength(15)]
    public string? Categoryname { get; set; }

    [Column("productname")]
    [StringLength(40)]
    public string? Productname { get; set; }

    [Column("productsales")]
    public decimal? Productsales { get; set; }
}
