namespace AI4Green4Students.Models.Plan;

using Data.Entities.SectionTypeData;
using SectionTypeData;

public record PlanModel : BaseSectionTypeModel
{
  public PlanNoteModel Note { get; }
  public string OwnerId { get; } = string.Empty;
  public string OwnerName { get; } = string.Empty;
  public List<string> Permissions { get; } = [];

  public PlanModel(Plan entity, List<string> permissions, PlanNoteModel note)
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
    Note = note;
  }
}

public record PlanNoteModel(
  int Id,
  string Stage,
  List<string> Permissions
);
