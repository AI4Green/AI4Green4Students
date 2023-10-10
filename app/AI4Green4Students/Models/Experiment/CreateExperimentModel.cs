using AI4Green4Students.Data.Entities;

namespace AI4Green4Students.Models.Experiment;

public class CreateExperimentModel
{
  public int ProjectGroupId { get; set; }
  public int ExperimentTypeId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string LiteratureReviewDescription { get; set; } = string.Empty;
  public bool IsLiteratureReviewFilePresent { get; set; } // suggests file exists or not. does not necessarily mean file is included in the request.
  public IFormFile? LiteratureReviewFile { get; set; }
  public List<ReferenceModel> References { get; set; } = new();
  public string SafetyDataFromLiterature { get; set; } = string.Empty;
  public string ExperimentalProcedure { get; set; } = string.Empty;
  
  public Data.Entities.Experiment ToEntity()
    => new()
    {
      Title = Title,
      LiteratureReviewDescription = LiteratureReviewDescription
    };
  
  public Data.Entities.Experiment ToUpdateEntity(Data.Entities.Experiment entity)
    => new()
    {
      Id = entity.Id,
      ProjectGroup = entity.ProjectGroup,
      ExperimentType = entity.ExperimentType,
      Title = Title,
      LiteratureReviewDescription = LiteratureReviewDescription,
      SafetyDataFromLiterature = SafetyDataFromLiterature,
      ExperimentalProcedure = ExperimentalProcedure,
      Owner = entity.Owner
    };
}
