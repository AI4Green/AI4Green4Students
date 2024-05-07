using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities.SectionTypeData;

public abstract class BaseSectionTypeData
{
  public int Id { get; set; }
  public List<FieldResponse> FieldResponses { get; set; } = new ();
}

public abstract class CoreSectionTypeData : BaseSectionTypeData
{
  public Project Project { get; set; } = null!;
  public ApplicationUser Owner { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; }
}
