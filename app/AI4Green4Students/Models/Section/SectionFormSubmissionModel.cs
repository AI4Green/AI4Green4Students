namespace AI4Green4Students.Models.Section;

/// <summary>
/// Payload sent whenever a section is saved, linking the responses back to their fields, using PlanId and FieldId to connect them
/// </summary>
public class SectionFormSubmissionModel
{
  public int SectionId { get; set; }
  public List<FieldResponseSubmissionModel> FieldResponses { get; set; } = new List<FieldResponseSubmissionModel>();
  public int PlanId { get; set; }

}

public class FieldResponseSubmissionModel
{
  public int Id { get; set; }
  public string Value { get; set; } = String.Empty;
}
