namespace AI4Green4Students.Data.Entities;

/// <summary>
/// Represents a stage permission with stage sort order range.
/// </summary>
public class StagePermission
{
  public int Id { get; set; }

  /// <summary>
  /// Represents the minimum stage sort order.
  /// </summary>
  public int MinStageSortOrder { get; set; }

  /// <summary>
  /// Represents the maximum stage sort order.
  /// </summary>
  public int MaxStageSortOrder { get; set; }

  /// <summary>
  /// Represents the key of the permission.
  /// </summary>
  public string Key { get; set; } = string.Empty;

  /// <summary>
  /// Represents the type of stage. E.g., 'Note' or 'Project'.
  /// </summary>
  public StageType Type { get; set; } = null!;
}
