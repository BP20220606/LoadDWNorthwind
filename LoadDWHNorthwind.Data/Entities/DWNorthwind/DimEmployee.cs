

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("DimEmployees", Schema = "DWH")]
    public class DimEmployee
    {
        [Key]
        public int EmployeeKey { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeTitle { get; set; }


    }
}
