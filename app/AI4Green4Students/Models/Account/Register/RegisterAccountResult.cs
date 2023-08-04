namespace AI4Green4Students.Models.Account.Register;

public record RegisterAccountResult
{
  public bool? IsExistingUser { get; init; } = null;
  public bool? IsNotAllowlisted { get; init; } = null;
  public bool? IsRolesNotValidOrSelected { get; init; } = null;
  public bool? IsIncompleteField { get; init; } = null;
  public List<string> Errors { get; init; } = new();
};


