namespace AI4Green4Students.Models.Field;

public class FieldModel
{
  public FieldModel(Data.Entities.Field entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    Section = entity.Section.Name;
    InputType = entity.InputType.Name;

    if (entity.TriggerCause != null && entity.TriggerTarget != null)
    {
      TriggerValue = entity.TriggerCause;
      TriggerId = entity.TriggerTarget.Id;
    }

    foreach(var so in entity.SelectFieldOptions) 
    {
      SelectFieldOptions.Add(so.Name);
    }
  }

  public int Id { get; set; }
  public string Name { get; set; }
  public string Section { get; set; } = string.Empty;
  public string InputType { get; set; } = string.Empty;
  public string TriggerValue { get; set; } = string.Empty;
  public int TriggerId { get; set; } = 0;
  public List<string> SelectFieldOptions = new List<string>();
  
}
