using AI4Green4Students.Models.ProjectGroup;

namespace AI4Green4Students.Models.Project;

public class ProjectModel
{
  public ProjectModel(Data.Entities.Project entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    ProjectGroups = entity.ProjectGroups?.Select(x => new ProjectGroupModel { Id = x.Id, Name = x.Name }).ToList() ??
                    new List<ProjectGroupModel>();
  }
  
  public ProjectModel()
  {
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Stage { get; set; } = string.Empty; // More of a status than a stage for now.
  public List<ProjectGroupModel> ProjectGroups { get; set; } = new();
};
