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
  private readonly ProjectGroupService _projectGroups;
  private readonly UserManager<ApplicationUser> _users;
  private readonly AZExperimentStorageService _azStorage;

  public SectionsController(
    SectionService sections, 
    LiteratureReviewService literatureReviewService,
    PlanService plans,
    ProjectGroupService projectGroups,
    UserManager<ApplicationUser> users,
    AZExperimentStorageService azStorage)
  {
    _sections = sections;
    _literatureReviews = literatureReviewService;
    _plans = plans;
    _projectGroups = projectGroups;
    _users = users;
    _azStorage = azStorage;
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

      return isAuthorised ? await _literatureReviews.ListSummariesByLiteratureReview(literatureReviewId, sectionTypeId) : Forbid();
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

      return isAuthorised ? await _plans.ListSummariesByPlan(planId, sectionTypeId) : Forbid();
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

      return isAuthorised ? await _literatureReviews.GetLiteratureReviewFormModel(sectionId, literatureReviewId) : Forbid();
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

      return isAuthorised ? await _plans.GetPlanFormModel(sectionId, planId) : Forbid();
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

      return isAuthorised ? await _projectGroups.GetProjectGroupFormModel(projectGroupId, sectionTypeId) : Forbid();
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
      
      var submission = new SectionFormSubmissionModel
      {
        SectionId = model.SectionId,
        RecordId = model.RecordId,
        FieldResponses = await _sections.GetFieldResponses(model.FieldResponses, model.Files, model.FileFieldResponses),
        NewFieldResponses = await _sections.GetFieldResponses(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses)
      };

      var section = await _sections.Get(model.SectionId);
      
      return section.SectionType.Name switch
      {
        SectionTypes.LiteratureReview => await _literatureReviews.SaveLiteratureReview(submission),
        SectionTypes.Plan => await _plans.SavePlan(submission),
        SectionTypes.ProjectGroup => await _projectGroups.SaveProjectGroupSection(submission),
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
      SectionTypes.ProjectGroup => await _projectGroups.IsProjectGroupMember(userId, recordId),
      _ => false
    };
  }
}
