using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class ProductsAboveAveragePrice
{
    [Column("productname")]
    [StringLength(40)]
    public string? Productname { get; set; }

    [Column("unitprice")]
    [Precision(19, 4)]
    public decimal? Unitprice { get; set; }
}
