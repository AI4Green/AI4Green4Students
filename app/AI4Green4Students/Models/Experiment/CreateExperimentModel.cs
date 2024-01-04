namespace AI4Green4Students.Models.Experiment;

public class CreateExperimentModel
{
  public int ProjectGroupId { get; set; }
  public int ExperimentTypeId { get; set; }
  public string Title { get; set; } = string.Empty;
}
