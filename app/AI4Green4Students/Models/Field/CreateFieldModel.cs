namespace AI4Green4Students.Models.Field;

public class CreateFieldModel
{
  public string Name { get; set; } = string.Empty;
  public int Section { get; set; }
  public string DefaultValue { get; set; } = string.Empty;
  public int InputType { get; set; }
  public bool Mandatory { get; set; } = true;
  public int SortOrder { get; set; }
  public bool Hidden { get; set; }
  public List<string> SelectFieldOptions { get; set; } = new List<string>();
  public string? TriggerCause { get; set; }
  public CreateFieldModel? TriggerTarget { get; set; }
}
