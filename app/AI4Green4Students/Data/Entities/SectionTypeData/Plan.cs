using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities.SectionTypeData;

public class Plan : BaseSectionTypeData
{
  public string Title { get; set; } = string.Empty;
  public Project Project { get; set; } = null!;
  public ApplicationUser Owner { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; }
  public Note Note { get; set; } = null!;
}
