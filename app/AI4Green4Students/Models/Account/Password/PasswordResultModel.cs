namespace AI4Green4Students.Models.Account.Password;

using User;

public record PasswordResultModel(
  List<string> Errors,
  UserProfileModel? User = null,
  bool? IsUnconfirmedAccount = null
);
