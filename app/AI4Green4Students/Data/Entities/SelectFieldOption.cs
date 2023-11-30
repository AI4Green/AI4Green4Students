namespace AI4Green4Students.Data.Entities;

/// <summary>
/// A list of selectable options 
/// </summary>
public class SelectFieldOption
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Field Field { get; set; } = new Field();
}
