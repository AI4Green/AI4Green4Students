using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI4Green4Students.Data.Entities.AI4Green;

[Table("Solvent")]
public class Solvent
{
    [Key]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("flag")]
    public int? Flag { get; set; }

    [Column("hazard")]
    public string? Hazard { get; set; }

    [Column("time_of_creation", TypeName = "timestamp without time zone")]
    public DateTime? TimeOfCreation { get; set; }

    [InverseProperty("SolventNavigation")]
    public ICollection<Compound> Compounds { get; set; } = new List<Compound>();
}
