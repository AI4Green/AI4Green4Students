using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI4Green4Students.Data.Entities.AI4Green;

[Table("Element")]
public class Element
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("symbol")]
    public string Symbol { get; set; } = null!;

    [Column("remaining_supply")]
    public string RemainingSupply { get; set; } = null!;

    [Column("colour")]
    public string Colour { get; set; } = null!;
}
