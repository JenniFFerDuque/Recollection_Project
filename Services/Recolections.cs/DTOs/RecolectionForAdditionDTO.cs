using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Recolections.cs.DTOs
{
    public class RecolectionForAdditionDTO
    {
        public DateTime? DateRecolection { get; set; }
        public int MaterialTypeId { get; set; }
        public decimal? Weight { get; set; }
        public int UserId { get; set; }
    }
}
