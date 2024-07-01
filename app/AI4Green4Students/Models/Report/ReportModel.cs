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
    Stage = entity.Stage.DisplayName;
    ProjectId = entity.Project.Id;
  }

  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public DateTimeOffset Deadline { get; set; }
  public string Stage { get; set; } = string.Empty;
  public List<string> Permissions { get; set; } = new();
  public int ProjectId { get; set; }
}
