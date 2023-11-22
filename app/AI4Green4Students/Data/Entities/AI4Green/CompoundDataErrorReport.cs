using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Data.Entities.AI4Green;

[Table("CompoundDataErrorReport")]
[Index("Compound", Name = "ix_CompoundDataErrorReport_compound")]
public class CompoundDataErrorReport
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("compound_name")]
    public string CompoundName { get; set; } = null!;

    [Column("compound")]
    public int Compound { get; set; }

    [Column("error_type")]
    public string ErrorType { get; set; } = null!;

    [Column("additional_info")]
    public string AdditionalInfo { get; set; } = null!;

    [Column("time", TypeName = "timestamp without time zone")]
    public DateTime Time { get; set; }

    [ForeignKey("Compound")]
    [InverseProperty("CompoundDataErrorReports")]
    public Compound CompoundNavigation { get; set; } = null!;
}
