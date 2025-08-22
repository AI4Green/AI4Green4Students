namespace AI4Green4Students.Models.ProjectType;

public record ProjectTypeModel(
  int Id,
  string Name,
  string Description
)
{
  public ProjectTypeModel(Data.Entities.ProjectType entity) : this(
    entity.Id,
    entity.Name,
    entity.Description
  )
  {
  }
}
