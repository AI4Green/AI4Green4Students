using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Models.Comment;

public class CreateCommentModel
{
  public string Value { get; set; } = string.Empty;
  public int FieldResponseId { get; set; }
  public ApplicationUser? User { get; set; }
  public bool IsInstructor { get; set; } 
}
