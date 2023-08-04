using System.ComponentModel.DataAnnotations;
using AI4Green4Students.Models.Account.Token;

namespace AI4Green4Students.Models.Account.Password;

/// <summary>
/// Model for setting a known User's password
/// </summary>
/// <param name="Password"></param>
/// <param name="PasswordConfirm"></param>
public record SetPasswordModel(
  [Required]
  [DataType(DataType.Password)]
  string Password
);

/// <summary>
/// Model for setting a password when the User isn't already known implicitly by the system
/// (i.e. they are unauthenticated, so Anonymous in that sense).
/// </summary>
/// <param name="Credentials">The Credentials that authorise the reset: UserId and Password Reset Token</param>
/// <param name="Data">The Payload for the reset: Password and PasswordConfirm</param>
public record AnonymousSetPasswordModel(
  UserTokenModel Credentials,
  SetPasswordModel Data
);
