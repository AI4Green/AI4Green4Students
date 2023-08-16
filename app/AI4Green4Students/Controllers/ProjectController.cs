using AI4Green4Students.Auth;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
  private readonly ProjectService _projects;

  public ProjectController(ProjectService projects)
  {
    _projects = projects;
  }
  
  /// <summary>
  /// Get Project list
  /// </summary>
  /// <returns>Project list</returns>
  [HttpGet]
  public async Task<List<ProjectModel>> List() 
    => await _projects.List();


  /// <summary>
  /// Get project based on project id
  /// </summary>
  /// <param name="id">Project id to get</param>
  /// <returns>Project associated with the id</returns>
  [HttpGet("{id}")]
  [Authorize(nameof(AuthPolicies.CanManageUsers))]
  public async Task<ProjectModel> Get(int id)
  => await _projects.Get(id);
  
  
  /// <summary>
  /// Delete project
  /// </summary>
  /// <param name="id">Project id to delete</param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      await _projects.Delete(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Create project or update if project (project name) already exist
  /// </summary>
  /// <param name="model">Project data</param>
  /// <returns></returns>
  [HttpPost]
  public async Task<ActionResult> Create(CreateProjectModel model)
  {
    return Ok(await _projects.Create(model));
  }
  
  
  /// <summary>
  /// Update project
  /// </summary>
  /// <param name="id">Project id to update</param>
  /// <param name="model">Project update data</param>
  /// <returns></returns>
  [HttpPut("{id}")]
  public async Task<ActionResult> Set(int id, [FromBody] CreateProjectModel model)
  {
    try
    {
      return Ok(await _projects.Set(id, model));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
