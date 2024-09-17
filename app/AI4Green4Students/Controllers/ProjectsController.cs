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
  /// List projects based on user's role
  /// </summary>
  /// <returns>Project list</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet]
  public async Task<ActionResult<List<ProjectModel>>> ListByUser()
  {
    var userId = _users.GetUserId(User);
    if (userId is null) return Forbid();

    var isInstructor = User.IsInRole(Roles.Instructor);
    return isInstructor
      ? await _projects.ListByInstructor(userId)
      : await _projects.ListByStudent(userId);
  }
  
  /// <summary>
  /// Get project based on project id and user's role
  /// </summary>
  /// <param name="id">Project id to get</param>
  /// <returns>Project associated with the id</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet("{id}")]
  public async Task<ActionResult<ProjectModel>> GetByUser(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();
      
      var isInstructor = User.IsInRole(Roles.Instructor);
      return isInstructor
        ? await _projects.GetByInstructor(id, userId)
        : await _projects.GetByStudent(id, userId); 
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
      if (userId is null) return Forbid();

      if (studentId is null)
        return User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments)
          ? await _projects.GetStudentProjectSummary(id, userId, true) // get own project summary
          : Forbid();

      // check if user is authorised to view project summary
      var isProjectGroupFellow = await _projects.IsInSameProjectGroup(userId, studentId, id) &&
                                 User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjectGroupExperiments);
      if (isProjectGroupFellow) return await _projects.GetStudentProjectSummary(id, studentId);
      
      var isProjectInstructor = await _projects.IsProjectInstructor(userId, id) &&
                                User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewProjectExperiments);
      if (isProjectInstructor) return await _projects.GetStudentProjectSummary(id, studentId, false, true);

      return Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
