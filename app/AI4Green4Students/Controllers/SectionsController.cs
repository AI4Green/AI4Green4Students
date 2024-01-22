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
  private readonly PlanService _plans;
  private readonly UserManager<ApplicationUser> _users;

  public SectionsController(SectionService sections, PlanService plans, UserManager<ApplicationUser> users)
  {
    _sections = sections;
    _plans = plans;
    _users = users;
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
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments))
        return await _sections.ListSummariesByPlan(planId, sectionTypeId);

      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments))
      {
        var userId = _users.GetUserId(User);
        return userId is not null && await _plans.IsPlanOwner(userId, planId)
          ? await _sections.ListSummariesByPlan(planId, sectionTypeId)
          : Forbid();
      }

      return Forbid();
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
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments))
        return await _sections.GetPlanFormModel(sectionId, planId);

      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments))
      {
        var userId = _users.GetUserId(User);
        return userId is not null && await _plans.IsPlanOwner(userId, planId)
          ? await _sections.GetReportFormModel(sectionId, planId)
          : Forbid();
      }

      return Forbid();
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
