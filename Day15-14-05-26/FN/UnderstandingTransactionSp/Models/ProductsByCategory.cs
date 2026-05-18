using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class ProductsByCategory
{
    [Column("categoryname")]
    [StringLength(15)]
    public string? Categoryname { get; set; }

    [Column("productname")]
    [StringLength(40)]
    public string? Productname { get; set; }

    [Column("quantityperunit")]
    [StringLength(20)]
    public string? Quantityperunit { get; set; }

    [Column("unitsinstock")]
    public short? Unitsinstock { get; set; }

    [Column("discontinued")]
    public short? Discontinued { get; set; }
}
