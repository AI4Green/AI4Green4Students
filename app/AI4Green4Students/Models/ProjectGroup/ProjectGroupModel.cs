namespace AI4Green4Students.Models.ProjectGroup;

public class ProjectGroupModel
{
  public ProjectGroupModel(Data.Entities.SectionTypeData.ProjectGroup entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    Students = entity.Students.ConvertAll<ProjectGroupStudentModel>
      (y => new ProjectGroupStudentModel(y.Id, y.FullName, y.Email)).ToList();
    ProjectId = entity.Project.Id;
    ProjectName = entity.Project.Name;
  }
  
  public ProjectGroupModel()
  {
    
  }
  
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<ProjectGroupStudentModel>? Students { get; set; }
  public int ProjectId { get; set; }
  public string ProjectName { get; set; }
};
