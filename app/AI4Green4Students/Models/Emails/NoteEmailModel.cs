namespace AI4Green4Students.Models.Emails;

/// <summary>
/// Model for sending a note feedback request/completion email.
/// </summary>
public record NoteFeedBackEmailModel(
  string Project,
  string Owner,
  string Url,
  string Instructor,
  string Plan
);
