using AI4Green4Students.Models.User;

namespace AI4Green4Students.Models.Account.Password;
public record SetPasswordResult
{
  public UserProfileModel? User { get; init; } = null;
  public bool? IsUnconfirmedAccount { get; init; } = null;
  public List<string>? Errors { get; init; } = new();
}
