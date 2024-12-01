

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("FactCustomersServed", Schema = "DWH")]
    public class FactCustomerServed
    {
        [Key]
        public int IDclientesatendidos { get; set; }
        public int EmployeeKey { get; set; }
        public string? NombreEmpleado { get; set; }
        public int TotalClientesAtendidos { get; set; }
    }
}

