namespace AI4Green4Students.Models.ProjectType;

public record ProjectTypeModel(
  int Id,
  string Name,
  string Description,
  string Stage,
  bool InUse,
  int ProjectCount
)
{
  public ProjectTypeModel(Data.Entities.ProjectType entity, bool inUse, int projectCount) : this(
    entity.Id,
    entity.Name,
    entity.Description,
    entity.Stage.DisplayName,
    inUse,
    projectCount
  )
  {
  }
}
