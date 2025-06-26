namespace AI4Green4Students.Controllers;

using Constants;
using Data.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Section;
using Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SectionsController : ControllerBase
{
  private readonly AzureStorageService _azureStorage;
  private readonly LiteratureReviewService _literatureReviews;
  private readonly NoteService _notes;
  private readonly PlanService _plans;
  private readonly ProjectGroupService _projectGroups;
  private readonly ReportService _reports;
  private readonly SectionService _sections;
  private readonly UserManager<ApplicationUser> _users;

  public SectionsController(
    SectionService sections,
    LiteratureReviewService literatureReviews,
    PlanService plans,
    NoteService notes,
    ProjectGroupService projectGroups,
    ReportService reports,
    UserManager<ApplicationUser> users,
    AzureStorageService azureStorage)
  {
    _sections = sections;
    _literatureReviews = literatureReviews;
    _plans = plans;
    _notes = notes;
    _projectGroups = projectGroups;
    _reports = reports;
    _users = users;
    _azureStorage = azureStorage;
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
  /// <param name="sectionType">Section type.</param>
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
  /// <param name="sectionId">Section id.</param>
  /// <param name="recordId">Record id.</param>
  /// <param name="fileLocation">Location.</param>
  /// <param name="name">Name.</param>
  /// <returns>File.</returns>
  [HttpGet("File")]
  public async Task<ActionResult> File(int sectionId, int recordId, string fileLocation, string name)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await CanViewRecord(sectionId, recordId, userId);

      if (!isAuthorised)
      {
        return Forbid();
      }

      var file = await _azureStorage.Get(fileLocation);
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
  /// <returns>Result.</returns>
  private async Task<bool> CanViewRecord(int sectionId, int recordId, string userId)
  {
    var section = await _sections.Get(sectionId);

    return section.SectionType.Name switch
    {
      SectionTypes.LiteratureReview => await _literatureReviews.IsOwner(userId, recordId) ||
                                       await _literatureReviews.IsProjectInstructor(userId, recordId) ||
                                       await _literatureReviews.IsInSameProjectGroup(userId, recordId),

      SectionTypes.Plan => await _plans.IsOwner(userId, recordId) ||
                           await _plans.IsProjectInstructor(userId, recordId) ||
                           await _plans.IsInSameProjectGroup(userId, recordId),

      SectionTypes.Note => await _notes.IsOwner(userId, recordId) ||
                           await _notes.IsProjectInstructor(userId, recordId) ||
                           await _notes.IsInSameProjectGroup(userId, recordId),

      SectionTypes.ProjectGroup => await _projectGroups.IsProjectGroupMember(userId, recordId) ||
                                   await _projectGroups.IsPgProjectInstructor(userId, recordId),

      SectionTypes.Report => await _reports.IsOwner(userId, recordId) ||
                             await _reports.IsProjectInstructor(userId, recordId) ||
                             await _reports.IsInSameProjectGroup(userId, recordId),
      _ => false
    };
  }
}
