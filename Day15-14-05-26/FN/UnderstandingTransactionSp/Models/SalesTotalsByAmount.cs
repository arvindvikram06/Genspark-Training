using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class SalesTotalsByAmount
{
    [Column("saleamount")]
    public decimal? Saleamount { get; set; }

    [Column("orderid")]
    public int? Orderid { get; set; }

    [Column("companyname")]
    [StringLength(40)]
    public string? Companyname { get; set; }

    [Column("shippeddate", TypeName = "timestamp without time zone")]
    public DateTime? Shippeddate { get; set; }
}
