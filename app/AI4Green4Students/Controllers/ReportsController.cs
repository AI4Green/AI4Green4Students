using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Report;
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
public class ReportsController : ControllerBase
{
  private readonly ReportService _reports;
  private readonly UserManager<ApplicationUser> _users;

  public ReportsController(ReportService reportService, UserManager<ApplicationUser> users)
  {
    _reports = reportService;
    _users = users;
  }

  /// <summary>
  /// Get report list for a given user.
  /// Each report is associated with a project's project group initialised by the Instructor.
  /// </summary>
  /// <param name="projectId">Id of the repoort to get reports for.</param>
  /// <returns>List of reports for the given project.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnExperiments))]
  [HttpGet]
  public async Task<ActionResult<List<ReportModel>>> List(int projectId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _reports.ListByUser(projectId, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get report. Only the owner or instructor can view the report.
  /// </summary>
  /// <param name="reportId">Id of the report.</param>
  /// <returns>Report</returns>
  [HttpGet("{reportId}")]
  public async Task<ActionResult<ReportModel>> Get(int reportId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null && (
        await _reports.IsReportOwner(userId, reportId) ||
        User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)
      )
        ? await _reports.Get(reportId)
        : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Create a new report. 
  /// </summary>
  /// <param name="model">Report dto model. Currently only contains project group id.</param>
  /// <returns>Newly created report.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPost]
  public async Task<ActionResult<ReportModel>> Create(CreateReportModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _reports.Create(userId, model) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Delete report by its id.
  /// </summary>
  /// <param name="id">The id of report to delete.</param>
  /// <returns>If the deletion is successful then no content</returns>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null || !await _reports.IsReportOwner(userId, id)) return Forbid();

      await _reports.Delete(id, userId);
      return NoContent();
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
  /// <param name="sectionTypeId">Id of section type to list sections based on.</param>
  /// <returns>List of report sections with status.</returns>
  [HttpGet("summary/{reportId}/{sectionTypeId}")]
  public async Task<ActionResult<List<SectionSummaryModel>>> ListSummary(int reportId, int sectionTypeId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _reports.IsReportOwner(userId, reportId));

      return isAuthorised ? await _reports.ListSummary(reportId, sectionTypeId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get report section form, which includes section fields and its responses.
  /// </summary>
  /// <param name="sectionId"> Id of section to get form for. </param>
  /// <param name="reportId"> Id of student's literatureReview to get field responses for. </param>
  /// <returns>Literature review section form for the given report matching the given section.</returns> 
  [HttpGet("form/{reportId}/{sectionId}")]
  public async Task<ActionResult<SectionFormModel>> GetSectionForm(int reportId, int sectionId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _reports.IsReportOwner(userId, reportId));

      return isAuthorised ? await _reports.GetSectionForm(reportId, sectionId) : Forbid();
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
  [HttpPut("save-form")]
  [Consumes("multipart/form-data")]
  public async Task<ActionResult<SectionFormModel>> SaveSectionForm([FromForm] SectionFormPayloadModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = userId is not null &&
                         User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments) &&
                         await _reports.IsReportOwner(userId, model.RecordId);

      if (!isAuthorised) return Forbid();

      return await _reports.SaveForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Advance the stage of the report
  /// </summary>
  /// <param name="id">The id of the report to advance</param>
  /// <param name="setStage">The stage to advance to</param>
  /// <returns></returns>
  [HttpPost("{id}/AdvanceStage")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var nextStage = await _reports.AdvanceStage(id, setStage.StageName);
    if (nextStage is null)
    {
      return Conflict();
    }
    return Ok(nextStage);
  }
}
