namespace AI4Green4Students.Data.Entities;

public class RegistrationRule
{
  public int Id { get; set; }
  public string Value  { get; set; } = String.Empty;
  public DateTimeOffset Modified { get; set; } = DateTimeOffset.UtcNow;
  public bool IsBlocked { get; set; }
}
