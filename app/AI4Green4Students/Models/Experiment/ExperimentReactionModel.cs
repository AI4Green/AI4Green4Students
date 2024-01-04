namespace AI4Green4Students.Models.Experiment;

public class ExperimentReactionModel
{
  public ExperimentReactionModel(Data.Entities.ExperimentReaction entity)
  {
    Id = entity.Id;
    Title = entity.Title;
    ExperimentId = entity.Experiment.Id;
    ExperimentTitle = entity.Experiment.Title;
    ProjectGroupId = entity.Experiment.ProjectGroup.Id;
    ProjectGroupName = entity.Experiment.ProjectGroup.Name;
    ProjectId = entity.Experiment.ProjectGroup.Project.Id;
    ProjectName = entity.Experiment.ProjectGroup.Project.Name;
    OwnerId = entity.Experiment.Owner.Id;
    OwnerName = entity.Experiment.Owner.FullName;
  }

  public ExperimentReactionModel()
  {
  }

  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public int ExperimentId { get; set; }
  public string ExperimentTitle { get; set; } = string.Empty;
  public int ProjectGroupId { get; set; }
  public string ProjectGroupName { get; set; } = string.Empty;
  public int ProjectId { get; set; }
  public string ProjectName { get; set; } = string.Empty;
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
}
