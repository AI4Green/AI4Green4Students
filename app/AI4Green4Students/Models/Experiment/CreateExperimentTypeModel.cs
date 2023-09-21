using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.Experiment;

public record CreateExperimentTypeModel(
  [Required] string Name
);
