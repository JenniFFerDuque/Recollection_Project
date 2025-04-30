using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Recolections.cs.DTOs
{
    public class RecolectionUpdateDTO
    {
        public int RecolectionId { get; set; }
        public DateTime? DateRecolection { get; set; }
        public int MaterialTypeId { get; set; }
        public decimal? Weight { get; set; }
        public int UserId { get; set; }
    }
}
