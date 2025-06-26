namespace AI4Green4Students.Models.Report;

using Data.Entities.SectionTypeData;
using SectionTypeData;

public record ReportModel : BaseSectionTypeModel
{
  public string OwnerId { get; } = string.Empty;
  public string OwnerName { get; } = string.Empty;
  public List<string> Permissions { get; } = [];

  public ReportModel(Report entity, List<string> permissions)
    : base(
      entity.Id,
      entity.Title,
      entity.Stage.DisplayName,
      entity.Project.Id,
      entity.Project.Name,
      entity.Deadline
    )
  {
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
    Permissions = permissions;
  }
}