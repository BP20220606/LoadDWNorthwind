using LoadDWHNorthwind.Data.Entities.DWNorthwind;
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
        #endregion
    }
}
