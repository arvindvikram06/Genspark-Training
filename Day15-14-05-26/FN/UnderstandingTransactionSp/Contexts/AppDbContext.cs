using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UnderstandingTransactionSp.Models;

namespace UnderstandingTransactionSp.Contexts;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Account1> Account1s { get; set; }

    public virtual DbSet<AlphabeticalListOfProduct> AlphabeticalListOfProducts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategorySalesFor1997> CategorySalesFor1997s { get; set; }

    public virtual DbSet<CurrentProductList> CurrentProductLists { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCities { get; set; }

    public virtual DbSet<Customerdemographic> Customerdemographics { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailsExtended> OrderDetailsExtendeds { get; set; }

    public virtual DbSet<OrderSubtotal> OrderSubtotals { get; set; }

    public virtual DbSet<OrdersQry> OrdersQries { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSalesFor1997> ProductSalesFor1997s { get; set; }

    public virtual DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrices { get; set; }

    public virtual DbSet<ProductsByCategory> ProductsByCategories { get; set; }

    public virtual DbSet<QuarterlyOrder> QuarterlyOrders { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<SalesByCategory> SalesByCategories { get; set; }

    public virtual DbSet<SalesTotalsByAmount> SalesTotalsByAmounts { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarters { get; set; }

    public virtual DbSet<SummaryOfSalesByYear> SummaryOfSalesByYears { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Territory> Territories { get; set; }

    public virtual DbSet<Tran> Trans { get; set; }

    public virtual DbSet<Trans3> Trans3s { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userdum> Userda { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ecom;Username=postgres;Password=Arvind");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Accno).HasName("account_pkey");

            entity.Property(e => e.Accno).ValueGeneratedNever();
        });

        modelBuilder.Entity<AlphabeticalListOfProduct>(entity =>
        {
            entity.ToView("alphabetical_list_of_products");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("pk_categories");
        });

        modelBuilder.Entity<CategorySalesFor1997>(entity =>
        {
            entity.ToView("category_sales_for_1997");
        });

        modelBuilder.Entity<CurrentProductList>(entity =>
        {
            entity.ToView("current_product_list");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("pk_customers");

            entity.Property(e => e.Customerid).IsFixedLength();

            entity.HasMany(d => d.Customertypes).WithMany(p => p.Customers)
                .UsingEntity<Dictionary<string, object>>(
                    "Customercustomerdemo",
                    r => r.HasOne<Customerdemographic>().WithMany()
                        .HasForeignKey("Customertypeid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_customercustomerdemo"),
                    l => l.HasOne<Customer>().WithMany()
                        .HasForeignKey("Customerid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_customercustomerdemo_customers"),
                    j =>
                    {
                        j.HasKey("Customerid", "Customertypeid").HasName("pk_customercustomerdemo");
                        j.ToTable("customercustomerdemo");
                        j.IndexerProperty<string>("Customerid")
                            .HasMaxLength(5)
                            .IsFixedLength()
                            .HasColumnName("customerid");
                        j.IndexerProperty<string>("Customertypeid")
                            .HasMaxLength(10)
                            .IsFixedLength()
                            .HasColumnName("customertypeid");
                    });
        });

        modelBuilder.Entity<CustomerAndSuppliersByCity>(entity =>
        {
            entity.ToView("customer_and_suppliers_by_city");
        });

        modelBuilder.Entity<Customerdemographic>(entity =>
        {
            entity.HasKey(e => e.Customertypeid).HasName("pk_customerdemographics");

            entity.Property(e => e.Customertypeid).IsFixedLength();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Employeeid).HasName("pk_employees");

            entity.HasOne(d => d.ReportstoNavigation).WithMany(p => p.InverseReportstoNavigation).HasConstraintName("fk_employees_employees");

            entity.HasMany(d => d.Territories).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "Employeeterritory",
                    r => r.HasOne<Territory>().WithMany()
                        .HasForeignKey("Territoryid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_employeeterritories_territories"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("Employeeid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_employeeterritories_employees"),
                    j =>
                    {
                        j.HasKey("Employeeid", "Territoryid").HasName("pk_employeeterritories");
                        j.ToTable("employeeterritories");
                        j.IndexerProperty<int>("Employeeid").HasColumnName("employeeid");
                        j.IndexerProperty<string>("Territoryid")
                            .HasMaxLength(20)
                            .HasColumnName("territoryid");
                    });
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToView("invoices");

            entity.Property(e => e.Customerid).IsFixedLength();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("pk_orders");

            entity.Property(e => e.Customerid).IsFixedLength();
            entity.Property(e => e.Freight).HasDefaultValueSql("0");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("fk_orders_customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders).HasConstraintName("fk_orders_employees");

            entity.HasOne(d => d.ShipviaNavigation).WithMany(p => p.Orders).HasConstraintName("fk_orders_shippers");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.Orderid, e.Productid }).HasName("pk_order_details");

            entity.Property(e => e.Quantity).HasDefaultValue((short)1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_details_orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_details_products");
        });

        modelBuilder.Entity<OrderDetailsExtended>(entity =>
        {
            entity.ToView("order_details_extended");
        });

        modelBuilder.Entity<OrderSubtotal>(entity =>
        {
            entity.ToView("order_subtotals");
        });

        modelBuilder.Entity<OrdersQry>(entity =>
        {
            entity.ToView("orders_qry");

            entity.Property(e => e.Customerid).IsFixedLength();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("pk_products");

            entity.Property(e => e.Discontinued).HasDefaultValue((short)0);
            entity.Property(e => e.Reorderlevel).HasDefaultValue((short)0);
            entity.Property(e => e.Unitprice).HasDefaultValueSql("0");
            entity.Property(e => e.Unitsinstock).HasDefaultValue((short)0);
            entity.Property(e => e.Unitsonorder).HasDefaultValue((short)0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("fk_products_categories");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products).HasConstraintName("fk_products_suppliers");
        });

        modelBuilder.Entity<ProductSalesFor1997>(entity =>
        {
            entity.ToView("product_sales_for_1997");
        });

        modelBuilder.Entity<ProductsAboveAveragePrice>(entity =>
        {
            entity.ToView("products_above_average_price");
        });

        modelBuilder.Entity<ProductsByCategory>(entity =>
        {
            entity.ToView("products_by_category");
        });

        modelBuilder.Entity<QuarterlyOrder>(entity =>
        {
            entity.ToView("quarterly_orders");

            entity.Property(e => e.Customerid).IsFixedLength();
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Regionid).HasName("pk_region");

            entity.Property(e => e.Regionid).ValueGeneratedNever();
            entity.Property(e => e.Regiondescription).IsFixedLength();
        });

        modelBuilder.Entity<SalesByCategory>(entity =>
        {
            entity.ToView("sales_by_category");
        });

        modelBuilder.Entity<SalesTotalsByAmount>(entity =>
        {
            entity.ToView("sales_totals_by_amount");
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasKey(e => e.Shipperid).HasName("pk_shippers");
        });

        modelBuilder.Entity<SummaryOfSalesByQuarter>(entity =>
        {
            entity.ToView("summary_of_sales_by_quarter");
        });

        modelBuilder.Entity<SummaryOfSalesByYear>(entity =>
        {
            entity.ToView("summary_of_sales_by_year");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Supplierid).HasName("pk_suppliers");
        });

        modelBuilder.Entity<Territory>(entity =>
        {
            entity.HasKey(e => e.Territoryid).HasName("pk_territories");

            entity.Property(e => e.Territorydescription).IsFixedLength();

            entity.HasOne(d => d.Region).WithMany(p => p.Territories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_territories_region");
        });

        modelBuilder.Entity<Tran>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trans_pkey");
        });

        modelBuilder.Entity<Trans3>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trans_pkey1");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
