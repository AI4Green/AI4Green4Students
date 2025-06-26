namespace AI4Green4Students.Models.SectionTypeData;

public abstract record BaseSectionTypeModel(
  int Id,
  string? Title,
  string Stage,
  int ProjectId,
  string ProjectName,
  DateTimeOffset? Deadline
);
