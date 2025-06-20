using AI4Green4Students.Models.User;

namespace AI4Green4Students.Models.Account.Login;

public record LoginResultModel(
  List<string> Errors,
  UserProfileModel? User = null,
  bool? IsUnconfirmedAccount = null
);
