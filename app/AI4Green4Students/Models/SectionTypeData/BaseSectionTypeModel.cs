namespace AI4Green4Students.Models.SectionTypeData;

using Data.Entities;

public abstract record BaseSectionTypeModel(
  int Id,
  string? Title,
  string Stage,
  BaseProjectModel Project,
  DateTimeOffset? Deadline
)
{
  protected BaseSectionTypeModel(
    int id,
    string? title,
    string stage,
    Project project,
    DateTimeOffset? deadline
  )
    : this(id, title, stage, new BaseProjectModel(project.Id, project.Name), deadline)
  {
  }
}

public record BaseProjectModel(int Id, string Name);
