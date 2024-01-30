using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class StageService
{
  private readonly ApplicationDbContext _db;

  public StageService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<Stage> GetNextStage(Stage currentStage, string stageTypes)
  {
    if (currentStage.NextStage == null)
    {
      var nextStage = await _db.Stages
        .Where(x => x.SortOrder == (currentStage.SortOrder + 1))
        .Where(x => x.Type.Value == stageTypes)
        .Include(x => x.NextStage)
        .SingleOrDefaultAsync();

      return nextStage;
    }
    else
      return currentStage.NextStage;
  }

  public async Task<List<string>> GetPlanStagePermissions(Stage stage, string stageTypes)
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
}
