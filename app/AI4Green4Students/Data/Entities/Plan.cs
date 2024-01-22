using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

public class Plan
{
  public int Id { get; set; }
  public ProjectGroup ProjectGroup { get; set; } = null!;
  public ApplicationUser Owner { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; }
  public List<PlanFieldResponse> PlanFieldResponses { get; set; } = null!;
}
