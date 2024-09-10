namespace AI4Green4Students.Models.ProjectGroup;

public class ProjectGroupModel
{
  public ProjectGroupModel(Data.Entities.SectionTypeData.ProjectGroup entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    Students = entity.Students.ConvertAll<ProjectGroupStudentModel>
      (y => new ProjectGroupStudentModel(y.Id, y.FullName, y.Email)).ToList();
    ProjectId = entity.Project.Id;
    ProjectName = entity.Project.Name;
    StartDate = FormatDate(entity.StartDate);
    PlanningDeadline = FormatDate(entity.PlanningDeadline);
    ExperimentDeadline = FormatDate(entity.ExperimentDeadline);
  }
  
  public ProjectGroupModel()
  {
  }
  
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ProjectGroupStudentModel>? Students { get; set; }
  public int ProjectId { get; set; }
  public string ProjectName { get; set; }
  public string? StartDate { get; set; }
  public string? PlanningDeadline { get; set; }
  public string? ExperimentDeadline { get; set; }
  
  /// <summary>
  /// During the project group creation, if no/invalid date is given, the date is set to DateTimeOffset.MaxValue.
  /// Checks if the date is valid i.e. not equal to DateTimeOffset.MaxValue.
  /// </summary>
  /// <param name="date"></param>
  /// <returns>Date in string format if valid, else null</returns>
  private string? FormatDate(DateTimeOffset date) 
    => date != DateTimeOffset.MaxValue ? date.ToString("yyyy-MM-dd") : null;
};


