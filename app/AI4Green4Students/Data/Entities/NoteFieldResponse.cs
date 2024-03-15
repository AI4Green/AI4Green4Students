namespace AI4Green4Students.Data.Entities;

public class NoteFieldResponse
{
  public int NoteId { get; set; }
  public Note Note { get; set; } = null!;
  public int FieldResponseId { get; set; }
  public FieldResponse FieldResponse { get; set; } = null!;
}
