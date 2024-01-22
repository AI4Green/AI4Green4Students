using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Plan;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlansController : ControllerBase
{
  private readonly PlanService _plans;
  private readonly UserManager<ApplicationUser> _users;

  public PlansController(PlanService plans, UserManager<ApplicationUser> users)
  {
    _plans = plans;
    _users = users;
  }

  /// <summary>
  /// Get plan list for a given user.
  /// Each plan is associated with a project's project group initialised by the Instructor.
  /// </summary>
  /// <param name="projectId">Id of the project to get plans for.</param>
  /// <returns>List of plans for the given project.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet]
  public async Task<ActionResult<List<PlanModel>>> List(int projectId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _plans.ListByUser(projectId, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get plan list for a given project group.
  /// Project groups consist of students allocated by the Instructor.
  /// Allows instructors to view all students plans for a given project group.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to get plans for.</param>
  /// <returns>List of plans for the given project group.</returns>
  [Authorize(nameof(AuthPolicies.CanViewAllProjects))]
  [HttpGet("ListProjectGroupPlans")]
  public async Task<ActionResult<List<PlanModel>>> ListProjectGroupPlans(int projectGroupId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _plans.ListByProjectGroup(projectGroupId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Create a new plan. 
  /// </summary>
  /// <returns>Newly created plan.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPost]
  public async Task<ActionResult<PlanModel>> Create(int projectGroupId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _plans.Create(userId, projectGroupId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Delete plan by its id.
  /// </summary>
  /// <param name="id">The id of plan to delete.</param>
  /// <returns>If the deletion is successful then no content</returns>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      await _plans.Delete(id, userId);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
