using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
[Table("account1")]
public partial class Account1
{
    [Column("aacno")]
    public int? Aacno { get; set; }

    [Column("balance")]
    public double? Balance { get; set; }
}
