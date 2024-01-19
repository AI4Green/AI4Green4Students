namespace AI4Green4Students.Data.Entities;

public class ProjectGroupFieldResponse
{
  public int ProjectGroupId { get; set; }
  public ProjectGroup ProjectGroup { get; set; } = null!;
  public int FieldResponseId { get; set; }
  public FieldResponse FieldResponse { get; set; } = null!;
}
