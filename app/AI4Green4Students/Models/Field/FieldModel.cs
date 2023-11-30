namespace AI4Green4Students.Models.Field;

public class FieldModel
{
  public FieldModel(Data.Entities.Field entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    Section = entity.Section.Name;
    InputType = entity.InputType.Name;
  }

  public int Id { get; set; }
  public string Name { get; set; }
  public string Section { get; set; } = string.Empty;
  public string InputType { get; set; } = string.Empty;
}
