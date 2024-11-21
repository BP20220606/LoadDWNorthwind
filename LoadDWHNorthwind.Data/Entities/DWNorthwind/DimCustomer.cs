

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("DimCustomers")]
    public class DimCustomer
    {
        [Key]
        public int CustomerKey { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? Country { get; set; }

    }
}
