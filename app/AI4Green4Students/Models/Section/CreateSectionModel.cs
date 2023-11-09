namespace AI4Green4Students.Models.Section;

public class CreateSectionModel
{
  public string Name { get; set; } = string.Empty;
  public int ProjectId { get; set; }
  public int SortOrder { get; set; }
}
