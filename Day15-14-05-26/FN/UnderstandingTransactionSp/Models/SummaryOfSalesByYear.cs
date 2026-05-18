using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class SummaryOfSalesByYear
{
    [Column("shippeddate", TypeName = "timestamp without time zone")]
    public DateTime? Shippeddate { get; set; }

    [Column("orderid")]
    public int? Orderid { get; set; }

    [Column("subtotal")]
    public decimal? Subtotal { get; set; }
}
