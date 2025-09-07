namespace AI4Green4Students.Models.Section.Form;

public record SectionSummaryModel(int Id, string Name, int SortOrder, SectionFeedbackModel Feedback);
public record SectionFeedbackModel(bool Approved, SectionFeedbackCommentModel Comments);
public record SectionFeedbackCommentModel(int Total, int Unread);
