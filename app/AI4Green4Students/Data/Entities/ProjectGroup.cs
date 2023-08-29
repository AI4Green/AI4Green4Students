using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

public class ProjectGroup
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ApplicationUser> Students { get; set; } = new ();
  public Project Project { get; set; } = new ();
}
