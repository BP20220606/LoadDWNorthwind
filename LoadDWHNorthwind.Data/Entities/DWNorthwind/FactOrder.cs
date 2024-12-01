

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("FactOrders",Schema ="DWH")]
    public class FactOrder
    {
        [Key]
        public int OrderID { get; set; }

        public int DateKey { get; set; }

        public int ProductKey { get; set; }

        public int EmployeeKey { get; set; }

        public int ShipperKey { get; set; }

        public int CustomerKey { get; set; }

        public string? Country { get; set; }

        public decimal TotalVentas { get; set; }

        public int CantidadVentas { get; set; }
    }
}
