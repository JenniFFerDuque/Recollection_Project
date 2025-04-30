using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

[Table("materialTypes")]
public partial class MaterialType
{
    [Key]
    [Column("materialType_id")]
    public int MaterialTypeId { get; set; }

    [Column("materialName")]
    [StringLength(255)]
    public string MaterialName { get; set; } = null!;

    [Column("withWeight")]
    public bool WithWeight { get; set; }

    [InverseProperty("MaterialType")]
    public virtual ICollection<Recolection> Recolections { get; set; } = new List<Recolection>();
}
