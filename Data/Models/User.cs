using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

[Table("users")]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Recolection> Recolections { get; set; } = new List<Recolection>();
}
