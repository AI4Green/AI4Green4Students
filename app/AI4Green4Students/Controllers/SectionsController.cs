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
  private readonly UserManager<ApplicationUser> _users;
  private readonly AZExperimentStorageService _azStorage;

  public SectionsController(
    SectionService sections, 
    LiteratureReviewService literatureReviewService,
    PlanService plans,
    NoteService notes,
    ProjectGroupService projectGroups,
    UserManager<ApplicationUser> users,
    AZExperimentStorageService azStorage)
  {
    _sections = sections;
    _literatureReviews = literatureReviewService;
    _plans = plans;
    _notes = notes;
    _projectGroups = projectGroups;
    _users = users;
    _azStorage = azStorage;
  }
  
  /// <summary>
  /// Get a list of sections based on the section type.
  /// </summary>
  /// <param name="sectionTypeId"></param>
  /// <returns>Sections list</returns>
  [HttpGet("ListSectionsBySectionType")]
  public async Task<ActionResult<List<SectionModel>>> ListSectionsBySectionType(int sectionTypeId)
  {
    try
    {
      return await _sections.ListBySectionType(sectionTypeId);
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

      return isAuthorised ? await _literatureReviews.ListSummary(literatureReviewId, sectionTypeId) : Forbid();
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

      return isAuthorised ? await _plans.ListSummary(planId, sectionTypeId) : Forbid();
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
                          await IsRecordOwner(sectionId, literatureReviewId, userId));

      return isAuthorised ? await _literatureReviews.GetSectionForm(literatureReviewId, sectionId) : Forbid();
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
                          await IsRecordOwner(sectionId, planId, userId));

      return isAuthorised ? await _plans.GetSectionForm(planId, sectionId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get note section form, which includes section fields and its responses.
  /// Only instructors or plan owners can view.
  /// </summary>
  /// <param name="sectionId"> Id of section to get form for. </param>
  /// <param name="noteId"> Id of student's note to get field responses for. </param>
  /// <returns>Note section form for the given note matching the given section.</returns> 
  [HttpGet("GetNoteSectionForm")]
  public async Task<ActionResult<SectionFormModel>> GetNoteSectionForm(int sectionId, int noteId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await IsRecordOwner(sectionId, noteId, userId));

      return isAuthorised ? await _notes.GetSectionForm(noteId, sectionId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get project group section form, which includes section fields and its responses.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to get the field responses for</param>
  /// <param name="sectionTypeId"> Id of the section type</param>
  /// <returns>Project group section form.</returns> 
  [HttpGet("GetProjectGroupSectionForm")]
  public async Task<ActionResult<SectionFormModel>> GetProjectGroupSectionForm(int projectGroupId, int sectionTypeId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _projectGroups.IsProjectGroupMember(userId, projectGroupId));

      return isAuthorised ? await _projectGroups.GetSectionForm(projectGroupId, sectionTypeId) : Forbid();
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
  [HttpPut("SaveSection")]
  [Consumes("multipart/form-data")]
  public async Task<ActionResult<SectionFormModel>> SaveSectionForm([FromForm] SectionFormPayloadModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = userId is not null &&
                         User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments) &&
                         await IsRecordOwner(model.SectionId,model.RecordId, userId);

      if (!isAuthorised) return Forbid();

      var section = await _sections.Get(model.SectionId);
      return section.SectionType.Name switch
      {
        SectionTypes.LiteratureReview => await _literatureReviews.SaveForm(model),
        SectionTypes.Plan => await _plans.SaveForm(model),
        SectionTypes.Note => await _notes.SaveForm(model),
        SectionTypes.ProjectGroup => await _projectGroups.SaveForm(model),
        _ => Forbid()
      };
    }
    catch(KeyNotFoundException) 
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
      _ => false
    };
  }
}
