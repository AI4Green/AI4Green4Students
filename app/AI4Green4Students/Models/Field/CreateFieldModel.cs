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

public record CreateSectionFieldModel(
  int? Id,
  int InputType,
  bool Mandatory,
  string Name,
  string DefaultValue,
  int SortOrder,
  bool Hidden,
  List<CreateSelectFieldOptionModel> SelectFieldOptions,
  string? TriggerValue = null,
  CreateSectionFieldModel? TriggerField = null
);

public record CreateSelectFieldOptionModel(int? Id, string Name);
