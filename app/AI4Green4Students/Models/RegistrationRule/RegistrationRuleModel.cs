namespace AI4Green4Students.Models;

public record RegistrationRuleModel {
  public int Id { get; set; }
  public string Value { get; set; } = string.Empty;
  public bool IsBlocked { get; set; }
  public DateTimeOffset Modified { get; set; }
}
