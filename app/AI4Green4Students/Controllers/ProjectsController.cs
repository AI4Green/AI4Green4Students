namespace AI4Green4Students.Controllers;

using System.Security.Claims;
using Auth;
using Data.Entities.Identity;
using Extensions;
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
  /// List projects.
  /// </summary>
  /// <returns>Projects.</returns>
  [Authorize(nameof(AuthPolicies.CanViewProjects))]
  [HttpGet]
  public async Task<IActionResult> ListByUser()
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }
    var isModuleConvenor = User.IsInRole(Roles.ModuleConvenor);
    var isInstructor = User.IsInRole(Roles.Instructor);

    var list = isModuleConvenor
      ? await _projects.List()
      : isInstructor
        ? await _projects.ListByInstructor(userId)
        : await _projects.ListByStudent(userId);

    return Ok(list);
  }

  /// <summary>
  /// Get a project.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <returns>Project.</returns>
  [Authorize(nameof(AuthPolicies.CanViewProjects))]
  [HttpGet("{id}")]
  public async Task<IActionResult> GetByUser(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isModuleConvenor = User.IsInRole(Roles.ModuleConvenor);
      var isInstructor = User.IsInRole(Roles.Instructor);

      var project = isModuleConvenor
        ? await _projects.Get(id)
        : isInstructor
          ? await _projects.GetByInstructor(id, userId)
          : await _projects.GetByStudent(id, userId);

      return Ok(project);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Delete a project.
  /// </summary>
  /// <param name="id">Project Id.</param>
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
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Create project.
  /// </summary>
  /// <param name="model">Create model.</param>
  [Authorize(nameof(AuthPolicies.CanCreateProjects))]
  [HttpPost]
  public async Task<IActionResult> Create(CreateProjectModel model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest();
    }

    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId is null)
    {
      return Forbid();
    }

    try
    {
      await _projects.Create(model, userId);
      return NoContent();
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
  }

  /// <summary>
  /// Update project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="model">Update model.</param>
  [Authorize(nameof(AuthPolicies.CanEditProjects))]
  [HttpPut("{id}")]
  public async Task<IActionResult> Set(int id, [FromBody] CreateProjectModel model)
  {
    try
    {
      await _projects.Set(id, model);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get student project summary.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <param name="studentId">Student Id.</param>
  /// <returns>Project summary.</returns>
  [HttpGet("{id}/summary")]
  public async Task<IActionResult> GetStudentProjectSummary(int id, string? studentId = null)
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
        return User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewExperiments)
          ? Ok(await _projects.GetStudentProjectSummary(id, userId, true))
          : Forbid();
      }

      var isGroupMember = await _projects.IsInSameProjectGroup(userId, studentId, id) &&
                          User.HasClaim(CustomClaimTypes.SitePermission,
                            SitePermissionClaims.ViewProjectGroupExperiments);
      if (isGroupMember)
      {
        return Ok(await _projects.GetStudentProjectSummary(id, studentId));
      }

      var isProjectInstructor = await _projects.IsProjectInstructor(userId, id) &&
                                User.HasClaim(CustomClaimTypes.SitePermission,
                                  SitePermissionClaims.ViewProjectExperiments);
      if (isProjectInstructor)
      {
        return Ok(await _projects.GetStudentProjectSummary(id, studentId, false, true));
      }

      return Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// List project instructors.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <returns>Instructors.</returns>
  [Authorize(nameof(AuthPolicies.CanInviteInstructors))]
  [HttpGet("{id}/instructors")]
  public async Task<IActionResult> ListInstructors(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isModuleConvenor = User.IsInRole(Roles.ModuleConvenor);
      if (isModuleConvenor)
      {
        return Ok(await _projects.ListInstructors(id));
      }

      return Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Bulk invite instructors to a project.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <param name="model">Invite model.</param>
  [Authorize(nameof(AuthPolicies.CanInviteInstructors))]
  [HttpPost("{id}/invite-instructors")]
  public async Task<ActionResult> InviteInstructors(int id, InviteModel model)
  {
    try
    {
      await _projects.InviteInstructors(id, model.Emails, Request.GetUICulture().Name);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Validate project instructor.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <returns>Validation result.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateProjectGroups))]
  [HttpPost("{id}/validate-instructor")]
  public async Task<IActionResult> ValidateInstructor(int id)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isInstructor = await _projects.IsProjectInstructor(userId, id);
    if (!isInstructor)
    {
      return Forbid();
    }

    return NoContent();
  }

  /// <summary>
  /// Remove an instructor from a project.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <param name="model">Remove model.</param>
  [Authorize(nameof(AuthPolicies.CanInviteInstructors))]
  [HttpPost("{id}/remove-instructor")]
  public async Task<IActionResult> RemoveInstructor(int id, RemoveModel model)
  {
    try
    {
      await _projects.RemoveInstructor(id, model.Id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
