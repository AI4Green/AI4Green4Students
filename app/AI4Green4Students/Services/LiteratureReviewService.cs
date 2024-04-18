using System.Text.Json;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.LiteratureReview;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class LiteratureReviewService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;
  private readonly SectionService _sections;

  public LiteratureReviewService(ApplicationDbContext db, StageService stages, SectionService sections)
  {
    _db = db;
    _stages = stages;
    _sections = sections;
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

    var draftStage = _db.Stages.FirstOrDefault(x => x.DisplayName == LiteratureReviewStages.Draft);
    
    var entity = new LiteratureReview { Owner = user, Project = projectGroup.Project, Stage = draftStage };
    await _db.LiteratureReviews.AddAsync(entity);
    await _db.SaveChangesAsync();

    var lrSectionsFields = await _sections.ListSectionFieldsByType(SectionTypes.LiteratureReview, projectGroup.Project.Id);
    var filteredLrFields = lrSectionsFields
      .Where(x => x.InputType.Name != InputTypes.Content && x.InputType.Name != InputTypes.Header).ToList(); // filter out fields, which doesn't need field responses
    
    await _sections.CreateFieldResponses(entity, filteredLrFields, null); // create field responses for the literature review.

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

  /// <summary>
  /// Get literature review field responses for a given literature review. 
  /// </summary>
  /// <param name="literatureReviewId">Id of the literature review to get field responses for.</param>
  /// <returns> A list of literature review field responses. </returns>
  public async Task<List<FieldResponse>> GetLiteratureReviewFieldResponses(int literatureReviewId)
  {
    var excludedInputTypes = new List<string> { InputTypes.Content, InputTypes.Header };
    return await _db.LiteratureReviews
             .AsNoTracking()
             .Where(x => x.Id == literatureReviewId)
             .SelectMany(x => x.LiteratureReviewFieldResponses
               .Select(y => y.FieldResponse))
             .Where(fr => !excludedInputTypes.Contains(fr.Field.InputType.Name))
             .Include(x => x.FieldResponseValues)
             .Include(x => x.Field)
             .ThenInclude(x => x.Section)
             .Include(x => x.Field)
             .ThenInclude(x => x.TriggerTarget)
             .Include(x => x.Conversation)
             .ToListAsync()
           ?? throw new KeyNotFoundException();
  }

  public async Task<LiteratureReviewModel?> AdvanceStage(int id, string? setStage = null)
  {
    var entity = await _db.LiteratureReviews
        .Include(x => x.Owner)
        .Include(x => x.Stage)
        .ThenInclude(y => y.NextStage)
        .SingleOrDefaultAsync(x => x.Id == id)
        ?? throw new KeyNotFoundException();
    var nextStage = new Stage();
  
  
    if (setStage == null)
    {
      nextStage = await _stages.GetNextStage(entity.Stage, StageTypes.LiteratureReview);
  
      if (nextStage == null)
        return null;
    }
    else
    {
      nextStage = await _db.Stages
      .Where(x => x.DisplayName == setStage)
      .Where(x => x.Type.Value == StageTypes.LiteratureReview)
      .SingleOrDefaultAsync()
      ?? throw new Exception("Stage identifier not recognised. Cannot advance to the specified stage");
    }
    entity.Stage = nextStage;
  
  
    var stagePermission = await _stages.GetStagePermissions(nextStage, StageTypes.LiteratureReview);
  
    await _db.SaveChangesAsync();
    return new(entity)
    {
      Permissions = stagePermission
    };
  }
  
  /// <summary>
  /// Get a list of literature review sections summaries.
  /// Includes each section's status, such as approval status and number of comments.
  /// </summary>
  /// <param name="literatureReviewId">Id of the literature review to be used when processing the summaries</param>
  /// <param name="sectionTypeId">
  /// Id if the section type
  /// Ensures that only sections matching the section type are returned
  /// </param>
  /// <returns>Section summaries list of a literature review</returns>
  public async Task<List<SectionSummaryModel>> ListSummariesByLiteratureReview(int literatureReviewId, int sectionTypeId)
  {
    var sections = await _sections.ListBySectionType(sectionTypeId);
    var literatureReviewFieldResponses = await GetLiteratureReviewFieldResponses(literatureReviewId);
    var lr = await Get(literatureReviewId);
    return _sections.GetSummaryModel(sections, literatureReviewFieldResponses, lr.Permissions, lr.Stage);
  }
  
  /// <summary>
  /// Get a literature review section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="literatureReviewId">Id of the literature review to get the field responses for</param>
  /// <returns>Literature review section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetLiteratureReviewFormModel(int sectionId, int literatureReviewId)
  {
    var section = await _sections.Get(sectionId);
    var sectionFields = await _sections.GetSectionFields(sectionId);
    var literatureReviewFieldResponses = await GetLiteratureReviewFieldResponses(literatureReviewId);
    return _sections.GetFormModel(section, sectionFields, literatureReviewFieldResponses);
  }
  
  /// <summary>
  /// Save literature review section form. Also creates new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<SectionFormModel> SaveLiteratureReview(SectionFormSubmissionModel model)
  {
    var stage = _db.LiteratureReviews.AsNoTracking()
      .Where(x => x.Id == model.RecordId)
      .Include(x => x.Stage).Single()
      .Stage;

    var selectedFieldResponses = await _sections.GetSectionFieldResponses(model.SectionId, model.RecordId);
    
    if (stage.DisplayName == LiteratureReviewStages.Draft)
    {
      _sections.UpdateDraftFieldResponses(model, selectedFieldResponses);
    }
    else if(stage.DisplayName == LiteratureReviewStages.AwaitingChanges)
    {
      _sections.UpdateAwaitingChangesFieldResponses(model, selectedFieldResponses);
    }

    await _db.SaveChangesAsync();
    
    if (model.NewFieldResponses.Count != 0)
    {
      var fields = await _sections.GetSectionFields(model.SectionId);
      var selectedFields = fields.Where(x => model.NewFieldResponses.Any(y=>y.Id == x.Id)).ToList();
      var lr = await _db.LiteratureReviews.FindAsync(model.RecordId) ?? throw new KeyNotFoundException();
      await _sections.CreateFieldResponses(lr, selectedFields, model.NewFieldResponses);
    }
    
    return await GetLiteratureReviewFormModel(model.SectionId, model.RecordId);
  }
}

