namespace AI4Green4Students.Models.LiteratureReview;

using Data.Entities.SectionTypeData;
using SectionTypeData;

public record LiteratureReviewModel : BaseSectionTypeModel
{
  public LiteratureReviewOwnerModel Owner { get; }
  public List<string> Permissions { get; } = [];

  public LiteratureReviewModel(LiteratureReview entity, List<string> permissions)
    : base(
      entity.Id,
      null,
      entity.Stage.DisplayName,
      entity.Project,
      entity.Deadline
    )
  {
    Owner = new LiteratureReviewOwnerModel(entity.Owner.Id, entity.Owner.FullName);
    Permissions = permissions;
  }
}

public record LiteratureReviewOwnerModel(string Id, string Name);
