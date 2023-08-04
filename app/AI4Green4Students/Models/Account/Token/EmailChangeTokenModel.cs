using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.Account.Token;

public record EmailChangeTokenModel(
  [Required]
  string UserId,
  [Required]
  string Token,
  [Required]
  string NewEmail);
