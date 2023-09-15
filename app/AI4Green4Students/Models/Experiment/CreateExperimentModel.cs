using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.Experiment;

public record CreateExperimentModel(
  [Required] int ProjectGroupId,
  [Required] int ExperimentTypeId,
  [Required] string Title
);
