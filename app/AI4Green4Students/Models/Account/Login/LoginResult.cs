using AI4Green4Students.Models.User;

namespace AI4Green4Students.Models.Account.Login;

public record LoginResult
{
  public UserProfileModel? User { get; set; }
  public bool? IsUnconfirmedAccount { get; init; } = null;
  public List<string> Errors { get; init; } = new();
}
