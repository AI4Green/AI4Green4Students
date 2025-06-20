namespace AI4Green4Students.Models.Account.Register;

public record RegisterResultModel(
  List<string> Errors,
  bool? IsExistingUser = null,
  bool? IsNotAllowedToRegister = null
);
