namespace AI4Green4Students.Models.Emails;

public record NotifyStudentOfProjectGroupModel(
  string RecipientName,
  string ProjectName,
  string ProjectGroupName);
