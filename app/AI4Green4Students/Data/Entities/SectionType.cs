namespace AI4Green4Students.Data.Entities;

public class SectionType
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<Section> Sections { get; set; } = null!;
}
