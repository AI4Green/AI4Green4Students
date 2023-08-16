using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.Project;

public record CreateProjectModel(
  [Required] string Name 
);
