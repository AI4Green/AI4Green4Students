namespace AI4Green4Students.Data.Entities;

/// <summary>
/// The response to a particular field by a certain student. Has a list of response values to track the changes made over time
/// </summary>
public class FieldResponse
{
  public int Id { get; set; }
  public Field Field { get; set; } = null!;
  public List<FieldResponseValue> FieldResponseValues { get; set; } = new List<FieldResponseValue>();
  public bool Approved { get; set; }

  public List<ProjectGroupFieldResponse> ProjectGroupFieldResponses { get; set; } = new List<ProjectGroupFieldResponse>();
  public List<LiteratureReviewFieldResponse> LiteratureReviewFieldResponses { get; set; } = new List<LiteratureReviewFieldResponse>();
  public List<PlanFieldResponse> PlanFieldResponses { get; set; } = new List<PlanFieldResponse>();
  public List<NoteFieldResponse> NoteFieldResponses { get; set; } = new List<NoteFieldResponse>();
  public List<ReportFieldResponse> ReportFieldResponses { get; set; } = new List<ReportFieldResponse>();
  public List<Comment> Conversation { get; set; } = new List<Comment>();
}
