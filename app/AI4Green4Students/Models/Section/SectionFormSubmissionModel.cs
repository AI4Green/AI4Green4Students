using System.Text.Json;

namespace AI4Green4Students.Models.Section;

/// <summary>
/// Payload sent whenever a section is saved, linking the responses back to their fields, using RecordId (eg. planId, literatureReviewId)
/// and FieldId to connect them
/// </summary>
public class SectionFormSubmissionModel
{
  public int SectionId { get; set; }
  public List<FieldResponseSubmissionModel> FieldResponses { get; set; } = new();
  public List<FieldResponseSubmissionModel> NewFieldResponses { get; set; } = new();
  public int RecordId { get; set; }
  public string SectionType { get; set; } = String.Empty;

}

public class FieldResponseSubmissionModel
{
  public int Id { get; set; }
  public string Value { get; set; } = String.Empty;
}

public class FieldResponseHelperModel
{
  public int Id { get; set; }
  public JsonElement Value { get; set; }
}
