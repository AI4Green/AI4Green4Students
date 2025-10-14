namespace AI4Green4Students.Models.ProjectType;

using Constants;

public record ProjectTypeModel(
  int Id,
  string Name,
  string Description,
  string Stage,
  int InUseCount,
  List<string> Permissions
)
{
  public ProjectTypeModel(Data.Entities.ProjectType entity, int inUseCount, List<string> permissions) : this(
    entity.Id,
    entity.Name,
    entity.Description,
    entity.Stage.DisplayName,
    inUseCount,
    inUseCount == 0 && entity.Stage.DisplayName == Stages.Ready
      ? new List<string>(permissions) { StagePermissions.CanPutInDraft }
      : permissions
  )
  {
  }
}
