
using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.Account.Register;

public record RegisterAccountModel(
  [Required]
  [DataType(DataType.Text)]
  string FullName,

  [Required]
  [EmailAddress]
  string Email,

  [Required]
  [DataType(DataType.Password)]
  string Password
);
