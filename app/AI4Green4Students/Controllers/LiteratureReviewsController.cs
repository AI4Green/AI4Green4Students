namespace AI4Green4Students.Controllers;

using Auth;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.LiteratureReview;
using Models.Section;
using Models.Stage;
using Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LiteratureReviewsController : ControllerBase
{
  private readonly LiteratureReviewService _literatureReviews;
  private readonly UserManager<ApplicationUser> _users;

  public LiteratureReviewsController(LiteratureReviewService literatureReviews, UserManager<ApplicationUser> users)
  {
    _literatureReviews = literatureReviews;
    _users = users;
  }

  /// <summary>
  /// List user's project literature reviews.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>List user's literature reviews.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnExperiments))]
  [HttpGet]
  public async Task<ActionResult<List<LiteratureReviewModel>>> List(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _literatureReviews.ListByUser(id, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get literature review.
  /// </summary>
  /// <param name="id">Literature review id.</param>
  /// <returns>Literature review.</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<LiteratureReviewModel>> Get(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _literatureReviews.IsOwner(userId, id) ||
                         await _literatureReviews.IsProjectInstructor(userId, id) ||
                         await _literatureReviews.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _literatureReviews.Get(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Create a new literature review.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Newly created literature review.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPost]
  public async Task<ActionResult<LiteratureReviewModel>> Create(CreateLiteratureReviewModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _literatureReviews.Create(userId, model) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Delete literature review.
  /// </summary>
  /// <param name="id">Literature review id.</param>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null || !await _literatureReviews.IsOwner(userId, id))
      {
        return Forbid();
      }

      await _literatureReviews.Delete(id, userId);
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
  /// <param name="id">Literature review id.</param>
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

      var isAuthorised = await _literatureReviews.IsOwner(userId, id) ||
                         await _literatureReviews.IsProjectInstructor(userId, id) ||
                         await _literatureReviews.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _literatureReviews.ListSummary(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a section form.
  /// </summary>
  /// <param name="id">Literature review id.</param>
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

      var isAuthorised = await _literatureReviews.IsOwner(userId, id) ||
                         await _literatureReviews.IsProjectInstructor(userId, id) ||
                         await _literatureReviews.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _literatureReviews.GetSectionForm(id, sectionId) : Forbid();
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
                         await _literatureReviews.IsOwner(userId, model.RecordId);

      if (!isAuthorised)
      {
        return Forbid();
      }

      return await _literatureReviews.SaveSectionForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Advance the stage.
  /// </summary>
  /// <param name="id">Literature review id.</param>
  /// <param name="setStage">Stage to advance to.</param>
  [HttpPost("{id}/advance")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isAuthorised = await _literatureReviews.IsOwner(userId, id) ||
                       await _literatureReviews.IsProjectInstructor(userId, id);

    if (!isAuthorised)
    {
      return Forbid();
    }

    try
    {
      await _literatureReviews.AdvanceStage(id, userId, setStage.StageName);
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
