namespace AI4Green4Students.Data.Entities.SectionTypeData;

public class Note : CoreSectionTypeData
{
  public int PlanId { get; set; }
  public Plan Plan { get; set; } = null!;

  public bool FeedbackRequested { get; set; } = false;
}
