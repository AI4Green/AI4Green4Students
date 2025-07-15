namespace AI4Green4Students.Models.Note;

public record CreateNoteFeedbackModel(
  string Value
);

public record NoteFeedbackModel(
  int Id,
  string? ReactionName,
  string Feedback
);
