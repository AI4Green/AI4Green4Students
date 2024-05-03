namespace AI4Green4Students.Data.Entities.SectionTypeData;

public abstract class BaseSectionTypeData
{
  public int Id { get; set; }
  public List<FieldResponse> FieldResponses { get; set; } = new ();
}
