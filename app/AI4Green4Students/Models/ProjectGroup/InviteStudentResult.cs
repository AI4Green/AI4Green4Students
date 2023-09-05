namespace AI4Green4Students.Models.ProjectGroup;

public record InviteStudentResult
{
  public string Warning { get; init; } = string.Empty;
  public string Error { get; init; } = string.Empty;
}
