namespace AI4Green4Students.Models.InputType;

using Data.Entities;

public record InputTypeModel(int Id, string Name)
{
  public InputTypeModel(InputType entity) : this(entity.Id, entity.Name)
  {
  }
}
