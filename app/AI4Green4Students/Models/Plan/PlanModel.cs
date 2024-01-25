namespace AI4Green4Students.Models.Plan;

public class PlanModel
{
  public PlanModel(Data.Entities.Plan entity)
  {
    Id = entity.Id;
    Deadline = entity.Deadline;
    ProjectId = entity.ProjectGroup.Project.Id;
    ProjectName = entity.ProjectGroup.Project.Name;
    ProjectGroupId = entity.ProjectGroup.Id;
    ProjectGroupName = entity.ProjectGroup.Name;
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
    Stage = entity.Stage.DisplayName;
  }

  public int Id { get; set; }

  public int ProjectId { get; set; }
  public string ProjectName { get; set; }
  public int ProjectGroupId { get; set; }
  public string ProjectGroupName { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public DateTimeOffset Deadline { get; set; }
  public string Stage { get; set; }
  public List<string> Permissions { get; set; } = new();
}

