namespace AI4Green4Students.Models.Report;

using Data.Entities.SectionTypeData;
using SectionTypeData;

public record ReportModel : BaseSectionTypeModel
{
  public ReportOwnerModel Owner { get; }
  public List<string> Permissions { get; } = [];

  public ReportModel(Report entity, List<string> permissions)
    : base(
      entity.Id,
      entity.Title,
      entity.Stage.DisplayName,
      entity.Project,
      entity.Deadline
    )
  {
    Owner = new ReportOwnerModel(entity.Owner.Id, entity.Owner.FullName);
    Permissions = permissions;
  }
}

public record ReportOwnerModel(string Id, string Name);
