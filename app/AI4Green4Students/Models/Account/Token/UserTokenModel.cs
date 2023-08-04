using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.Account.Token;

public record UserTokenModel(
    [Required]
    string UserId,
    [Required]
    string Token);

