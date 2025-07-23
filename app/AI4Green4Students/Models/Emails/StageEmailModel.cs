namespace AI4Green4Students.Models.Emails;

public record AdvanceStageEmailModel(
  string Owner,
  StageEmailProjectModel Project,
  StageEmailProjectGroupModel ProjectGroup,
  StageEmailItemModel Item,
  bool IsNewSubmission = false
)
{
  public string? Instructor { get; set; }
  public int CommentCount { get; set; }
  public string? TargetUrl { get; set; }
}

public record StageEmailProjectModel(int Id, string Name);
public record StageEmailProjectGroupModel(int Id, string Name);
public record StageEmailItemModel(int Id, string Name, string Type);
