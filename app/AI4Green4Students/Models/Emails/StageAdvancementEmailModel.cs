namespace AI4Green4Students.Models.Emails;

public record StageAdvancementEmailModel(
  string StudentName,
  string ProjectName,
  string ProjectGroupName,
  string RecordType,
  string? RecordName = null,
  bool IsNewSubmission = false
)
{
  public string? InstructorName { get; set; }
  public int CommentCount { get; set; }
}
