using System.Text.Json;
using AI4Green4Students.Models.Field;

namespace AI4Green4Students.Models.Section;

public class SectionFormModel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<FieldResponseFormModel> FieldResponses { get; set; } = new List<FieldResponseFormModel>();
}

public record FieldResponseFormModel(
  int? Id,
  FieldModel Field,
  FieldResponseFeedbackModel Feedback,
  JsonElement? Response
);

public record FieldResponseFeedbackModel(bool Approved, FieldResponseFeedbackCommentModel Comments);
public record FieldResponseFeedbackCommentModel(int Total, int Unread);
