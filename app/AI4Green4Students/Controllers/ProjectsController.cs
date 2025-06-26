namespace AI4Green4Students.Controllers;

using Auth;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Project;
using Services;

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
  /// List projects based on user's role.
  /// </summary>
  /// <returns>Project list.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet]
  public async Task<ActionResult<List<ProjectModel>>> ListByUser()
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isInstructor = User.IsInRole(Roles.Instructor);
    return isInstructor
      ? await _projects.ListByInstructor(userId)
      : await _projects.ListByStudent(userId);
  }

  /// <summary>
  /// Get a project based on and user's role.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>Project.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet("{id}")]
  public async Task<ActionResult<ProjectModel>> GetByUser(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

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
  /// Delete project.
  /// </summary>
  /// <param name="id">Project id.</param>
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
  /// Create project.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Created project.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateProjects))]
  [HttpPost]
  public async Task<ActionResult> Create(CreateProjectModel model) => Ok(await _projects.Create(model));


  /// <summary>
  /// Update project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Updated project.</returns>
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
  /// Get project summary. Only for students.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="studentId">Student id.</param>
  /// <returns>Project summary</returns>
  [HttpGet("{id}/summary")]
  public async Task<ActionResult<ProjectSummaryModel>> GetStudentProjectSummary(int id, string? studentId = null)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      if (studentId is null)
      {
        return User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments)
          ? await _projects.GetStudentProjectSummary(id, userId, true)
          : Forbid();
      }

      var isProjectGroupFellow = await _projects.IsInSameProjectGroup(userId, studentId, id) &&
                                 User.HasClaim(CustomClaimTypes.SitePermission,
                                   SitePermissionClaims.ViewProjectGroupExperiments);
      if (isProjectGroupFellow)
      {
        return await _projects.GetStudentProjectSummary(id, studentId);
      }

      var isProjectInstructor = await _projects.IsProjectInstructor(userId, id) &&
                                User.HasClaim(CustomClaimTypes.SitePermission,
                                  SitePermissionClaims.ViewProjectExperiments);
      if (isProjectInstructor)
      {
        return await _projects.GetStudentProjectSummary(id, studentId, false, true);
      }

      return Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
