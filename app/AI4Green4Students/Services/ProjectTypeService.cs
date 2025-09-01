namespace AI4Green4Students.Services;

using Data;
using Microsoft.EntityFrameworkCore;
using Models.ProjectType;

public class ProjectTypeService
{
  private ApplicationDbContext _db;
  public ProjectTypeService(ApplicationDbContext db) => _db = db;

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
