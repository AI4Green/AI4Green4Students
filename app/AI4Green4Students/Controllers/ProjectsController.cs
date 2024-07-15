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
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet]
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
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet("{id}")]
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
  [Authorize(nameof(AuthPolicies.CanDeleteProjects))]
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
  [Authorize(nameof(AuthPolicies.CanCreateProjects))]
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
  [Authorize(nameof(AuthPolicies.CanEditProjects))]
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
  
  /// <summary>
  /// Get project summary. Only available for students.
  /// </summary>
  /// <param name="id">Project id to get summary</param>
  /// <param name="studentId">Student id to get project summary</param>
  /// <returns>Project summary</returns>
  [HttpGet("{id}/project-summary")]
  public async Task<ActionResult<ProjectSummaryModel>> GetStudentProjectSummary(int id, string? studentId = null)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (studentId is null) return userId is not null ? await _projects.GetStudentProjectSummary(id, userId) : Forbid();

      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments);
      return isAuthorised ? await _projects.GetStudentProjectSummary(id, studentId, true) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get project summary for project group. Only available for instructors.
  /// </summary>
  /// <param name="projectGroupId">Project group id to get project summary</param>
  /// <returns>Project summary for project group</returns>
  [Authorize(nameof(AuthPolicies.CanViewAllExperiments))]
  [HttpGet("GetProjectGroupProjectSummary")]
  public async Task<ActionResult<ProjectSummaryModel>> GetProjectGroupProjectSummary(int projectGroupId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _projects.GetProjectGroupProjectSummary(projectGroupId) : Forbid();      
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
