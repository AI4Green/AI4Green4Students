using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Plan;
using AI4Green4Students.Models.Stage;
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
  [Authorize(nameof(AuthPolicies.CanViewOwnExperiments))]
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
  /// Get plan. Only the owner or instructor can view the plan.
  /// </summary>
  /// <param name="planId">Id of the plan.</param>
  /// <returns>Plan</returns>
  [HttpGet("{planId}")]
  public async Task<ActionResult<PlanModel>> Get(int planId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null && (
        await _plans.IsPlanOwner(userId, planId) ||
        User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)
      )
        ? await _plans.Get(planId)
        : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Create a new plan. 
  /// </summary>
  /// <param name="model">Plan dto model. Currently only contains project group id.</param>
  /// <returns>Newly created plan.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPost]
  public async Task<ActionResult<PlanModel>> Create(CreatePlanModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _plans.Create(userId, model) : Forbid();
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
      if (userId is null || !await _plans.IsPlanOwner(userId, id)) return Forbid();

      await _plans.Delete(id, userId);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  [HttpPost("{id}/AdvanceStage")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var nextStage = await _plans.AdvanceStage(id, setStage.StageName);
    if (nextStage is null)
    {
      return Conflict();
    }
    return Ok(nextStage);
  }
}
