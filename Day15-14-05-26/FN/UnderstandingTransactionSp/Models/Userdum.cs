using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
[Table("userdum")]
public partial class Userdum
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string? Name { get; set; }

    [Column("password")]
    [StringLength(50)]
    public string? Password { get; set; }
}
