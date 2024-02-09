using AI4Green4Students.Models.Field;

namespace AI4Green4Students.Models.Section;

public class SectionFormModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<FieldResponseFormModel> FieldResponses { get; set; } = null!;
}

public class FieldResponseFormModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public bool Mandatory { get; set; }
  public bool Hidden { get; set; }
  public int SortOrder { get; set; }
  public string FieldType { get; set; } = string.Empty;
  public string DefaultResponse { get; set; } = string.Empty;
  public string? FieldResponse { get; set; }
  public List<SelectFieldOptionModel>? SelectFieldOptions { get; set; } = null!;
  public int Comments { get; set; }
  public TriggerFormModel? Trigger { get; set; }
}

public class TriggerFormModel
{
  public string Value { get; set; } = string.Empty;
  public int Target { get; set; }
}
