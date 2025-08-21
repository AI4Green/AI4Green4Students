using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Data.Entities.SectionTypeData;

namespace AI4Green4Students.Data.Entities;

public class Project
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public List<ProjectGroup> ProjectGroups { get; set; } = new  List<ProjectGroup>();
  public List<Plan> Plans { get; set; } = new List<Plan>();
  public List<Note> Notes { get; set; } = new List<Note>();
  public List<Report> Reports { get; set; } = new List<Report>();
  public List<ApplicationUser> Instructors { get; set; } = new List<ApplicationUser>();
  public required ProjectType ProjectType { get; set; }
}
