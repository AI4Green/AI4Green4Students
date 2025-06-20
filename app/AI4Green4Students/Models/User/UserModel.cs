using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.User;

public record UserModel(
  [Required] string Id,
  [Required, EmailAddress] string Email,
  string FullName,
  bool EmailConfirmed,
  string? UICulture,
  [Required] List<string> Roles
);
