namespace AI4Green4Students.Data.Entities;

public class ReportFieldResponse
{
  public int ReportId { get; set; }
  public Report Report { get; set; } = null!;
  public int FieldResponseId { get; set; }
  public FieldResponse FieldResponse { get; set; } = null!;
}
