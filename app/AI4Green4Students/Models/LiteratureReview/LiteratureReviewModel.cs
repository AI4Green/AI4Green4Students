namespace AI4Green4Students.Models.LiteratureReview;

using Data.Entities.SectionTypeData;
using SectionTypeData;

public record LiteratureReviewModel : BaseSectionTypeModel
{
  public string OwnerId { get; } = string.Empty;
  public string OwnerName { get; } = string.Empty;
  public List<string> Permissions { get; } = [];

  public LiteratureReviewModel(LiteratureReview entity, List<string> permissions)
    : base(
      entity.Id,
      null,
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
