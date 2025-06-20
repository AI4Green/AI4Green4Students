namespace AI4Green4Students.Models.Account.Email;

using User;

public record EmailResultModel(
  List<string> Errors,
  UserProfileModel? User = null,
  bool? IsUnconfirmedAccount = null
);
