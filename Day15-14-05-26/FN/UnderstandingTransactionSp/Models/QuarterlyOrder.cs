using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class QuarterlyOrder
{
    [Column("customerid")]
    [StringLength(5)]
    public string? Customerid { get; set; }

    [Column("companyname")]
    [StringLength(40)]
    public string? Companyname { get; set; }

    [Column("city")]
    [StringLength(15)]
    public string? City { get; set; }

    [Column("country")]
    [StringLength(15)]
    public string? Country { get; set; }
}
