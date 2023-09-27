namespace AI4Green4Students.Models.Experiment;

public class CreateExperimentModel
{
  public int ProjectGroupId { get; set; }
  public int ExperimentTypeId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string LiteratureReviewDescription { get; set; } = string.Empty;
  public bool IsLiteratureReviewFilePresent { get; set; } // suggests file exists or not. does not necessarily mean file is included in the request.
  public IFormFile? LiteratureReviewFile { get; set; }

  public Data.Entities.Experiment ToEntity()
    => new()
    {
      Title = Title,
      LiteratureReviewDescription = LiteratureReviewDescription
    };
}
