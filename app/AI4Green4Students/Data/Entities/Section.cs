namespace AI4Green4Students.Data.Entities;

/// <summary>
/// A section splits up the fields for an experiment. Each section can be approved by the instructor.
/// </summary>
public class Section
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Experiment Experiment { get; set; } = null!;
  public bool Approved { get; set; } = false;
  public List<Field> Fields { get; set; } = null!;
}
