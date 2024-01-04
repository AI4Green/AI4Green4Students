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
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
    Reactions = entity.ExperimentReactions.ConvertAll<ExperimentReactionModel>
      (y => new ExperimentReactionModel(y)).ToList();
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
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public List<ExperimentReactionModel> Reactions { get; set; } = new();
}
