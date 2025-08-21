namespace AI4Green4Students.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController]
[Route("api/project-types")]
[Authorize]
public class ProjectTypesController : ControllerBase
{
  private readonly ProjectTypeService _projectType;

  public ProjectTypesController(ProjectTypeService projectType) => _projectType = projectType;

  /// <summary>
  /// List project types.
  /// </summary>
  /// <returns>Project types.</returns>
  [HttpGet]
  public async Task<IActionResult> List() => Ok(await _projectType.List());

  /// <summary>
  /// Get a project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <returns>Project type.</returns>
  [HttpGet("{id}")]
  public async Task<IActionResult> Get(int id)
  {
    try
    {
      var projectType = await _projectType.Get(id);
      return Ok(projectType);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

}
