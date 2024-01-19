using Microsoft.Identity.Client;

namespace AI4Green4Students.Data.Entities;

/// <summary>
/// A section splits up the fields for an experiment. Each section can be approved by the instructor.
/// </summary>
public class Section
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Project Project { get; set; } = null!;
  public List<Field> Fields { get; set; } = null!;
  public SectionType SectionType { get; set; } = null!;
  public int SortOrder { get; set; } = 0;
}
