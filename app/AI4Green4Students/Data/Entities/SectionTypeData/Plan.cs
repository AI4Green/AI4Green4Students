namespace AI4Green4Students.Data.Entities.SectionTypeData;

public class Plan : CoreSectionTypeData
{
  public string Title { get; set; } = string.Empty;
  public Note Note { get; set; } = null!;
}
