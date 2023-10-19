namespace AI4Green4Students.Data.Entities;

/// <summary>
/// A specific datestamped comment, which is part of a conversation.
/// </summary>
public class Comment
{
  public int Id { get; set; }
  public Conversation Conversation { get; set; } = null!;
  public string Value { get; set; } = string.Empty; 
  public DateTime CommentDate { get; set; } 
}
