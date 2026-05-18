using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Table("orders")]
[Index("Customerid", Name = "idx_orders_customerid")]
[Index("Customerid", Name = "idx_orders_customersorders")]
[Index("Employeeid", Name = "idx_orders_employeeid")]
[Index("Employeeid", Name = "idx_orders_employeesorders")]
[Index("Orderdate", Name = "idx_orders_orderdate")]
[Index("Shippeddate", Name = "idx_orders_shippeddate")]
[Index("Shipvia", Name = "idx_orders_shippersorders")]
[Index("Shippostalcode", Name = "idx_orders_shippostalcode")]
public partial class Order
{
    [Key]
    [Column("orderid")]
    public int Orderid { get; set; }

    [Column("customerid")]
    [StringLength(5)]
    public string? Customerid { get; set; }

    [Column("employeeid")]
    public int? Employeeid { get; set; }

    [Column("orderdate", TypeName = "timestamp without time zone")]
    public DateTime? Orderdate { get; set; }

    [Column("requireddate", TypeName = "timestamp without time zone")]
    public DateTime? Requireddate { get; set; }

    [Column("shippeddate", TypeName = "timestamp without time zone")]
    public DateTime? Shippeddate { get; set; }

    [Column("shipvia")]
    public int? Shipvia { get; set; }

    [Column("freight")]
    [Precision(19, 4)]
    public decimal? Freight { get; set; }

    [Column("shipname")]
    [StringLength(40)]
    public string? Shipname { get; set; }

    [Column("shipaddress")]
    [StringLength(60)]
    public string? Shipaddress { get; set; }

    [Column("shipcity")]
    [StringLength(15)]
    public string? Shipcity { get; set; }

    [Column("shipregion")]
    [StringLength(15)]
    public string? Shipregion { get; set; }

    [Column("shippostalcode")]
    [StringLength(10)]
    public string? Shippostalcode { get; set; }

    [Column("shipcountry")]
    [StringLength(15)]
    public string? Shipcountry { get; set; }

    [ForeignKey("Customerid")]
    [InverseProperty("Orders")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("Employeeid")]
    [InverseProperty("Orders")]
    public virtual Employee? Employee { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("Shipvia")]
    [InverseProperty("Orders")]
    public virtual Shipper? ShipviaNavigation { get; set; }
}
