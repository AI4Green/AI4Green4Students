namespace AI4Green4Students.Models.ProjectGroup;

public record BulkInviteStudentResult
{
  public ProjectGroupModel? ProjectGroup { get; init; }
  public List<string>? Warnings { get; init; } = new();
  public List<string>? Errors { get; init; } = new();
}
