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
  private readonly AzureStorageService _azureStorageService;

  public SectionsController(
    SectionService sections, 
    LiteratureReviewService literatureReviewService,
    PlanService plans,
    NoteService notes,
    ProjectGroupService projectGroups,
    ReportService reports,
    UserManager<ApplicationUser> users,
    AzureStorageService azureStorageService)
  {
    _sections = sections;
    _literatureReviews = literatureReviewService;
    _plans = plans;
    _notes = notes;
    _projectGroups = projectGroups;
    _reports = reports;
    _users = users;
    _azureStorageService = azureStorageService;
  }
  
  /// <summary>
  /// List sections based on the project.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <returns>Sections list.</returns>
  [HttpGet("ListSectionsByProject")]
  public async Task<ActionResult<List<SectionModel>>> ListSectionsByProject(int projectId)
  {
    try
    {
      return await _sections.ListByProject(projectId);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// List sections based on the section type.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="sectionType">Section type name. e.g. Plan.</param>
  /// <returns>Sections list.</returns>
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
  /// <returns>File.</returns>
  [HttpGet("File")]
  public async Task<ActionResult> File(int sectionId, int recordId, string fileLocation, string name)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();
      
      var isAuthorised = await CanViewRecord(sectionId, recordId, userId);

      if (!isAuthorised) return Forbid();
      
      var file = await _azureStorageService.Get(fileLocation);
      return File(file, "application/octet-stream", name);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Check if user can view the record.
  /// </summary>
  /// <param name="sectionId">Section id to retrieve the section type.</param>
  /// <param name="recordId">Record id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>True if user can view the record, otherwise false.</returns>
  private async Task<bool> CanViewRecord (int sectionId, int recordId, string userId)
  {
    var section = await _sections.Get(sectionId);

    return section.SectionType.Name switch
    {
      SectionTypes.LiteratureReview => await _literatureReviews.IsLiteratureReviewOwner(userId, recordId) ||
                                       await _literatureReviews.IsProjectInstructor(userId, recordId) ||
                                       await _literatureReviews.IsInSameProjectGroup(userId, recordId),
      
      SectionTypes.Plan => await _plans.IsPlanOwner(userId, recordId) ||
                          await _plans.IsProjectInstructor(userId, recordId) ||
                          await _plans.IsInSameProjectGroup(userId, recordId),
      
      SectionTypes.Note => await _notes.IsNoteOwner(userId, recordId) ||
                          await _notes.IsProjectInstructor(userId, recordId) ||
                          await _notes.IsInSameProjectGroup(userId, recordId),
      
      SectionTypes.ProjectGroup => await _projectGroups.IsProjectGroupMember(userId, recordId) ||
                                   await _projectGroups.IsPgProjectInstructor(userId, recordId),
      
      SectionTypes.Report => await _reports.IsReportOwner(userId, recordId) ||
                             await _reports.IsProjectInstructor(userId, recordId) ||
                             await _reports.IsInSameProjectGroup(userId, recordId),
      _ => false
    };
  }
}
