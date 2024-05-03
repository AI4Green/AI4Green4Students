using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities.SectionTypeData;

public class LiteratureReview : BaseSectionTypeData
{
  public Project Project { get; set; } = null!;
  public ApplicationUser Owner { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; }
}
