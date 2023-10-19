using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

/// <summary>
/// A collection of comments by an instructor for a given FieldResponse.
/// </summary>
public class Conversation
{
  public int Id { get; set; }
  public List<Comment> Comments { get; set; } = new();
  public FieldResponse FieldResponse { get; set; } = null!;
  public ApplicationUser Instructor { get; set; } = null!;
  public bool Resolved { get; set; } = false;
}
