namespace AI4Green4Students.Data.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a type of stage. E.g., 'Note' or 'Project'.
/// </summary>
public class StageType
{
  [Key]
  public int Id { get; set; }

  public string Value { get; set; } = string.Empty;
}
