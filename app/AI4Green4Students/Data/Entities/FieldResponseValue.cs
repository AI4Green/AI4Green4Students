namespace AI4Green4Students.Data.Entities;

public class FieldResponseValue
{
  /// <summary>
  /// The specific responses made at a certain time to a field. Held in a collection by the FieldResponse.
  /// </summary>
  public int Id { get; set; } 
  public FieldResponse FieldResponse { get; set; } = null!;
  public string Value { get; set; } = string.Empty;
  public DateTimeOffset ResponseDate { get; set; } = DateTimeOffset.UtcNow;
}
