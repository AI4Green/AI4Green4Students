using System.ComponentModel.DataAnnotations;
using AI4Green4Students.Models.Account.Token;

namespace AI4Green4Students.Models.Account.Email;

/// <summary>
/// Model for setting a known User's email
/// </summary>
/// <param name="NewEmail"></param>

public record SetEmailModel(
  [Required]
  [DataType(DataType.EmailAddress)]
  string NewEmail
);

/// <summary>
/// Model for setting a email when the User isn't already known implicitly by the system
/// (i.e. they are unauthenticated, so Anonymous in that sense).
/// </summary>
/// <param name="Credentials">The Credentials that authorise the email change: UserId and Email Change Token</param>
/// <param name="Data">The Payload for the email change: Email </param>
public record AnonymousSetEmailModel(
  UserTokenModel Credentials,
  SetEmailModel Data
);

