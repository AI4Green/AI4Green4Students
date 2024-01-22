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
  }

  public int Id { get; set; }

  // TODO: to be updated with the relevant properties
  public int ProjectId { get; set; }
  public string ProjectName { get; set; }
  public int ProjectGroupId { get; set; }
  public string ProjectGroupName { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public DateTimeOffset Deadline { get; set; }
}
