namespace AI4Green4Students.Models.InputType;

public class InputTypeModel
{
  public InputTypeModel(Data.Entities.InputType entity)
  {
    Id = entity.Id;
    Name = entity.Name;
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
}
