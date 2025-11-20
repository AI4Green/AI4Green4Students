namespace AI4Green4Students.Models.ProjectGroup;

using System.ComponentModel.DataAnnotations;

public record InviteStudentModel([Required] List<string> Emails);
public record RemoveStudentModel([Required] string Id);
