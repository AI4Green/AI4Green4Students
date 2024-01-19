using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

public class Experiment
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public ProjectGroup ProjectGroup { get; set; } = null!;
  public List<ExperimentReaction> ExperimentReactions { get; set; } = new();
  public ApplicationUser Owner { get; set; } = null!;
  public List<FieldResponse> FieldResponses { get; set; } = new();
}
