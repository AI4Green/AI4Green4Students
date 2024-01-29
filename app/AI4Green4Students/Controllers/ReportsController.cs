using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Report;
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
  /// Get report list for a given project group.
  /// Project groups consist of students allocated by the Instructor.
  /// Allows instructors to view all students reports for a given project group.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to get reports for.</param>
  /// <returns>List of reports for the given project group.</returns>
  [Authorize(nameof(AuthPolicies.CanViewAllExperiments))]
  [HttpGet("ListProjectGroupReports")]
  public async Task<ActionResult<List<ReportModel>>> ListProjectGroupReports(int projectGroupId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _reports.ListByProjectGroup(projectGroupId) : Forbid();
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
  /// Advance the stage of the report
  /// </summary>
  /// <param name="id">The id of the report to advance</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanAdvanceStages))]
  [HttpPost("{id}/AdvanceStage")]
  public async Task<ActionResult> AdvanceStage(int id)
  {
    var nextStage = await _reports.AdvanceStage(id);
    if (nextStage is null)
    {
      return Conflict();
    }
    return Ok(nextStage);
  }
}
