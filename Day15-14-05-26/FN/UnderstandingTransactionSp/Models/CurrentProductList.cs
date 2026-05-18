using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class CurrentProductList
{
    [Column("productid")]
    public int? Productid { get; set; }

    [Column("productname")]
    [StringLength(40)]
    public string? Productname { get; set; }
}
