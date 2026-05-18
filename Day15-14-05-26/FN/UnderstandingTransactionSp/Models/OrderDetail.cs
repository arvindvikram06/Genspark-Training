using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnderstandingTransactionSp.Models;

[PrimaryKey("Orderid", "Productid")]
[Table("order_details")]
[Index("Orderid", Name = "idx_order_details_orderid")]
[Index("Orderid", Name = "idx_order_details_ordersorder_details")]
[Index("Productid", Name = "idx_order_details_productid")]
[Index("Productid", Name = "idx_order_details_productsorder_details")]
public partial class OrderDetail
{
    [Key]
    [Column("orderid")]
    public int Orderid { get; set; }

    [Key]
    [Column("productid")]
    public int Productid { get; set; }

    [Column("unitprice")]
    [Precision(19, 4)]
    public decimal Unitprice { get; set; }

    [Column("quantity")]
    public short Quantity { get; set; }

    [Column("discount")]
    public float Discount { get; set; }

    [ForeignKey("Orderid")]
    [InverseProperty("OrderDetails")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("Productid")]
    [InverseProperty("OrderDetails")]
    public virtual Product Product { get; set; } = null!;
}
