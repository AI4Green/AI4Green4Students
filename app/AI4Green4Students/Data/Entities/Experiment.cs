using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

public class Experiment
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public ProjectGroup ProjectGroup { get; set; } = null!;
  public ExperimentType ExperimentType { get; set; } = null!;
  public string LiteratureReviewDescription { get; set; } = string.Empty;
  public string LiteratureFileName { get; set; } = string.Empty;
  public string LiteratureFileLocation { get; set; } = string.Empty;
  public ApplicationUser Owner { get; set; } = null!;
}
