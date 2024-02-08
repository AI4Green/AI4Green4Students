using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SectionsController : ControllerBase
{
  private readonly SectionService _sections;
  private readonly LiteratureReviewService _literatureReviews;
  private readonly PlanService _plans;
  private readonly UserManager<ApplicationUser> _users;

  public SectionsController(
    SectionService sections, 
    LiteratureReviewService literatureReviewService,
    PlanService plans,
    UserManager<ApplicationUser> users)
  {
    _sections = sections;
    _literatureReviews = literatureReviewService;
    _plans = plans;
    _users = users;
  }
  
  /// <summary>
  /// Get a list of literature review sections including sections status, such as completion status and no. of unread comments.
  /// </summary>
  /// <param name="literatureReviewId">Id of the student's literature review to be used for generating literature review sections status.</param>
  /// <param name="sectionTypeId">
  /// Id of section type to list sections based on.
  /// Ensures that only sections matching the section type are returned.
  /// </param>
  /// <returns>List of literature review sections with status.</returns>
  [HttpGet("ListLiteratureReviewSectionSummaries")]
  public async Task<ActionResult<List<SectionSummaryModel>>> ListLiteratureReviewSectionSummaries(int literatureReviewId, int sectionTypeId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _literatureReviews.IsLiteratureReviewOwner(userId, literatureReviewId));

      return isAuthorised ? await _sections.ListSummariesByLiteratureReview(literatureReviewId, sectionTypeId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a list of plan sections including sections status, such as completion status and no. of unread comments.
  /// </summary>
  /// <param name="planId">Id of the student's plan to be used for generating plan sections status.</param>
  /// <param name="sectionTypeId">
  /// Id of section type to list sections based on.
  /// Ensures that only sections matching the section type are returned.
  /// </param>
  /// <returns>List of plan sections with status.</returns>
  [HttpGet("ListPlanSectionSummaries")]
  public async Task<ActionResult<List<SectionSummaryModel>>> ListPlanSectionSummaries(int planId, int sectionTypeId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _plans.IsPlanOwner(userId, planId));

      return isAuthorised ? await _sections.ListSummariesByPlan(planId, sectionTypeId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a list of report sections including sections status, such as completion status and no. of unread comments.
  /// </summary>
  /// <param name="reportId">Id of the student's report to be used for generating report sections status.</param>
  /// <param name="sectionTypeId">
  /// Id of section type to list sections based on.
  /// Ensures that only sections matching the section type are returned.
  /// </param>
  /// <returns>List of report sections with status.</returns>
  [HttpGet("ListReportSectionSummaries")]
  public async Task<ActionResult<List<SectionSummaryModel>>> ListReportSectionSummaries(int reportId, int sectionTypeId)
  {
    try
    {
      return await _sections.ListSummariesByReport(reportId, sectionTypeId);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  /// <summary>
  /// Get literature review section form, which includes section fields and its responses.
  /// Only instructors or owners can view.
  /// </summary>
  /// <param name="sectionId"> Id of section to get form for. </param>
  /// <param name="literatureReviewId"> Id of student's literatureReview to get field responses for. </param>
  /// <returns>Literature review section form for the given literature review matching the given section.</returns> 
  [HttpGet("GetLiteratureReviewSectionForm")]
  public async Task<ActionResult<SectionFormModel>> GetLiteratureReviewSectionForm(int sectionId, int literatureReviewId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _literatureReviews.IsLiteratureReviewOwner(userId, literatureReviewId));

      return isAuthorised ? await _sections.GetLiteratureReviewFormModel(sectionId, literatureReviewId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get plan section form, which includes section fields and its responses.
  /// Only instructors or owners can view.
  /// </summary>
  /// <param name="sectionId"> Id of section to get form for. </param>
  /// <param name="planId"> Id of student's plan to get field responses for. </param>
  /// <returns>Plan section form for the given plan matching the given section.</returns> 
  [HttpGet("GetPlanSectionForm")]
  public async Task<ActionResult<SectionFormModel>> GetPlanSectionForm(int sectionId, int planId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _plans.IsPlanOwner(userId, planId));

      return isAuthorised ? await _sections.GetPlanFormModel(sectionId, planId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get report section form, which includes section fields and its responses.
  /// Will also need plan id as reports cannot be created without a plan.
  /// Only instructors or owners can view.
  /// </summary>
  /// <param name="sectionId"> Id of section to get form for. </param>
  /// <param name="reportId"> Id of student's report to get field responses for. </param>
  /// <returns>Plan section form for the given plan matching the given section.</returns> 
  [HttpGet("GetReportSectionForm")]
  public async Task<ActionResult<SectionFormModel>> GetReportSectionForm(int sectionId, int reportId)
  {
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments))
        return await _sections.GetReportFormModel(sectionId, reportId);

      return Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
