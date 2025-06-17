namespace AI4Green4Students.Data.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a stage in the sequence of stages.
/// </summary>
public class Stage
{
  public int Id { get; set; }
  public string Value { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;

  /// <summary>
  /// Represents the order of the stage in the sequence of stages.
  /// </summary>
  [Required]
  public int SortOrder { get; set; }

  /// <summary>
  /// Represents the type of stage. E.g., 'Note' or 'Project'.
  /// </summary>
  [Required]
  public StageType Type { get; set; }
  public int TypeId { get; set; }

  /// <summary>
  /// Represents the next stage in the sequence of stages.
  /// </summary>
  public Stage? NextStage { get; set; }
}
