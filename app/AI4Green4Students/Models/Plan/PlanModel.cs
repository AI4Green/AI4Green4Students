namespace AI4Green4Students.Models.Plan;

using Data.Entities.SectionTypeData;
using SectionTypeData;

public record PlanModel : BaseSectionTypeModel
{
  public PlanOwnerModel Owner { get; }
  public PlanNoteModel Note { get; }
  public List<string> Permissions { get; } = [];

  public PlanModel(Plan entity, List<string> permissions, PlanNoteModel note)
    : base(
      entity.Id,
      entity.Title,
      entity.Stage.DisplayName,
      entity.Project,
      entity.Deadline
    )
  {
    Owner = new PlanOwnerModel(entity.Owner.Id, entity.Owner.FullName);
    Permissions = permissions;
    Note = note;
  }
}

public record PlanOwnerModel(string Id, string Name);
public record PlanNoteModel(int Id, string Stage, List<string> Permissions);
