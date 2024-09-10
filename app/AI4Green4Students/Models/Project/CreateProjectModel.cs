using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Models.Project;

public record CreateProjectModel
{
  public string Name { get; init; } = string.Empty;
  public List<ApplicationUser> Instructors { get; init; } = new List<ApplicationUser>();
}
