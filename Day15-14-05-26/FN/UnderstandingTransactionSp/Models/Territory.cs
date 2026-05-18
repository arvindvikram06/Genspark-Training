using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("territories")]
public partial class Territory
{
    [Key]
    [Column("territoryid")]
    [StringLength(20)]
    public string Territoryid { get; set; } = null!;

    [Column("territorydescription")]
    [StringLength(50)]
    public string Territorydescription { get; set; } = null!;

    [Column("regionid")]
    public int Regionid { get; set; }

    [ForeignKey("Regionid")]
    [InverseProperty("Territories")]
    public virtual Region Region { get; set; } = null!;

    [ForeignKey("Territoryid")]
    [InverseProperty("Territories")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
