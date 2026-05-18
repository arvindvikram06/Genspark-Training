using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("account")]
public partial class Account
{
    [Key]
    [Column("accno")]
    public int Accno { get; set; }

    [Column("balance")]
    public double? Balance { get; set; }
}
