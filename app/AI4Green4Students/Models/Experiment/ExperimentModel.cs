namespace AI4Green4Students.Models.Experiment;

public class ExperimentModel
{
  public ExperimentModel(Data.Entities.Experiment entity)
  {
    Id = entity.Id;
    Title = entity.Title;
    ProjectGroupId = entity.ProjectGroup.Id;
    ProjectGroupName = entity.ProjectGroup.Name;
    ProjectId = entity.ProjectGroup.Project.Id;
    ProjectName = entity.ProjectGroup.Project.Name;
    LiteratureReviewDescription = entity.LiteratureReviewDescription;
    LiteratureFileName = entity.LiteratureFileName;
    LiteratureFileLocation = entity.LiteratureFileLocation;
  }
  
  public ExperimentModel()
  {
    
  }
  
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public int ProjectGroupId { get; set; }
  public string ProjectGroupName { get; set; } = string.Empty;
  public int ProjectId { get; set; }
  public string ProjectName { get; set; } = string.Empty;
  public string LiteratureReviewDescription { get; set; } = string.Empty;
  public string LiteratureFileName { get; set; } = string.Empty;
  public string LiteratureFileLocation { get; set; } = string.Empty;
}
