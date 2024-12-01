using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadDWHNorthwind.Data.Entities.DWNorthwind
{
    [Table("DimDates", Schema = "DWH")]

    public class DimDate
    {
        [Key]
        public int DateKey { get; set; }

        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Quarter { get; set; }
        public string? MonthName { get; set; }
        public string? DayName { get; set; }
    }
}
