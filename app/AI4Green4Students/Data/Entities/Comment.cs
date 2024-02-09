using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

/// <summary>
/// A specific datestamped comment, which is part of a conversation.
/// </summary>
public class Comment
{
  public int Id { get; set; }
  public string Value { get; set; } = string.Empty; 
  public DateTime CommentDate { get; set; }
  public ApplicationUser Owner { get; set; } = null!;
  public bool Read { get; set; }
}
