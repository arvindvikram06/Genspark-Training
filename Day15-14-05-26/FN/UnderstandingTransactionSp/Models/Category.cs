using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("categories")]
[Index("Categoryname", Name = "idx_categories_categoryname")]
public partial class Category
{
    [Key]
    [Column("categoryid")]
    public int Categoryid { get; set; }

    [Column("categoryname")]
    [StringLength(15)]
    public string Categoryname { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("picture")]
    public byte[]? Picture { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
