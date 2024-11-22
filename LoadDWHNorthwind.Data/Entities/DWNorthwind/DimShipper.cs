

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("DimShippers", Schema = "DWH")]
    public class DimShipper
    {
        [Key]
        public int ShipperKey { get; set; }
        public int ShipperID { get; set; }
        public string? ShipperName { get; set; }
    }
}

