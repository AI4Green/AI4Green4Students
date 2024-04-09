using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

public class Plan
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public Project Project { get; set; } = null!;
  public ApplicationUser Owner { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; }
  public Note Note { get; set; } = null!;
  public List<PlanFieldResponse> PlanFieldResponses { get; set; } = null!;
}
