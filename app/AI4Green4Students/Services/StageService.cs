using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class StageService
{
  private readonly ApplicationDbContext _db;

  public StageService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<Stage?> GetNextStage(Stage currentStage, string stageTypes)
  {
    if (currentStage.NextStage is not null) return currentStage.NextStage;
    
    var nextStage = await _db.Stages
      .Where(x => x.SortOrder == (currentStage.SortOrder + 1))
      .Where(x => x.Type.Value == stageTypes)
      .Include(x => x.NextStage)
      .SingleOrDefaultAsync();

    return nextStage;
  }

  public async Task<List<string>> GetStagePermissions(Stage stage, string stageTypes)
  {
    var proposalStagePermission = await _db.StagePermissions
        .Include(x => x.Type)
        .Where(x => x.Type.Value == stageTypes)
        .Where(x => x.MinStageSortOrder <= stage.SortOrder)
        .Where(x => x.MaxStageSortOrder >= stage.SortOrder)
        .ToListAsync();

    var stagePermission = proposalStagePermission.Select(x => x.Key).ToList();
    return stagePermission;
  }

  /// <summary>
  /// Advance the stage of a section type entity
  /// </summary>
  /// <param name="id">Id of the entity to advance </param>
  /// <param name="stageType">Type of stage to advance. E.g. 'Plan' </param>
  /// <param name="setStage">Stage to advance to. If null, the next stage will be used </param>
  /// <typeparam name="T">Type of entity to advance </typeparam>
  /// <returns>Entity with the updated stage </returns>
  public async Task<T?> AdvanceStage<T>(int id, string stageType, string? setStage = null) where T : CoreSectionTypeData
  {
    IQueryable<T> query = _db.Set<T>()
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .ThenInclude(y => y.NextStage);

    if (typeof(T) == typeof(Plan)) query = query.Include("Note");

    var entity = await query.SingleOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();

    var nextStage = await GetStageToAdvance(entity.Stage, stageType, setStage);
    if (nextStage is null) return null;

    entity.Stage = nextStage;
    await _db.SaveChangesAsync();

    return entity;
  }
  
  /// <summary>
  /// Get the next stage to advance to
  /// </summary>
  /// <param name="currentStage">Current stage </param>
  /// <param name="stageType">Type of stage to advance. E.g. 'Plan' </param>
  /// <param name="setStage">Stage to advance to. If null, the next stage will be used </param>
  /// <returns>Next stage to advance to </returns>
  public async Task<Stage?> GetStageToAdvance(Stage currentStage, string stageType, string? setStage = null)
  {
    if (setStage is null) return await GetNextStage(currentStage, stageType);
    
    var nextStage = await _db.Stages
                      .Where(x => x.DisplayName == setStage && x.Type.Value == stageType)
                      .SingleOrDefaultAsync()
                    ?? throw new Exception("Stage identifier not recognised. Cannot advance to the specified stage");

    return nextStage;
  }

}
