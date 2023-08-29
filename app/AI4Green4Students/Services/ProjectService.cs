using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Models.ProjectGroup;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ProjectService
{
  private readonly ApplicationDbContext _db;
  
  public ProjectService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<ProjectModel>> List()
  {
    var list = await _db.Projects
      .AsNoTracking()
      .Include(x=>x.ProjectGroups)
      .ToListAsync();

    return list.ConvertAll<ProjectModel>(x => new ProjectModel(x));
  }
  
  public async Task<List<ProjectModel>> ListEligible(string userId)
  {
    var eligibleProjectList = await _db.Projects
      .AsNoTracking()
      .Include(x=>x.ProjectGroups)
      .Where(x => x.ProjectGroups.Any(y => 
        y.Students.Any(z => z.Id == userId)))
      .ToListAsync();

    return eligibleProjectList.ConvertAll<ProjectModel>(x => new ProjectModel
    {
      Id = x.Id,
      Name = x.Name,
      ProjectGroups = x.ProjectGroups
        .Where(y => y.Students.Any(z=> z.Id == userId))
        .Select(y => new ProjectGroupModel(y)) // also ensure that only eligible ProjectGroups are included
        .ToList()
    });
  }
  public async Task<ProjectModel> Get(int id)
  {
    var result = await _db.Projects
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .Include(x=>x.ProjectGroups)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    return new ProjectModel(result);
  }
  
  public async Task<ProjectModel> GetEligible(int id, string userId)
  {
    var result = await _db.Projects
                   .AsNoTracking()
                   .Where(x => x.Id == id &&
                               x.ProjectGroups.Any(y =>
                                 y.Students.Any(z => z.Id == userId)))
                   .Include(x => x.ProjectGroups) // include ProjectGroups
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    return new ProjectModel(result);
  }
  public async Task Delete(int id)
  {
    var entity = await _db.Projects
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x=>x.Id == id)
                 ?? throw new KeyNotFoundException();
    
    _db.Projects.Remove(entity);
    await _db.SaveChangesAsync();
  }
  
  public async Task<ProjectModel> Create(CreateProjectModel model)
  {
    var isExistingValue = await _db.Projects
      .AsNoTracking()
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();
    
    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model); // Update existing Project if it exists
    
    // Else, create new Project
    var entity = new Project()
    {
      Name = model.Name,
    };
    
    await _db.Projects.AddAsync(entity);
    await _db.SaveChangesAsync();
    
    return await Get(entity.Id);
  }
  
  public async Task<ProjectModel> Set (int id, CreateProjectModel model)
  {
    var entity = await _db.Projects
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if project does not exist
    
    entity.Name = model.Name;
    
    _db.Projects.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
}
