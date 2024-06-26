namespace AI4Green4Students.Models.Field;

public class FieldModel
{
  public FieldModel(Data.Entities.Field entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    Section = entity.Section.Name;
    FieldType = entity.InputType.Name;

    TriggerValue = entity.TriggerCause;
    TriggerId = entity.TriggerTarget?.Id;

    SelectFieldOptions = entity.SelectFieldOptions.Count >= 1
      ? entity.SelectFieldOptions.Select(x => new SelectFieldOptionModel(x)).ToList()
      : null;
  }

  public int Id { get; set; }
  public string Name { get; set; }
  public string Section { get; set; } = string.Empty;
  public string FieldType { get; set; } = string.Empty;
  public string? TriggerValue { get; set; } = string.Empty;
  public int? TriggerId { get; set; }
  public List<SelectFieldOptionModel>? SelectFieldOptions { get; set; }
}
