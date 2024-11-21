

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("DimShippers")]
    public class DimShipper
    {
        [Key]
        public int ShipperKey { get; set; }
        public string? ShipperName { get; set; }
    }
}

