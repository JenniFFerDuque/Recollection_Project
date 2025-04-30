using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

[Table("recolections")]
public partial class Recolection
{
    [Key]
    [Column("recolection_id")]
    public int RecolectionId { get; set; }

    [Column("dateRecolection", TypeName = "datetime")]
    public DateTime DateRecolection { get; set; }

    [Column("materialType_id")]
    public int MaterialTypeId { get; set; }

    [Column("weight", TypeName = "decimal(18, 0)")]
    public decimal? Weight { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("MaterialTypeId")]
    [InverseProperty("Recolections")]
    public virtual MaterialType MaterialType { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Recolections")]
    public virtual User User { get; set; } = null!;
}
