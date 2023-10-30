using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

/// <summary>
/// The response to a particular field by a certain student. Has a list of response values to track the changes made over time
/// </summary>
public class FieldResponse
{
  public int Id { get; set; }
  public Field Field { get; set; } = null!;
  public int FieldId { get; set; }
  public List<FieldResponseValue> FieldResponseValues { get; set; } = new();
  public Conversation Conversation { get; set; } = new();
  public ApplicationUser Student { get; set; } = null!;
}
