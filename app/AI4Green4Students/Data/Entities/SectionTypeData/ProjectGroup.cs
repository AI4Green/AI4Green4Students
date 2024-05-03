using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities.SectionTypeData;

public class ProjectGroup : BaseSectionTypeData
{
  public string Name { get; set; } = string.Empty;
  public List<ApplicationUser> Students { get; set; } = new ();
  public Project Project { get; set; } = null!;
}
