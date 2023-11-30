namespace AI4Green4Students.Data.Entities;

/// <summary>
/// Input field. Has a type to define if its text input, numbers, multiple choice, reaction scheme.
/// </summary>
public class Field
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public InputType InputType { get; set; } = null!;
  public string? TriggerCause { get; set; }
  public Field? TriggerTarget { get; set; }
  public bool Mandatory { get; set; } = true;
  public Section Section { get; set; } = new();
  public List<FieldResponse> FieldResponses { get; set; } = new();
  public List<SelectFieldOption> SelectFieldOptions { get; set; } = new();
  public string DefaultResponse { get; set; } = string.Empty;
}
