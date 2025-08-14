namespace AI4Green4Students.Data.Entities;

/// <summary>
/// Represents a section within a section type (e.g. Plan or Lab-Note).
/// </summary>
public class Section
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public ProjectType? ProjectType { get; set; }
  public int SortOrder { get; set; }
  public List<Field> Fields { get; set; } = new List<Field>();
  public SectionType SectionType { get; set; } = null!;
}
