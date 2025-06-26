namespace AI4Green4Students.Controllers;

using Auth;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Report;
using Models.Section;
using Models.Stage;
using Services;

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
  /// List user's project reports.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>List user's reports.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnExperiments))]
  [HttpGet]
  public async Task<ActionResult<List<ReportModel>>> List(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _reports.ListByUser(id, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get report.
  /// </summary>
  /// <param name="id">Report id.</param>
  /// <returns>Report.</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<ReportModel>> Get(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _reports.IsOwner(userId, id) || await _reports.IsProjectInstructor(userId, id);

      return isAuthorised ? await _reports.Get(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Create a new report.
  /// </summary>
  /// <param name="model">Create model.</param>
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
  /// Delete report.
  /// </summary>
  /// <param name="id">Report id.</param>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null || !await _reports.IsOwner(userId, id))
      {
        return Forbid();
      }

      await _reports.Delete(id, userId);
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
  /// <param name="id">Report id.</param>
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

      var isAuthorised = await _reports.IsOwner(userId, id) || await _reports.IsProjectInstructor(userId, id);

      return isAuthorised ? await _reports.ListSummary(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get section form.
  /// </summary>
  /// <param name="id">Report id.</param>
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

      var isAuthorised = await _reports.IsOwner(userId, id) || await _reports.IsProjectInstructor(userId, id);

      return isAuthorised ? await _reports.GetSectionForm(id, sectionId) : Forbid();
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
  /// <returns>Saved section form data.</returns>
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
                         await _reports.IsOwner(userId, model.RecordId);

      if (!isAuthorised)
      {
        return Forbid();
      }

      return await _reports.SaveSectionForm(model);
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
  [HttpPost("{id}/advance")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isAuthorised = await _reports.IsOwner(userId, id) || await _reports.IsProjectInstructor(userId, id);

    if (!isAuthorised)
    {
      return Forbid();
    }

    try
    {
      await _reports.AdvanceStage(id, userId, setStage.StageName);
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

  /// <summary>
  /// Generate a report export
  /// </summary>
  /// <param name="id">Report id</param>
  /// <returns>File</returns>
  [HttpGet("{id}/GenerateExport")]
  public async Task<ActionResult> GenerateDocument(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _reports.IsOwner(userId, id) || await _reports.IsProjectInstructor(userId, id);

      if (!isAuthorised)
      {
        return Forbid();
      }

      var report = await _reports.Get(id);
      var fileName = $"{report.Title}-{report.OwnerName}.docx";
      var stream = await _reports.GenerateExport(id, report.ProjectId, report.Title ?? string.Empty, report.OwnerName);

      return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
      {
        FileDownloadName = fileName
      };
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
