namespace AI4Green4Students.Models.User;

using System.ComponentModel.DataAnnotations;

public record UserInviteModel(
  [Required] string Email,
  [Required] List<string> Roles
);

public record UserInviteResultModel(
  List<string> Errors,
  bool? IsNotAllowedToRegister = null,
  bool? IsExistingUser = null,
  bool? HasInvalidRoles = null,
  bool? HasNoRoles = null,
  bool? IsExistingEmail = null
);
