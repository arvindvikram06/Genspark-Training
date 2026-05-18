using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class CustomerAndSuppliersByCity
{
    [Column("city")]
    [StringLength(15)]
    public string? City { get; set; }

    [Column("companyname")]
    [StringLength(40)]
    public string? Companyname { get; set; }

    [Column("contactname")]
    [StringLength(30)]
    public string? Contactname { get; set; }

    [Column("relationship")]
    public string? Relationship { get; set; }
}
