

using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.Northwind
{
    public class VwOrder
    {
        public int OrderID { get; set; }
 
        public string? CustomerID { get; set; }

        public string? CustomerName { get; set; }

        public int EmployeeID { get; set; }

        public string? EmployeeName { get; set; }

        public int ShipperID { get; set; }

        public string? Shipper { get; set; }
        public string? Country { get; set; }

        public int ProductID { get; set; }

        public string? ProductName { get; set; }

        public int? DateKey { get; set; }

        public int? Año { get; set; }

        public int? Mes { get; set; }

        public double? TotalVentas { get; set; }

        public int? Cantidad { get; set; }
    }
}