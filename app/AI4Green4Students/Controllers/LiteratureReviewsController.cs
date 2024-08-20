using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.LiteratureReview;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Models.Stage;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

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
  /// Get literature review list for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get literature reviews for.</param>
  /// <returns>List of literature reviews for the given project.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnExperiments))]
  [HttpGet]
  public async Task<ActionResult<List<LiteratureReviewModel>>> List(int projectId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _literatureReviews.ListByUser(projectId, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get literature review. Only the owner or instructor can view the literature review.
  /// </summary>
  /// <param name="literatureReviewId">Id of the literature review.</param>
  /// <returns>Literature review</returns>
  [HttpGet("{literatureReviewId}")]
  public async Task<ActionResult<LiteratureReviewModel>> Get(int literatureReviewId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      var isAuthorised = await _literatureReviews.IsLiteratureReviewOwner(userId, literatureReviewId) ||
                         await _literatureReviews.IsProjectInstructor(userId, literatureReviewId) ||
                         await _literatureReviews.IsInSameProjectGroup(userId, literatureReviewId);

      return isAuthorised ? await _literatureReviews.Get(literatureReviewId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Create a new literature review. 
  /// </summary>
  /// <param name="model">Literature review dto model. Currently only contains project group id.</param>
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
  /// Delete literature review by its id.
  /// </summary>
  /// <param name="id">The id of literature review to delete.</param>
  /// <returns>If the deletion is successful then no content</returns>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null || !await _literatureReviews.IsLiteratureReviewOwner(userId, id)) return Forbid();

      await _literatureReviews.Delete(id, userId);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get a list of literature review sections including sections status, such as completion status and no. of unread comments.
  /// </summary>
  /// <param name="literatureReviewId">Id of the student's literature review to be used for generating literature review sections status.</param>
  /// <returns>List of literature review sections with status.</returns>
  [HttpGet("summary/{literatureReviewId}")]
  public async Task<ActionResult<List<SectionSummaryModel>>> ListSummary(int literatureReviewId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      var isAuthorised = await _literatureReviews.IsLiteratureReviewOwner(userId, literatureReviewId) ||
                         await _literatureReviews.IsProjectInstructor(userId, literatureReviewId) ||
                         await _literatureReviews.IsInSameProjectGroup(userId, literatureReviewId);

      return isAuthorised ? await _literatureReviews.ListSummary(literatureReviewId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get literature review section form, which includes section fields and its responses.
  /// </summary>
  /// <param name="sectionId"> Id of section to get form for. </param>
  /// <param name="literatureReviewId"> Id of student's literatureReview to get field responses for. </param>
  /// <returns>Literature review section form for the given literature review matching the given section.</returns> 
  [HttpGet("form/{literatureReviewId}/{sectionId}")]
  public async Task<ActionResult<SectionFormModel>> GetSectionForm(int literatureReviewId, int sectionId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      var isAuthorised = await _literatureReviews.IsLiteratureReviewOwner(userId, literatureReviewId) ||
                         await _literatureReviews.IsProjectInstructor(userId, literatureReviewId) ||
                         await _literatureReviews.IsInSameProjectGroup(userId, literatureReviewId);

      return isAuthorised ? await _literatureReviews.GetSectionForm(literatureReviewId, sectionId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Save the field responses for a section accordingly to the section type.
  /// </summary>
  /// <param name="model"> Section form payload model. </param>
  /// <returns> saved section form data.</returns>
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
                         await _literatureReviews.IsLiteratureReviewOwner(userId, model.RecordId);

      if (!isAuthorised) return Forbid();

      return await _literatureReviews.SaveForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Advance the stage of the literature review
  /// </summary>
  /// <param name="id">The id of the literature review to advance</param>
  /// <param name="setStage">The stage to advance to</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanAdvanceStages))]
  [HttpPost("{id}/AdvanceStage")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var userId = _users.GetUserId(User);
    if (userId is null) return Forbid();

    var isAuthorised = await _literatureReviews.IsLiteratureReviewOwner(userId, id) ||
                       await _literatureReviews.IsProjectInstructor(userId, id);
    
    if (!isAuthorised) return Forbid();
    
    var nextStage = await _literatureReviews.AdvanceStage(id, userId, setStage.StageName);
    if (nextStage is null)
    {
      return Conflict();
    }
    return Ok(nextStage);
  }
}
