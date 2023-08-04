using AI4Green4Students.Data;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class FeatureFlagService
{
  private readonly ApplicationDbContext _db;

  public FeatureFlagService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<Dictionary<string, bool>> List()
  {
    var list = await _db.FeatureFlags
      .AsNoTracking()
      .ToListAsync();
    return list.ToDictionary(x => x.Key, x => x.isActive);
  }



}
