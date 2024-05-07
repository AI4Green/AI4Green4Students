using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.LiteratureReview;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class LiteratureReviewService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;
  private readonly SectionFormService _sectionForm;

  public LiteratureReviewService(ApplicationDbContext db, StageService stages, SectionFormService sectionForm)
  {
    _db = db;
    _stages = stages;
    _sectionForm = sectionForm;
  }

  /// <summary>
  /// Get a list of project literature review for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get literature review for.</param>
  /// <param name="userId">Id of the user to get literature review for.</param>
  /// <returns>List of project literature review of the user.</returns>
  public async Task<List<LiteratureReviewModel>> ListByUser(int projectId, string userId)
  {
    var literatureReviews = await _db.LiteratureReviews
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.Project.Id == projectId)
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .ToListAsync();

    var list = new List<LiteratureReviewModel>();

    foreach (var lr in literatureReviews)
    {
      var permissions = await _stages.GetStagePermissions(lr.Stage, StageTypes.LiteratureReview);
      var model = new LiteratureReviewModel(lr)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list;
  }

  /// <summary>
  /// Get student literature reviews for a project group.
  /// </summary>
  /// <param name="projectGroupId">Project group id.</param>
  /// <returns>List of student literature reviews.</returns>
  public async Task<List<LiteratureReviewModel>> ListByProjectGroup(int projectGroupId)
  {
    var pgStudents = await _db.ProjectGroups
      .AsNoTracking()
      .Include(x => x.Students)
      .Where(x => x.Id == projectGroupId)
      .SelectMany(x => x.Students)
      .ToListAsync();
    
    var literatureReviews = await _db.LiteratureReviews.AsNoTracking()
      .Where(x => pgStudents.Contains(x.Owner))
      .Include(x => x.Owner)
      .Include(x=>x.Stage)
      .ToListAsync();
    
    var list = new List<LiteratureReviewModel>();

    foreach (var lr in literatureReviews)
    {
      var permissions = await _stages.GetStagePermissions(lr.Stage, StageTypes.LiteratureReview);
      var model = new LiteratureReviewModel(lr)
      {
        Permissions = permissions
      };
      list.Add(model);
    }
    return list;
  }

  /// <summary>
  /// Get a literature review by its id.
  /// </summary>
  /// <param name="id">Id of the literature review</param>
  /// <returns>Literature review matching the id.</returns>
  public async Task<LiteratureReviewModel> Get(int id)
  {
    var lr = await _db.LiteratureReviews.AsNoTracking()
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Include(x => x.Owner)
        .Include(x=>x.Stage)
        .SingleOrDefaultAsync()
      ?? throw new KeyNotFoundException();
    
    var permissions = await _stages.GetStagePermissions(lr.Stage, StageTypes.LiteratureReview); 
    return new LiteratureReviewModel(lr)
    {
      Permissions = permissions
    };
  }

  /// <summary>
  /// Create a new literature review.
  /// Before creating a literature review, check if the user is a member of the project group.
  /// </summary>
  /// <param name="ownerId">Id of the user creating the literature review.</param>
  /// <param name="model">Literature review dto model. Currently only contains project group id.</param>
  /// <returns>Newly created literature review.</returns>
  public async Task<LiteratureReviewModel> Create(string ownerId, CreateLiteratureReviewModel model)
  {
    var user = await _db.Users.FindAsync(ownerId)
               ?? throw new KeyNotFoundException();

    var projectGroup = await _db.ProjectGroups
                         .Where(x => x.Id == model.ProjectGroupId && x.Students.Any(y => y.Id == ownerId))
                         .Include(x=>x.Project)
                         .SingleOrDefaultAsync()
                       ?? throw new KeyNotFoundException();
    
    // check if student's have an existing literature review for the project.
    var existing = await _db.LiteratureReviews
      .Where(x => x.Owner.Id == ownerId && x.Project.Id == projectGroup.Project.Id)
      .FirstOrDefaultAsync();

    if (existing is not null) return await Get(existing.Id); // Only one literature review allowed to a user for a project.

    var draftStage = await _db.Stages.SingleAsync(x => x.DisplayName == LiteratureReviewStages.Draft && x.Type.Value == StageTypes.LiteratureReview);
    
    var entity = new LiteratureReview { Owner = user, Project = projectGroup.Project, Stage = draftStage };
    await _db.LiteratureReviews.AddAsync(entity);
    
    entity.FieldResponses = await _sectionForm.CreateFieldResponse(projectGroup.Project.Id, SectionTypes.LiteratureReview, null); // create field responses for the literature review.
    
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Delete literature review by its id.
  /// </summary>
  /// <param name="userId">Id of the user to delete the literature review for.</param>
  /// <param name="id">The id of a literature review to delete.</param>
  /// <returns></returns>
  public async Task Delete(int id, string userId)
  {
    var entity = await _db.LiteratureReviews
                   .Where(x => x.Id == id && x.Owner.Id == userId)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    _db.LiteratureReviews.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Check if a given user is the owner of a given literature review.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="literatureReviewId">Id of the literature review to check the user against.</param>
  /// <returns>True if the user is the owner of the literature review, false otherwise.</returns>
  public async Task<bool> IsLiteratureReviewOwner(string userId, int literatureReviewId)
    => await _db.LiteratureReviews
      .AsNoTracking()
      .AnyAsync(x => x.Id == literatureReviewId && x.Owner.Id == userId);

  public async Task<LiteratureReviewModel?> AdvanceStage(int id, string? setStage = null)
  {
    var entity = await _stages.AdvanceStage<LiteratureReview>(id, StageTypes.LiteratureReview, setStage);
    
    if (entity?.Stage is null) return null;
    
    var stagePermission = await _stages.GetStagePermissions(entity.Stage, StageTypes.LiteratureReview);
    return new LiteratureReviewModel(entity) { Permissions = stagePermission };
  }
  
  /// <summary>
  /// Get section summaries for a given literature review.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="literatureReviewId">Id of the literature review to be used when processing the summaries</param>
  /// <param name="sectionTypeId">Id of the section type.</param>
  /// <returns>Section summaries</returns>
  public async Task<List<SectionSummaryModel>> ListSummary(int literatureReviewId, int sectionTypeId)
  {
    var lr = await Get(literatureReviewId);
    var fieldsResponses = await _sectionForm.ListBySectionType<LiteratureReview>(literatureReviewId);
    return await _sectionForm.GetSummaryModel(sectionTypeId, fieldsResponses, lr.Permissions, lr.Stage);
  }
  
  /// <summary>
  /// Get a literature review section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="literatureReviewId">Id of the literature review to get the field responses for</param>
  /// <returns>Literature review section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int literatureReviewId, int sectionId)
  {
    var fieldsResponses = await _sectionForm.ListBySection<LiteratureReview>(literatureReviewId, sectionId);
    return await _sectionForm.GetFormModel(sectionId, fieldsResponses);
  }
  
  /// <summary>
  /// Save literature review section form. Also creates new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<SectionFormModel> SaveForm(SectionFormPayloadModel model)
  {
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _sectionForm.GenerateFieldResponses(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _sectionForm.GenerateFieldResponses(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses)
    };
    
    var lr = await Get(submission.RecordId);
    var fieldResponses = await _sectionForm.ListBySection<LiteratureReview>(submission.RecordId, submission.SectionId);

    var updatedValues= lr.Stage == LiteratureReviewStages.Draft
      ? _sectionForm.UpdateDraftFieldResponses(submission.FieldResponses, fieldResponses)
      : _sectionForm.UpdateAwaitingChangesFieldResponses(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();

    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId, submission.SectionId);
    
    var entity = await _db.LiteratureReviews.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    entity.FieldResponses = await _sectionForm.CreateFieldResponse(submission.RecordId, SectionTypes.LiteratureReview, submission.NewFieldResponses);

    return await GetSectionForm(model.RecordId, model.SectionId);
  }
}

