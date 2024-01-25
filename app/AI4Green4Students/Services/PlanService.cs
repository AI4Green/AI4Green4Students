using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Plan;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class PlanService
{
  private readonly ApplicationDbContext _db;

  public PlanService(ApplicationDbContext db)
  {
    _db = db;
  }

  /// <summary>
  /// Get a list of project plans for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get plans for.</param>
  /// <param name="userId">Id of the user to get plans for.</param>
  /// <returns>List of project plans of the user.</returns>
  public async Task<List<PlanModel>> ListByUser(int projectId, string userId)
    => await _db.Plans.AsNoTracking()
      .AsNoTracking()
      .Where(x => x.Owner.Id == userId && x.ProjectGroup.Project.Id == projectId)
      .Include(x => x.Owner)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .Select(x => new PlanModel(x)).ToListAsync();

  /// <summary>
  /// Get a list of plans matching a given project group.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to check plans for.</param>
  /// <returns>List of project plans for given project group.</returns>
  public async Task<List<PlanModel>> ListByProjectGroup(int projectGroupId)
  {
    return await _db.Plans.AsNoTracking()
      .Where(x => x.ProjectGroup.Id == projectGroupId)
      .Include(x => x.Owner)
      .Include(x => x.ProjectGroup)
      .ThenInclude(x => x.Project)
      .Select(x => new PlanModel(x))
      .ToListAsync();
  }

  /// <summary>
  /// Get a plan by its id.
  /// </summary>
  /// <param name="id">Id of the plan</param>
  /// <returns>Plan matching the id.</returns>
  public async Task<PlanModel> Get(int id)
    => await _db.Plans.AsNoTracking()
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.Owner)
         .Include(x => x.ProjectGroup)
         .ThenInclude(x => x.Project)
         .Select(x => new PlanModel(x)).SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Create a new plan.
  /// Before creating a plan, check if the user is a member of the project group.
  /// </summary>
  /// <param name="ownerId">Id of the user creating the plan.</param>
  /// <param name="model">Plan dto model. Currently only contains project group id.</param>
  /// <returns>Newly created plan.</returns>
  public async Task<PlanModel> Create(string ownerId, CreatePlanModel model)
  {
    var user = await _db.Users.FindAsync(ownerId)
               ?? throw new KeyNotFoundException();

    var projectGroup = await _db.ProjectGroups
                         .Where(x => x.Id == model.ProjectGroupId && x.Students.Any(y => y.Id == ownerId))
                         .SingleOrDefaultAsync()
                       ?? throw new KeyNotFoundException();

    var draftStage = _db.Stages.FirstOrDefault(x => x.DisplayName == PlanStages.Draft);


    var entity = new Plan { Owner = user, ProjectGroup = projectGroup, Stage = draftStage };
    await _db.Plans.AddAsync(entity);
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Delete plan by its id.
  /// </summary>
  /// <param name="userId">Id of the user to delete the plan for.</param>
  /// <param name="id">The id of an experiment reaction to delete.</param>
  /// <returns></returns>
  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Plans
                   .Where(x => x.Id == id && x.Owner.Id == userId)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    _db.Plans.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Check if a given user is the owner of a given plan.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="planId">Id of the plan to check the user against.</param>
  /// <returns>True if the user is the owner of the plan, false otherwise.</returns>
  public async Task<bool> IsPlanOwner(string userId, int planId)
    => await _db.Plans
      .AsNoTracking()
      .AnyAsync(x => x.Id == planId && x.Owner.Id == userId);

  /// <summary>
  /// Get plan field responses for a given plan. 
  /// </summary>
  /// <param name="planId">Id of the plan to get field responses for.</param>
  /// <returns> A list of plan field responses. </returns>
  public async Task<List<FieldResponse>> GetPlanFieldResponses(int planId)
    => await _db.Plans
         .AsNoTracking()
         .Where(x => x.Id == planId)
         .SelectMany(x => x.PlanFieldResponses
           .Select(y => y.FieldResponse))
         .Include(x => x.FieldResponseValues)
         .Include(x => x.Field)
         .ThenInclude(x => x.Section)
         .Include(x => x.Conversation)
         .ToListAsync()
       ?? throw new KeyNotFoundException();

public async Task<PlanModel?> AdvanceStage(int id, string? setStage = null)
{
  var entity = await _db.Plans
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .ThenInclude(y => y.NextStage)
      .SingleOrDefaultAsync(x => x.Id == id)
      ?? throw new KeyNotFoundException();
  var nextStage = new Stage();


  if (setStage == null)
  {
    nextStage = await GetNextStage(entity.Stage);

    if (nextStage == null)
      return null;
  }
  else
  {
    nextStage = await _db.Stages
    .Where(x => x.DisplayName == setStage)
    .Where(x => x.Type.Value == StageTypes.Plan)
    .SingleOrDefaultAsync()
    ?? throw new Exception("Stage identifier not recognised. Cannot advance to the specified stage");
  }
  entity.Stage = nextStage;


  var stagePermission = await GetPlanStagePermissions(nextStage);

  await _db.SaveChangesAsync();
  return new(entity)
  {
    Permissions = stagePermission
  };
}

  private async Task<Stage> GetNextStage(Stage currentStage)
  {
    if (currentStage.NextStage == null)
    {
      var nextStage = await _db.Stages
        .Where(x => x.SortOrder == (currentStage.SortOrder + 1))
        .Where(x => x.Type.Value == StageTypes.Plan)
        .Include(x => x.NextStage)
        .SingleOrDefaultAsync();

      return nextStage;
    }
    else
      return currentStage.NextStage;
  }

  private async Task<List<string>> GetPlanStagePermissions(Stage stage)
  {
    var proposalStagePermission = await _db.StagePermissions
        .Include(x => x.Type)
        .Where(x => x.Type.Value == StageTypes.Plan)
        .Where(x => x.MinStageSortOrder <= stage.SortOrder)
        .Where(x => x.MaxStageSortOrder >= stage.SortOrder)
        .ToListAsync();

    var stagePermission = proposalStagePermission.Select(x => x.Key).ToList();
    return stagePermission;
  }
}
