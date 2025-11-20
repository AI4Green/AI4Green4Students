namespace AI4Green4Students.Models.Project;

using System.ComponentModel.DataAnnotations;

public record ProjectModel(
  int Id,
  string Name,
  string Stage,
  List<ProjectGroupModel> ProjectGroups,
  ProjectTypeModel ProjectType
)
{
  public ProjectModel(Data.Entities.Project entity) : this(
    entity.Id,
    entity.Name,
    string.Empty,
    entity.ProjectGroups.Select(x => new ProjectGroupModel(x.Id, x.Name)).ToList(),
    new ProjectTypeModel(entity.ProjectType.Id, entity.ProjectType.Name, entity.ProjectType.Description)
  )
  {
  }
}

public record ProjectGroupModel(int Id, string Name);
public record ProjectTypeModel(int Id, string Name, string Description);
public record InviteModel([Required] List<string> Emails);
public record RemoveModel([Required] string Id);
