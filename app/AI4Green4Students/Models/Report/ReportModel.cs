namespace AI4Green4Students.Models.Report;

public class ReportModel
{
  public ReportModel(Data.Entities.SectionTypeData.Report entity)
  {
    Id = entity.Id;
    Title = entity.Title;
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
    Deadline = entity.Deadline;
    StageId = entity.Stage.Id;
    StageName = entity.Stage.DisplayName;
  }

  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public DateTimeOffset Deadline { get; set; }
  public int StageId { get; set; }
  public string StageName { get; set; } = string.Empty;
  public List<string> Permissions { get; set; } = new();
}
