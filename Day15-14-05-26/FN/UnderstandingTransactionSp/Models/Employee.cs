using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("employees")]
[Index("Lastname", Name = "idx_employees_lastname")]
[Index("Postalcode", Name = "idx_employees_postalcode")]
public partial class Employee
{
    [Key]
    [Column("employeeid")]
    public int Employeeid { get; set; }

    [Column("lastname")]
    [StringLength(20)]
    public string Lastname { get; set; } = null!;

    [Column("firstname")]
    [StringLength(10)]
    public string Firstname { get; set; } = null!;

    [Column("title")]
    [StringLength(30)]
    public string? Title { get; set; }

    [Column("titleofcourtesy")]
    [StringLength(25)]
    public string? Titleofcourtesy { get; set; }

    [Column("birthdate", TypeName = "timestamp without time zone")]
    public DateTime? Birthdate { get; set; }

    [Column("hiredate", TypeName = "timestamp without time zone")]
    public DateTime? Hiredate { get; set; }

    [Column("address")]
    [StringLength(60)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(15)]
    public string? City { get; set; }

    [Column("region")]
    [StringLength(15)]
    public string? Region { get; set; }

    [Column("postalcode")]
    [StringLength(10)]
    public string? Postalcode { get; set; }

    [Column("country")]
    [StringLength(15)]
    public string? Country { get; set; }

    [Column("homephone")]
    [StringLength(24)]
    public string? Homephone { get; set; }

    [Column("extension")]
    [StringLength(4)]
    public string? Extension { get; set; }

    [Column("photo")]
    public byte[]? Photo { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("reportsto")]
    public int? Reportsto { get; set; }

    [Column("photopath")]
    [StringLength(255)]
    public string? Photopath { get; set; }

    [InverseProperty("ReportstoNavigation")]
    public virtual ICollection<Employee> InverseReportstoNavigation { get; set; } = new List<Employee>();

    [InverseProperty("Employee")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("Reportsto")]
    [InverseProperty("InverseReportstoNavigation")]
    public virtual Employee? ReportstoNavigation { get; set; }

    [ForeignKey("Employeeid")]
    [InverseProperty("Employees")]
    public virtual ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
