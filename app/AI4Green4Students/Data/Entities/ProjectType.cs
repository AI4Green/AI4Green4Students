namespace AI4Green4Students.Data.Entities;

public class ProjectType
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public string Description { get; set; } = string.Empty;
  public required Stage Stage { get; set; }
  public List<Section> Sections { get; set; } = new List<Section>();
}
