namespace AI4Green4Students.Controllers;

using Auth;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Plan;
using Models.Section;
using Models.Stage;
using Services;

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
  /// List user's plans for a given project.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <returns>User's project plans.</returns>
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
  /// Get plan.
  /// </summary>
  /// <param name="id">Plan id.</param>
  /// <returns>Plan</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<PlanModel>> Get(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _plans.IsOwner(userId, id) ||
                         await _plans.IsProjectInstructor(userId, id) ||
                         await _plans.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _plans.Get(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Create a new plan.
  /// </summary>
  /// <param name="model">Create model.</param>
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
  /// Delete plan.
  /// </summary>
  /// <param name="id">Plan id.</param>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null || !await _plans.IsOwner(userId, id))
      {
        return Forbid();
      }

      await _plans.Delete(id, userId);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// List sections summary, includes information, such as completion status and unread comments.
  /// </summary>
  /// <param name="id">Plan id.</param>
  /// <returns>Sections summary.</returns>
  [HttpGet("{id}/summary")]
  public async Task<ActionResult<List<SectionSummaryModel>>> ListSummary(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _plans.IsOwner(userId, id) ||
                         await _plans.IsProjectInstructor(userId, id) ||
                         await _plans.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _plans.ListSummary(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a section form.
  /// </summary>
  /// <param name="id">Plan id.</param>
  /// <param name="sectionId">Section id.</param>
  /// <returns>Section form.</returns>
  [HttpGet("{id}/form/{sectionId}")]
  public async Task<ActionResult<SectionFormModel>> GetSectionForm(int id, int sectionId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _plans.IsOwner(userId, id) ||
                         await _plans.IsProjectInstructor(userId, id) ||
                         await _plans.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _plans.GetSectionForm(id, sectionId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Save section form.
  /// </summary>
  /// <param name="model">Section form payload model.</param>
  /// <returns>Saved data.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPut("save-form")]
  [Consumes("multipart/form-data")]
  public async Task<ActionResult<SectionFormModel>> SaveSectionForm([FromForm] SectionFormPayloadModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = userId is not null &&
                         User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments) &&
                         await _plans.IsOwner(userId, model.RecordId);

      if (!isAuthorised)
      {
        return Forbid();
      }

      return await _plans.SaveSectionForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Advance the stage.
  /// </summary>
  /// <param name="id">Plan id.</param>
  /// <param name="setStage">Stage to advance to.</param>
  [HttpPost("{id}/advance")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isAuthorised = await _plans.IsOwner(userId, id) || await _plans.IsProjectInstructor(userId, id);

    if (!isAuthorised)
    {
      return Forbid();
    }

    try
    {
      await _plans.AdvanceStage(id, userId, setStage.StageName);
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
    catch (InvalidOperationException e)
    {
      return Conflict(e.Message);
    }
  }
}
