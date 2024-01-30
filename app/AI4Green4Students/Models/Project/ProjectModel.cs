using AI4Green4Students.Models.ProjectGroup;
using AI4Green4Students.Models.SectionType;

namespace AI4Green4Students.Models.Project;

public class ProjectModel
{
  public ProjectModel(Data.Entities.Project entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    ProjectGroups = entity.ProjectGroups.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x)).ToList();
    SectionTypes = new ProjectSectionTypeModel(entity.Sections.ConvertAll<SectionTypeModel>(x => new SectionTypeModel(x.SectionType)));
    StartDate = FormatDate(entity.StartDate);
    PlanningDeadline = FormatDate(entity.PlanningDeadline);
    ExperimentDeadline = FormatDate(entity.ExperimentDeadline);
  }
  
  /// <summary>
  /// During the project creation, if no/invalid date is given, the date is set to DateTimeOffset.MaxValue.
  /// Checks if the date is valid i.e. not equal to DateTimeOffset.MaxValue.
  /// </summary>
  /// <param name="date"></param>
  /// <returns>Date in string format if valid, else null</returns>
  private string? FormatDate(DateTimeOffset date) 
    => date != DateTimeOffset.MaxValue ? date.ToString("yyyy-MM-dd") : null;
  
  public ProjectModel()
  {
    
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? StartDate { get; set; }
  public string? PlanningDeadline { get; set; }
  public string? ExperimentDeadline { get; set; }
  public List<ProjectGroupModel> ProjectGroups { get; set; } = new();
  public ProjectSectionTypeModel SectionTypes { get; set; } = null!;
};
