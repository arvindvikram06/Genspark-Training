using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("trans")]
public partial class Tran
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("fromacc")]
    public int? Fromacc { get; set; }

    [Column("toacc")]
    public int? Toacc { get; set; }

    [Column("amount")]
    public double? Amount { get; set; }
}
