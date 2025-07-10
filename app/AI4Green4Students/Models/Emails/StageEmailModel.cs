namespace AI4Green4Students.Models.Emails;

public record AdvanceStageEmailModel(
  string Owner,
  string Project,
  string ProjectGroup,
  string Type,
  string? Name = null,
  bool IsNewSubmission = false
)
{
  public string? Instructor { get; set; }
  public int CommentCount { get; set; }
}
