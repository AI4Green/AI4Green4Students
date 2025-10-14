namespace AI4Green4Students.Models.ProjectType;

/// <summary>
/// Create model.
/// </summary>
/// <param name="Name">Project type name.</param>
/// <param name="Description">Description.</param>
/// <param name="Id">Existing project type ID.</param>
public record CreateProjectTypeModel(string Name, string Description, int? Id = null);
