using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models;

public record CreateRegistrationRuleModel(
  [Required] string Value,

  [Required] bool IsBlocked
);
