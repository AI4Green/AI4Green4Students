namespace AI4Green4Students.Data.Entities;

/// <summary>
/// A list of selectable options 
/// </summary>
public class SelectFieldOption
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public Field Field { get; set; } = null!;
}
