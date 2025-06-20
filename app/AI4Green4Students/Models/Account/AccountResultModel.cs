namespace AI4Green4Students.Models.Account;

using User;

public record AccountResultModel(
  List<string> Errors,
  UserProfileModel? User = null,
  bool? IsInvalidActivationToken = null
);
