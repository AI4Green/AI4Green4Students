namespace AI4Green4Students.Data.Entities;

public class PlanFieldResponse
{
  public int PlanId { get; set; }
  public Plan Plan { get; set; } = null!;
  public int FieldResponseId { get; set; }
  public FieldResponse FieldResponse { get; set; } = null!;
}
