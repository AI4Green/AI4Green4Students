using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.ProjectGroup;

public record CreateProjectGroupModel(
  [Required] string Name,
  [Required] int ProjectId
);
