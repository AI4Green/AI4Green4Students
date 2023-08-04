using AI4Green4Students.Models.User;

namespace AI4Green4Students.Models.Account.Email;

public record SetAccountActivateResult
{
  public UserProfileModel? User { get; init; } = null;
  public bool? IsActivationTokenInvalid { get; init; } = null;
  public List<string> Errors { get; init; } = new();
};


