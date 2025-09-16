namespace AI4Green4Students.Models.Section.Form;

using System.Text.Json;
using Field;

public record SectionFormModel(int Id, string Name, List<FieldResponseFormModel> FieldResponses);

public record FieldResponseFormModel(
  int? Id,
  FieldModel Field,
  FieldResponseFeedbackModel Feedback,
  JsonElement? Response
);

public record FieldResponseFeedbackModel(bool Approved, FieldResponseFeedbackCommentModel Comments);
public record FieldResponseFeedbackCommentModel(int Total, int Unread);
