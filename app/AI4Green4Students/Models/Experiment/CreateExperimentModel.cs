namespace AI4Green4Students.Models.Experiment;

public class CreateExperimentModel
{
  public int ProjectGroupId { get; set; }
  public int ExperimentTypeId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string LiteratureReviewDescription { get; set; } = string.Empty;
  public IFormFile? LiteratureReviewFile { get; set; }

  public Data.Entities.Experiment ToEntity()
    => new()
    {
      Title = Title,
      LiteratureReviewDescription = LiteratureReviewDescription,
    };
}
