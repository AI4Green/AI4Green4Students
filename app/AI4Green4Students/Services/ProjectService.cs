using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Models.ProjectGroup;
using AI4Green4Students.Models.SectionType;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ProjectService
{
  private readonly ApplicationDbContext _db;
  private readonly PlanService _plans;
  
  public ProjectService(ApplicationDbContext db, PlanService plans)
  {
    _db = db;
    _plans = plans;
  }

  public async Task<List<ProjectModel>> ListAll()
  {
    var list = await _db.Projects
      .AsNoTracking()
      .Include(x=>x.ProjectGroups)
      .Include(x=>x.Sections)
      .ThenInclude(y=>y.SectionType)
      .ToListAsync();

    return list.ConvertAll<ProjectModel>(x => new ProjectModel(x));
  }
  
  public async Task<List<ProjectModel>> ListByUser(string userId)
  {
    var eligibleProjectList = await _db.Projects
      .AsNoTracking()
      .Where(x => x.ProjectGroups.Any(y => 
        y.Students.Any(z => z.Id == userId)))
      .Include(x=>x.ProjectGroups)
      .ThenInclude(y=>y.Students)
      .Include(x=>x.Sections)
      .ThenInclude(y=>y.SectionType)
      .ToListAsync();
    
    
    return eligibleProjectList.ConvertAll<ProjectModel>(x => new ProjectModel
    {
      Id = x.Id,
      Name = x.Name,
      ProjectGroups = x.ProjectGroups
        .Where(y => y.Students.Any(z=> z.Id == userId))
        .Select(y => new ProjectGroupModel
        {
          Id = y.Id,
          Name = y.Name,
          ProjectId = y.Project.Id
        }) 
        .ToList(),
      SectionTypes = new ProjectSectionTypeModel(x.Sections.ConvertAll<SectionTypeModel>(z => new SectionTypeModel(z.SectionType)))
    });
  }
  public async Task<ProjectModel> Get(int id)
  {
    var result = await _db.Projects
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .Include(x=>x.ProjectGroups)
                   .Include(x=>x.Sections)
                   .ThenInclude(y=>y.SectionType)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    return new ProjectModel(result);
  }
  
  public async Task<ProjectModel> GetByUser(int id, string userId)
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
                   .FirstOrDefaultAsync(x=>x.Id == id)
                 ?? throw new KeyNotFoundException();
    
    _db.Projects.Remove(entity);
    await _db.SaveChangesAsync();
  }
  
  public async Task<ProjectModel> Create(CreateProjectModel model)
  {
    var isExistingValue = await _db.Projects
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();
    
    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model); // Update existing Project if it exists
    
    // Else, create new Project
    var entity = new Project()
    {
      Name = model.Name,
      StartDate = model.ParseDateOrDefault(model.StartDate),
      PlanningDeadline = model.ParseDateOrDefault(model.PlanningDeadline),
      ExperimentDeadline = model.ParseDateOrDefault(model.ExperimentDeadline)
    };
    
    await _db.Projects.AddAsync(entity);
    await _db.SaveChangesAsync();
    
    return await Get(entity.Id);
  }
  
  public async Task<ProjectModel> Set (int id, CreateProjectModel model)
  {
    var entity = await _db.Projects
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if project does not exist
    
    entity.Name = model.Name;
    entity.StartDate = model.ParseDateOrDefault(model.StartDate);
    entity.PlanningDeadline = model.ParseDateOrDefault(model.PlanningDeadline);
    entity.ExperimentDeadline = model.ParseDateOrDefault(model.ExperimentDeadline);
    
    _db.Projects.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
  
  public async Task<ProjectSummaryModel> GetStudentProjectSummary(int projectId, string userId)
  {
    var project = await _db.Projects
                    .AsNoTracking()
                    .Where(x => x.Id == projectId)
                    .Include(x => x.ProjectGroups)
                    .ThenInclude(x => x.Students)
                    .SingleOrDefaultAsync()
                  ?? throw new KeyNotFoundException();
    
    var projectGroup = project.ProjectGroups
      .FirstOrDefault(x => x.Students.Any(y => y.Id == userId));
    if (projectGroup is null) throw new KeyNotFoundException();

    var plans = await _plans.ListByUser(projectId, userId);
    
    return new ProjectSummaryModel
    {
      ProjectId = project.Id,
      ProjectName = project.Name,
      ProjectGroupId = projectGroup.Id,
      ProjectGroupName = projectGroup.Name,
      Plans = plans
    };
  }

  public async Task<ProjectSummaryModel> GetProjectGroupProjectSummary(int projectGroupId)
  {
    var projectGroup = await _db.ProjectGroups
                         .AsNoTracking()
                         .Where(x => x.Id == projectGroupId)
                         .Include(x => x.Project)
                         .SingleOrDefaultAsync()
                       ?? throw new KeyNotFoundException();
    var project = projectGroup.Project;
    
    var plans = await _plans.ListByProjectGroup(projectGroupId);
    
    return new ProjectSummaryModel
    {
      ProjectId = project.Id,
      ProjectName = project.Name,
      ProjectGroupId = projectGroup.Id,
      ProjectGroupName = projectGroup.Name,
      Plans = plans
    };
  }
}
