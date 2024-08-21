using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.Account.Login;

public record LoginModel(
  [Required]
  string Username,

  [Required]
  [DataType(DataType.Password)]
  string Password
);
