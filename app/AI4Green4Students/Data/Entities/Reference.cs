namespace AI4Green4Students.Data.Entities;

public class Reference
{
  public int Id { get; set; }
  public int Order { get; set; }
  public string Content { get; set; } = string.Empty;
  public Experiment Experiment { get; set; } = null!;
}
