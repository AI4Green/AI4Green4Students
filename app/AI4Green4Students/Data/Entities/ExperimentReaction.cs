namespace AI4Green4Students.Data.Entities;

public class ExperimentReaction
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public Experiment Experiment { get; set; } = null!;
}
