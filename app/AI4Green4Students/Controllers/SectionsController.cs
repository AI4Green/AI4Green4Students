using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
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
  private readonly NoteService _notes;
  private readonly ProjectGroupService _projectGroups;
  private readonly ReportService _reports;
  private readonly UserManager<ApplicationUser> _users;
  private readonly AZExperimentStorageService _azStorage;

  public SectionsController(
    SectionService sections, 
    LiteratureReviewService literatureReviewService,
    PlanService plans,
    NoteService notes,
    ProjectGroupService projectGroups,
    ReportService reports,
    UserManager<ApplicationUser> users,
    AZExperimentStorageService azStorage)
  {
    _sections = sections;
    _literatureReviews = literatureReviewService;
    _plans = plans;
    _notes = notes;
    _projectGroups = projectGroups;
    _reports = reports;
    _users = users;
    _azStorage = azStorage;
  }
  
  /// <summary>
  /// Get a list of sections based on the section type.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="sectionType">Section type name. e.g. Plan.</param>
  /// <returns>Sections list</returns>
  [HttpGet("ListSectionsBySectionType")]
  public async Task<ActionResult<List<SectionModel>>> ListSectionsBySectionType(int projectId, string sectionType)
  {
    try
    {
      return await _sections.ListBySectionTypeName(sectionType, projectId);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get a file from the storage.
  /// </summary>
  /// <param name="sectionId"></param>
  /// <param name="recordId"></param>
  /// <param name="fileLocation"></param>
  /// <param name="name"></param>
  /// <returns>File</returns>
  [HttpGet("File")]
  public async Task<ActionResult> File(int sectionId, int recordId, string fileLocation, string name)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised =  User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) || 
                          (userId is not null && 
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments) && 
                          await IsRecordOwner(sectionId, recordId, userId));

      if (!isAuthorised) return Forbid();
      
      var file = await _azStorage.Get(fileLocation);
      return File(file, "application/octet-stream", name);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Check if a user is the owner of a record.
  /// </summary>
  /// <param name="sectionId"> Section id to retrieve the section type</param>
  /// <param name="recordId"> Record id to check owner for</param>
  /// <param name="userId"> User id to check if it is the record owner</param>
  /// <returns>Ownership status</returns>
  private async Task<bool> IsRecordOwner (int sectionId, int recordId, string userId)
  {
    var section = await _sections.Get(sectionId);

    return section.SectionType.Name switch
    {
      SectionTypes.LiteratureReview => await _literatureReviews.IsLiteratureReviewOwner(userId, recordId),
      SectionTypes.Plan => await _plans.IsPlanOwner(userId, recordId),
      SectionTypes.Note => await _notes.IsNoteOwner(userId, recordId),
      SectionTypes.ProjectGroup => await _projectGroups.IsProjectGroupMember(userId, recordId),
      SectionTypes.Report => await _reports.IsReportOwner(userId, recordId),
      _ => false
    };
  }
}
