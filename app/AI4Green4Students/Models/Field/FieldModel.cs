namespace AI4Green4Students.Models.Field;

public record FieldModel(
  int Id,
  string Name,
  bool Mandatory,
  bool Hidden,
  int SortOrder,
  string DefaultResponse,
  FieldSectionModel Section,
  FieldInputTypeModel InputType,
  TriggerFieldModel? TriggerField,
  List<SelectFieldOptionModel>? SelectFieldOptions
)
{
  public FieldModel(Data.Entities.Field entity) : this(
    entity.Id,
    entity.Name,
    entity.Mandatory,
    entity.Hidden,
    entity.SortOrder,
    entity.DefaultResponse,
    new FieldSectionModel(entity.Section.Id, entity.Section.Name),
    new FieldInputTypeModel(entity.InputType.Id, entity.InputType.Name),
    entity.TriggerCause is not null && entity.TriggerTarget is not null
      ? new TriggerFieldModel(entity.TriggerTarget.Id, entity.TriggerCause!)
      : null,
    entity.SelectFieldOptions.Count >= 1
      ? entity.SelectFieldOptions.Select(x => new SelectFieldOptionModel(x.Id, x.Name)).ToList()
      : null
  )
  {
  }
}

public record SelectFieldOptionModel(int Id, string Name);
public record FieldInputTypeModel(int Id, string Name);
public record FieldSectionModel(int Id, string Name);
public record TriggerFieldModel(int Id, string Value);
