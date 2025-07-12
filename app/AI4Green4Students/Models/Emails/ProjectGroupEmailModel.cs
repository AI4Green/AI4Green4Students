namespace AI4Green4Students.Models.Emails;

public record ProjectGroupEmailModel(
  EmailAddress Recipient,
  string Project,
  string ProjectGroup
);
