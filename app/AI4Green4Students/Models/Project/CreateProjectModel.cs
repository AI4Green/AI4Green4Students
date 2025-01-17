using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Models.Project;

public record CreateProjectModel
{
  public string Name { get; init; } = string.Empty;
  public List<string> InstructorIds { get; init; } = new List<string>();
}
