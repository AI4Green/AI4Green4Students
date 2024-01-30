using AI4Green4Students.Models.Plan;
namespace AI4Green4Students.Models.Project;

public record ProjectSummaryModel
{
  public int ProjectId { get; init; }
  public string ProjectName { get; init; } = string.Empty;
  public int ProjectGroupId { get; init; }
  public string ProjectGroupName { get; init; } = string.Empty;
  public List<PlanModel> Plans { get; init; } = new();
}
