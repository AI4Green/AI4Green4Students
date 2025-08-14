namespace AI4Green4Students.Data.Entities;

public class SectionType
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public List<Section> Sections { get; set; } = new List<Section>();
}
