namespace AI4Green4Students.Models.Project;

public record ProjectModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ProjectModel> Projects { get; set; } = new();
};
