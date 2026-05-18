using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class CategorySalesFor1997
{
    [Column("categoryname")]
    [StringLength(15)]
    public string? Categoryname { get; set; }

    [Column("categorysales")]
    public decimal? Categorysales { get; set; }
}
