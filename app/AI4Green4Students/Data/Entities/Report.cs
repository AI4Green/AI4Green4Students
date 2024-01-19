namespace AI4Green4Students.Data.Entities;

public class Report
{
  public int Id { get; set; }
  public Plan Plan { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; } = null!;
  public List<ReportFieldResponse> ReportFieldResponses { get; set; } = null!;
}
