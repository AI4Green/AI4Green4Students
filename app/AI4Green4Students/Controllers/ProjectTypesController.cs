namespace AI4Green4Students.Controllers;

using Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ProjectType;
using Services;

[ApiController]
[Route("api/project-types")]
[Authorize]
public class ProjectTypesController : ControllerBase
{
  private readonly ProjectTypeService _projectType;

  public ProjectTypesController(ProjectTypeService projectType) => _projectType = projectType;

  /// <summary>
  /// Create a new project type.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>New project type.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateProjectTypes))]
  [HttpPost]
  public async Task<IActionResult> Create(CreateProjectTypeModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    try
    {
      return Ok(await _projectType.Create(model));
    }
    catch (KeyNotFoundException e)
    {
      return BadRequest(e.Message);
    }
  }

  /// <summary>
  /// Update a project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Updated project type.</returns>
  [HttpPut("{id}")]
  public async Task<IActionResult> Set(int id, CreateProjectTypeModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    try
    {
      return Ok(await _projectType.Set(id, model));
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
  }

  /// <summary>
  /// Delete a project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      await _projectType.Delete(id);
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
    catch (InvalidOperationException e)
    {
      return BadRequest(e.Message);
    }
  }

  /// <summary>
  /// List project types.
  /// </summary>
  /// <returns>Project types.</returns>
  [Authorize(nameof(AuthPolicies.CanViewProjectTypes))]
  [HttpGet]
  public async Task<IActionResult> List() => Ok(await _projectType.List());

  /// <summary>
  /// Get a project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <returns>Project type.</returns>
  [Authorize(nameof(AuthPolicies.CanViewProjectTypes))]
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
