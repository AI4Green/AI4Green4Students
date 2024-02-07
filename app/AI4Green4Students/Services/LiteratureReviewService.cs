using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.LiteratureReview;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class LiteratureReviewService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;

  public LiteratureReviewService(ApplicationDbContext db, StageService stages)
  {
    _db = db;
    _stages = stages;
  }

  /// <summary>
  /// Get a list of project literature review for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get literature review for.</param>
  /// <param name="userId">Id of the user to get literature review for.</param>
  /// <returns>List of project literature review of the user.</returns>
  public async Task<List<LiteratureReviewModel>> ListByUser(int projectId, string userId)
    => await _db.LiteratureReviews.AsNoTracking()
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.Project.Id == projectId)
      .Include(x => x.Owner)
      .Include(x=>x.Stage)
      .Select(x => new LiteratureReviewModel(x)).ToListAsync();

  /// <summary>
  /// Get a list of literature review matching a given project group.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to check literature review for.</param>
  /// <returns>List of project literature review for given project group.</returns>
  public async Task<List<LiteratureReviewModel>> ListByProjectGroup(int projectGroupId)
  {
    return await _db.LiteratureReviews.AsNoTracking()
      .Where(x => x.Project.ProjectGroups.Any(y => y.Id == projectGroupId))
      .Include(x => x.Owner)
      .Include(x=>x.Stage)
      .Select(x => new LiteratureReviewModel(x))
      .ToListAsync();
  }

  /// <summary>
  /// Get a literature review by its id.
  /// </summary>
  /// <param name="id">Id of the literature review</param>
  /// <returns>Literature review matching the id.</returns>
  public async Task<LiteratureReviewModel> Get(int id)
    => await _db.LiteratureReviews.AsNoTracking()
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.Owner)
         .Include(x=>x.Stage)
         .Select(x => new LiteratureReviewModel(x)).SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

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
    => await _db.LiteratureReviews
         .AsNoTracking()
         .Where(x => x.Id == literatureReviewId)
         .SelectMany(x => x.LiteratureReviewFieldResponses
           .Select(y => y.FieldResponse))
         .Include(x => x.FieldResponseValues)
         .Include(x => x.Field)
         .ThenInclude(x => x.Section)
         .Include(x => x.Conversation)
         .ToListAsync()
       ?? throw new KeyNotFoundException();

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
}

