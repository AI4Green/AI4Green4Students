namespace AI4Green4Students.Data.Entities;

public class Note
{
  public int Id { get; set; }
  public int PlanId { get; set; }
  public Plan Plan { get; set; } = null!;
  public List<NoteFieldResponse> NoteFieldResponses { get; set; } = null!;
}
