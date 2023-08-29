using AI4Green4Students.Models.ProjectGroup;

namespace AI4Green4Students.Models.Project;

public record ProjectModel
{
  public ProjectModel(Data.Entities.Project entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    ProjectGroups = entity.ProjectGroups.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x)).ToList();
  }

  public ProjectModel()
  {
    
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ProjectGroupModel> ProjectGroups { get; set; } = new();
};
