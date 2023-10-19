namespace AI4Green4Students.Data.Entities;

/// <summary>
/// The type of input which a field is e.g. Text, ReactionScheme, Multiple Choice
/// </summary>
public class InputType
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
}
