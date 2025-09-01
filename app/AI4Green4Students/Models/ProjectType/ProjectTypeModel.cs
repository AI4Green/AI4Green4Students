namespace AI4Green4Students.Models.ProjectType;

public record ProjectTypeModel(
  int Id,
  string Name,
  string Description,
  string Stage,
  int InUseCount
)
{
  public ProjectTypeModel(Data.Entities.ProjectType entity, int inUseCount) : this(
    entity.Id,
    entity.Name,
    entity.Description,
    entity.Stage.DisplayName,
    inUseCount
  )
  {
  }
}
