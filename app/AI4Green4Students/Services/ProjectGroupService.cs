using System.Data;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.ProjectGroup;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ProjectGroupService
{
  private readonly ApplicationDbContext _db;

  public ProjectGroupService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<ProjectGroupModel>> List()
  {
    var list = await _db.ProjectGroups
      .AsNoTracking()
      .Include(x => x.Project)
      .ToListAsync();

    return list.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x));
  }
  
  public async Task<List<ProjectGroupModel>> ListEligible(string userId)
  {
    var list = await _db.ProjectGroups
      .AsNoTracking()
      .Where(x=> x.Students.Any(y=>y.Id==userId))
      .Include(x => x.Project)
      .ToListAsync();

    return list.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x));
  }
  
  public async Task<ProjectGroupModel> Get(int id)
  {
    var result = await _db.ProjectGroups
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .Include(x => x.Project)
                   .Include(x=>x.Students)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ProjectGroupModel(result);
  }
  
  public async Task<ProjectGroupModel> GetEligible(int id, string userId)
  {
    var result = await _db.ProjectGroups
                   .AsNoTracking()
                   .Where(x => x.Id == id && 
                               x.Students.Any(y=>y.Id==userId))
                   .Include(x => x.Project)
                   .Include(x=>x.Students)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ProjectGroupModel(result);
  }

  public async Task Delete(int id)
  {
    var entity = await _db.ProjectGroups
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x=>x.Id == id)
                 ?? throw new KeyNotFoundException();
    
    _db.ProjectGroups.Remove(entity);
    await _db.SaveChangesAsync();
  }
  
  public async Task<ProjectGroupModel> Create(CreateProjectGroupModel model)
  {
    var existingProject = await _db.Projects
                    .Where(x=>x.Id == model.ProjectId)
                    .FirstOrDefaultAsync()
                  ?? throw new KeyNotFoundException();
    
    var existingProjectGroup = existingProject.ProjectGroups
      .FirstOrDefault(y => EF.Functions.ILike(y.Name, model.Name));

    if (existingProjectGroup is not null) 
      throw new InvalidOperationException("Project group name already exist");

    var newProjectGroup = new ProjectGroup { Name = model.Name, Project = existingProject}; // create new ProjectGroup
    
    await _db.ProjectGroups.AddAsync(newProjectGroup); // add ProjectGroup to db
    await _db.SaveChangesAsync();
    
    return await Get(newProjectGroup.Id);
  }
  
  public async Task<ProjectGroupModel> Set (int id, CreateProjectGroupModel model)
  {
    var entity = await _db.ProjectGroups
                   .AsNoTracking()
                   .Where(x=>x.Id == id && x.Project.Id == model.ProjectId)
                   .Include(y => y.Project)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    entity.Name = model.Name;
    
    _db.ProjectGroups.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
}
