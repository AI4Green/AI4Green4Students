using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI4Green4Students.Data.Entities.AI4Green;

[Table("HazardCode")]
public class HazardCode
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public string Code { get; set; } = null!;

    [Column("phrase")]
    public string Phrase { get; set; } = null!;

    [Column("category")]
    public string? Category { get; set; }
}
