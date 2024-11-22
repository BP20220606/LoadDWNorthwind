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
        #endregion

    }
}
