using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[Keyless]
public partial class OrdersQry
{
    [Column("orderid")]
    public int? Orderid { get; set; }

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

    [Column("companyname")]
    [StringLength(40)]
    public string? Companyname { get; set; }

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
}
