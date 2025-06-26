namespace AI4Green4Students.Models.ProjectGroup;

using Data.Entities.SectionTypeData;

public record ProjectGroupModel(
  int Id,
  string Name,
  List<ProjectGroupStudentModel> Students,
  int ProjectId,
  string ProjectName,
  string? StartDate,
  string? PlanningDeadline,
  string? ExperimentDeadline
)
{
  public ProjectGroupModel(ProjectGroup entity) : this(
    entity.Id,
    entity.Name,
    entity.Students.Select(y => new ProjectGroupStudentModel(y.Id, y.FullName, y.Email)).ToList(),
    entity.Project.Id,
    entity.Project.Name,
    FormatDate(entity.StartDate),
    FormatDate(entity.PlanningDeadline),
    FormatDate(entity.ExperimentDeadline)
  )
  { }

  private static string? FormatDate(DateTimeOffset date)
    => date != DateTimeOffset.MaxValue ? date.ToString("yyyy-MM-dd") : null;
}
