namespace AI4Green4Students.Models.Section;

public class SectionModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public bool Approved { get; set; }
  public int Comments { get; set; }
}
