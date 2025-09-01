namespace AI4Green4Students.Services;

using Data;
using Microsoft.EntityFrameworkCore;
using Models.ProjectType;

public class ProjectTypeService
{
  private ApplicationDbContext _db;
  public ProjectTypeService(ApplicationDbContext db) => _db = db;

  /// <summary>
  /// List all project types with project counts.
  /// </summary>
  /// <returns>Project types.</returns>
  public async Task<List<ProjectTypeModel>> List()
  {
    var projectCounts = await _db.Projects
      .GroupBy(x => x.ProjectType.Id)
      .Select(x => new
      {
        ProjectTypeId = x.Key, Count = x.Count()
      })
      .ToDictionaryAsync(x => x.ProjectTypeId, x => x.Count);

    return await _db.ProjectTypes.AsNoTracking()
      .Include(x => x.Stage)
      .Select(x => new ProjectTypeModel(x, projectCounts.ContainsKey(x.Id), projectCounts.GetValueOrDefault(x.Id, 0)))
      .ToListAsync();
  }

  /// <summary>
  /// Get a project type with project count.
  /// </summary>
  /// <param name="id">Project type id.</param>
  /// <returns>Project type.</returns>
  public async Task<ProjectTypeModel> Get(int id)
  {
    var projectType = await _db.ProjectTypes.AsNoTracking()
                        .Where(x => x.Id == id)
                        .Include(x => x.Stage)
                        .FirstOrDefaultAsync()
                      ?? throw new KeyNotFoundException("Project type not found.");

    var projectCount = await _db.Projects.CountAsync(p => p.ProjectType.Id == id);

    return new ProjectTypeModel(projectType, projectCount > 0, projectCount);
  }
  public async Task<List<ProjectTypeModel>> List()
    => await _db.ProjectTypes
      .AsNoTracking()
      .Include(x => x.Stage)
      .Select(x => new ProjectTypeModel(x))
      .ToListAsync();

  public async Task<ProjectTypeModel> Get(int id)
    => await _db.ProjectTypes.AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.Stage)
         .Select(x => new ProjectTypeModel(x))
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException("Project type not found.");
}
