namespace AI4Green4Students.Data.Entities;

public class Project
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ProjectGroup> ProjectGroups { get; set; } = new();
  public List<Section> Sections { get; set; } = new();
  public DateTimeOffset StartDate { get; set; }
  public DateTimeOffset PlanningDeadline { get; set; }
  public DateTimeOffset ExperimentDeadline { get; set; }
}
