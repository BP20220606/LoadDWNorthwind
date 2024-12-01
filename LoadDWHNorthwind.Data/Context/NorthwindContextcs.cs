using LoadDWHNorthwind.Data.Entities.Northwind;
using Microsoft.EntityFrameworkCore;


namespace LoadDWHNorthwind.Data.Context
{
    public partial class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
        {

        }
        #region"Db Sets"

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<VwCustomersServed> VwCustomersServed { get; set; }

        public DbSet<VwOrder> VwOrders { get; set; }


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

                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.CustomerID)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsFixedLength()
                    .HasColumnName("CustomerID");
                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.EmployeeID).HasColumnName("EmployeeID");
                entity.Property(e => e.EmployeeName)
                    .IsRequired()
                    .HasMaxLength(31);
                entity.Property(e => e.OrderID).HasColumnName("OrderID");
                entity.Property(e => e.ProductID).HasColumnName("ProductID");
                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.Shipper)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.ShipperID).HasColumnName("ShipperID");
                entity.Property(e => e.DateKey).HasColumnName("DateKey");
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
