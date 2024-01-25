using AI4Green4Students.Models.ProjectGroup;
using AI4Green4Students.Models.SectionType;

namespace AI4Green4Students.Models.Project;

public class ProjectModel
{
  public ProjectModel(Data.Entities.Project entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    ProjectGroups = entity.ProjectGroups.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x)).ToList();
    SectionTypes = new ProjectSectionTypeModel(entity.Sections.ConvertAll<SectionTypeModel>(x => new SectionTypeModel(x.SectionType)));
  }

  public ProjectModel()
  {
    
  }

  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ProjectGroupModel> ProjectGroups { get; set; } = new();
  public ProjectSectionTypeModel SectionTypes { get; set; } = null!;
};
