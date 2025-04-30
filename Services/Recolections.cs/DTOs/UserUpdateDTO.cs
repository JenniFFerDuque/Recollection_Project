using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Recolections.cs.DTOs
{
     public class UserUpdateDTO
    {
        public int UserId { get; set; }
        public String? Email { get; set; }
        public String? Password { get; set; }
    }
}
