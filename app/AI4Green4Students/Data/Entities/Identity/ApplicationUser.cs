using AI4Green4Students.Data.Entities.SectionTypeData;
using Microsoft.AspNetCore.Identity;

namespace AI4Green4Students.Data.Entities.Identity;

public class ApplicationUser : IdentityUser
{
  [PersonalData]
  public string FullName { get; set; } = string.Empty;

  [PersonalData]
  public string UICulture { get; set; } = string.Empty;
  
  public List<Project> Projects { get; set; } = new ();
  public List<ProjectGroup> ProjectGroups { get; set; } = new ();
}
