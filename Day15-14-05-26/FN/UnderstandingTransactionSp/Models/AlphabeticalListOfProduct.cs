using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class AlphabeticalListOfProduct
{
    [Column("productid")]
    public int? Productid { get; set; }

    [Column("productname")]
    [StringLength(40)]
    public string? Productname { get; set; }

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
    public short? Discontinued { get; set; }

    [Column("categoryname")]
    [StringLength(15)]
    public string? Categoryname { get; set; }
}
