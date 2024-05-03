namespace AI4Green4Students.Models.Report;

public class ReportModel
{
  public ReportModel(Data.Entities.SectionTypeData.Report entity)
  {
    Id = entity.Id;
    PlanId = entity.Plan.Id;
    Deadline = entity.Deadline;
    StageId = entity.Stage.Id;
    StageName = entity.Stage.DisplayName;
  }

  public int Id { get; set; }
  public int PlanId { get; set; } 
  public DateTimeOffset Deadline { get; set; }
  public int StageId { get; set; }
  public string StageName { get; set; } = string.Empty;
  public List<string> Permissions { get; set; } = new();
}
