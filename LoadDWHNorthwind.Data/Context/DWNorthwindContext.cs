using LoadDWHNorthwind.Data.Entities.DWNorthwind;
using LoadDWHNorthwind.Data.Entities.Northwind;
using Microsoft.EntityFrameworkCore;


namespace LoadDWHNorthwind.Data.Context
{
    public partial class DWNorthwindContext : DbContext
    {
        public DWNorthwindContext(DbContextOptions<DWNorthwindContext> options) : base(options)
        {

        }
        #region"Db Sets"

        public DbSet<DimEmployee> DimEmployees { get; set; }
        public DbSet<DimProduct> DimProducts { get; set; }
        public DbSet<DimShipper> DimShippers { get; set; }
        public DbSet<DimCustomer> DimCustomers { get; set; }

        public DbSet<DimDate> DimDates { get; set; }

        public DbSet<FactOrder> FactOrders { get; set; }
        public DbSet<FactCustomerServed> FactCustomersServed { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VwCustomersServed>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("VwCustomersServed");


            });
            modelBuilder.Entity<VwOrder>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("VwOrders");


            });
            modelBuilder.Entity<DimDate>(entity =>
            {
                entity.HasKey(e => e.DateKey).HasName("PK__DimDates__40DF45E300468511");

                entity.ToTable("DimDates", "DWH");

                entity.Property(e => e.DayName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.MonthName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK_Order_Details");

                entity.ToTable("Order Details");

                entity.HasIndex(e => e.OrderId, "OrderID");

                entity.HasIndex(e => e.OrderId, "OrdersOrder_Details");

                entity.HasIndex(e => e.ProductId, "ProductID");

                entity.HasIndex(e => e.ProductId, "ProductsOrder_Details");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.Quantity).HasDefaultValue((short)1);
                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Orders");

                entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Products");
            });


        }
    }
}
