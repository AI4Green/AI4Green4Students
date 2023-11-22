using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data.Entities.AI4Green;

[Table("Compound")]
[Index("Cas", Name = "Compound_cas_key", IsUnique = true)]
[Index("Cid", Name = "Compound_cid_key", IsUnique = true)]
[Index("Solvent", Name = "ix_Compound_solvent")]
public class Compound
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("cid")]
    public int? Cid { get; set; }

    [Column("cas")]
    public string Cas { get; set; } = null!;

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("smiles")]
    public string? Smiles { get; set; }

    [Column("inchi")]
    public string? Inchi { get; set; }

    [Column("inchikey")]
    public string? Inchikey { get; set; }

    [Column("molec_formula")]
    public string? MolecFormula { get; set; }

    [Column("density")]
    public double? Density { get; set; }

    [Column("concentration")]
    public double? Concentration { get; set; }

    [Column("boiling_point")]
    public double? BoilingPoint { get; set; }

    [Column("melting_point")]
    public double? MeltingPoint { get; set; }

    [Column("flash_point")]
    public double? FlashPoint { get; set; }

    [Column("autoignition_temp")]
    public double? AutoignitionTemp { get; set; }

    [Column("molec_weight")]
    public double? MolecWeight { get; set; }

    [Column("state")]
    public string? State { get; set; }

    [Column("form")]
    public string? Form { get; set; }

    [Column("hphrase")]
    public string? Hphrase { get; set; }

    [Column("safety_score")]
    public double? SafetyScore { get; set; }

    [Column("health_score")]
    public double? HealthScore { get; set; }

    [Column("enviro_score")]
    public double? EnviroScore { get; set; }

    [Column("econom_score")]
    public double? EconomScore { get; set; }

    [Column("solvent")]
    public string? Solvent { get; set; }

    [InverseProperty("CompoundNavigation")]
    public ICollection<CompoundDataErrorReport> CompoundDataErrorReports { get; set; } = new List<CompoundDataErrorReport>();

    [ForeignKey("Solvent")]
    [InverseProperty("Compounds")]
    public Solvent? SolventNavigation { get; set; }
}
