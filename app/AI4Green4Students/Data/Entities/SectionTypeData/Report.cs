namespace AI4Green4Students.Data.Entities.SectionTypeData;

public class Report : BaseSectionTypeData
{
  public Plan Plan { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; } = null!;
}
