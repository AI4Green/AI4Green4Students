namespace AI4Green4Students.Models.Project;

public record CreateProjectModel(string Name, List<string> InstructorIds, int ProjectTypeId);
