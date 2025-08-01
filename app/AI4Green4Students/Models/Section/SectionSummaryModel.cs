using AI4Green4Students.Models.SectionType;

namespace AI4Green4Students.Models.Section;

public class SectionSummaryModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public bool Approved { get; set; }
  public int Comments { get; set; }
  public int SortOrder { get; set; }
  public SectionTypeModel SectionType { get; set; } = null!;
  public string Stage { get; set; } = string.Empty;
  public List<string> Permissions { get; set; } = new();
}
