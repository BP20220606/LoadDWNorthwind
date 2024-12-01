

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("DimProducts", Schema = "DWH")]
    public class DimProduct
    {
        [Key]
        public int ProductKey { get; set; }
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }

    }
}
