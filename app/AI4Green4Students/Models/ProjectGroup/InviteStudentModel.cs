using System.ComponentModel.DataAnnotations;

namespace AI4Green4Students.Models.ProjectGroup;

public record InviteStudentModel(
  [Required] int ProjectId,
  [Required] List<string> Emails
);
