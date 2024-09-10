using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Data.Entities.SectionTypeData;

namespace AI4Green4Students.Data.Entities;

public class Project
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ProjectGroup> ProjectGroups { get; set; } = null!;
  public List<Plan> Plans { get; set; } = null!;
  public List<Note> Notes { get; set; } = null!;
  public List<Report> Reports { get; set; } = null!;
  public List<Section> Sections { get; set; } = null!;
  public List<ApplicationUser> Instructors { get; set; } = new();
}
