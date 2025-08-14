namespace AI4Green4Students.Data.Entities;

/// <summary>
/// Input field. Has a type to define if its text input, numbers, multiple choice, reaction scheme.
/// </summary>
public class Field
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public int SortOrder { get; set; }
  public required InputType InputType { get; set; }
  public string? TriggerCause { get; set; }
  public Field? TriggerTarget { get; set; }
  public bool Mandatory { get; set; } = true;
  public required Section Section { get; set; }
  public bool Hidden { get; set; }
  public List<FieldResponse> FieldResponses { get; set; } = new List<FieldResponse>();
  public List<SelectFieldOption> SelectFieldOptions { get; set; } = new List<SelectFieldOption>();
  public string DefaultResponse { get; set; } = string.Empty;
}
