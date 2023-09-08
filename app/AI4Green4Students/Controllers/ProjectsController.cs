using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
  private readonly ProjectService _projects;
  private readonly UserManager<ApplicationUser> _users;

  public ProjectsController(ProjectService projects, UserManager<ApplicationUser> users)
  {
    _projects = projects;
    _users = users;
  }
  
  /// <summary>
  /// Get Project list based on user permissions
  /// </summary>
  /// <returns>Project list</returns>
  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  public async Task<ActionResult<List<ProjectModel>>> List()
  {
    if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllProjects))
      return await _projects.ListAll();
    
    var userId = _users.GetUserId(User);
    return userId is not null ? await _projects.ListByUser(userId) : Forbid();
  }
  
  
  /// <summary>
  /// Get project based on project id and user permission
  /// </summary>
  /// <param name="id">Project id to get</param>
  /// <returns>Project associated with the id</returns>
  [HttpGet("{id}")]
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  public async Task<ActionResult<ProjectModel>> Get(int id)
  {
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllProjects))
        return await _projects.Get(id);
    
      var userId = _users.GetUserId(User);
      return userId is not null ? await _projects.GetByUser(id, userId) : Forbid();      
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Delete project
  /// </summary>
  /// <param name="id">Project id to delete</param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  [Authorize(nameof(AuthPolicies.CanDeleteProjects))]
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
  [Authorize(nameof(AuthPolicies.CanCreateProjects))]
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
  [Authorize(nameof(AuthPolicies.CanEditProjects))]
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
