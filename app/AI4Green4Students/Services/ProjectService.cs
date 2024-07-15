using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Models.ProjectGroup;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ProjectService
{
  private readonly ApplicationDbContext _db;
  private readonly LiteratureReviewService _literatureReviews;
  private readonly PlanService _plans;
  private readonly ReportService _reports;
  
  public ProjectService(ApplicationDbContext db, LiteratureReviewService literatureReviews, PlanService plans, ReportService reports)
  {
    _db = db;
    _literatureReviews = literatureReviews;
    _plans = plans;
    _reports = reports;
  }

  /// <summary>
  /// List all projects
  /// </summary>
  /// <returns>Project list</returns>
  public async Task<List<ProjectModel>> ListAll()
  {
    var projects = await ProjectsQuery().AsNoTracking().ToListAsync();

    var list = new List<ProjectModel>();
    foreach (var project in projects)
    {
      list.Add(new ProjectModel(project)
      {
        ProjectGroups = project.ProjectGroups.Select(x => new ProjectGroupModel { Id = x.Id, Name = x.Name }).ToList(),
        Stage = await Status(project.Id)
      });
    }

    return list;
  }

  /// <summary>
  /// List user's projects
  /// </summary>
  /// <param name="userId">User id to get projects for</param>
  /// <returns>Project list</returns>
  public async Task<List<ProjectModel>> ListByUser(string userId)
  {
    var userProjects = await ProjectsQuery().AsNoTracking()
      .Where(x => x.ProjectGroups.Any(y => y.Students.Any(z => z.Id == userId)))
      .Include(x => x.ProjectGroups)
      .ThenInclude(y => y.Students)
      .ToListAsync();
    
    var list = new List<ProjectModel>();
    foreach (var project in userProjects)
    {
      list.Add(new ProjectModel(project)
      {
        ProjectGroups = project.ProjectGroups.Where(pg => pg.Students.Any(s => s.Id == userId))
          .Select(x => new ProjectGroupModel
          {
            Id = x.Id, 
            Name = x.Name
          }).ToList(),
        Stage = await Status(project.Id, userId)
      });
    }

    return list;
  }

  /// <summary>
  /// Get project based on project id.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>Project matching the id.</returns>
  public async Task<ProjectModel> Get(int id)
  {
    var project = await ProjectsQuery().AsNoTracking().Where(x => x.Id == id).SingleAsync() ?? throw new KeyNotFoundException();
    
    return new ProjectModel(project)
    {
      ProjectGroups = project.ProjectGroups.Select(x => new ProjectGroupModel { Id = x.Id, Name = x.Name }).ToList(),
      Stage = await Status(project.Id)
    };
  }
  
  /// <summary>
  /// Get user's project based on project id.
  /// </summary>
  /// <param name="id">Project id. </param>
  /// <param name="userId">Users id </param>
  /// <returns>Project matching the id and user id. </returns>
  public async Task<ProjectModel> GetByUser(int id, string userId)
  {
    var result = await _db.Projects.AsNoTracking()
                   .Where(x => x.Id == id && x.ProjectGroups.Any(y => y.Students.Any(z => z.Id == userId)))
                   .Include(x=>x.Sections)
                   .ThenInclude(y=>y.SectionType)
                   .Select(x => new
                   {
                     Project = x,
                     ProjectGroups = x.ProjectGroups
                       .Where(pg => pg.Students.Any(s => s.Id == userId))
                       .ToList()
                   })
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    var projectModel = new ProjectModel(result.Project)
    {
      ProjectGroups = result.ProjectGroups.Select(pg => new ProjectGroupModel { Id = pg.Id, Name = pg.Name }).ToList(),
      Stage = await Status(result.Project.Id, userId)
    };
    
    return projectModel;
  }
  
  /// <summary>
  /// Delete the project.
  /// </summary>
  /// <param name="id">Project id to delete</param>
  public async Task Delete(int id)
  {
    var entity = await _db.Projects.FirstOrDefaultAsync(x=>x.Id == id) ?? throw new KeyNotFoundException();
    
    _db.Projects.Remove(entity);
    await _db.SaveChangesAsync();
  }
  
  /// <summary>
  /// Create project.
  /// </summary>
  /// <param name="model">Model for creating project</param>
  /// <returns>Create project using the model</returns>
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
  
  /// <summary>
  /// Update project.
  /// </summary>
  /// <param name="id">Project id to update</param>
  /// <param name="model">Model for updating project</param>
  /// <returns>Updated Project</returns>
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
  
  public async Task<ProjectSummaryModel> GetStudentProjectSummary(int projectId, string userId, bool filterDrafts = false)
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

    var literatureReviews = await _literatureReviews.ListByUser(projectId, userId);
    var plans = await _plans.ListByUser(projectId, userId);
    var reports = await _reports.ListByUser(projectId, userId);

    var owner = project.ProjectGroups.SelectMany(x => x.Students).First(y => y.Id == userId)
                ?? throw new KeyNotFoundException();
    
    return new ProjectSummaryModel(
      filterDrafts ? literatureReviews.Where(x => x.Stage != Stages.Draft).ToList() : literatureReviews,
      filterDrafts ? plans.Where(x => x.Stage != Stages.Draft).ToList() : plans,
      reports,
      new ProjectSummaryProjectModel(project.Id, project.Name),
      new ProjectSummaryProjectGroupModel(projectGroup.Id, projectGroup.Name),
      new ProjectSummaryAuthorModel(owner.Id, owner.FullName)
    );
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

    var literatureReviews = await _literatureReviews.ListByProjectGroup(projectGroupId);
    var plans = await _plans.ListByProjectGroup(projectGroupId);
    var reports = await _reports.ListByProjectGroup(projectGroupId);
    
    // Filter out drafts
    return new ProjectSummaryModel(
      literatureReviews.Where(x => x.Stage != Stages.Draft).ToList(),
      plans.Where(x => x.Stage != Stages.Draft).ToList(),
      reports,
      new ProjectSummaryProjectModel(project.Id, project.Name),
      new ProjectSummaryProjectGroupModel(projectGroup.Id, projectGroup.Name),
      null
    );
  }
  
  /// <summary>
  /// Construct a query to fetch Project along with its related entities.
  /// </summary>
  /// <returns>An IQueryable of Project entities.</returns>
  private IQueryable<Project> ProjectsQuery()
  {
    return _db.Projects
      .Include(x=>x.ProjectGroups)
      .Include(x=>x.Sections)
      .ThenInclude(y=>y.SectionType);
  }

  /// <summary>
  /// Get Project status based on report submission for the project.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="userId">User id. If provided, status is based on user's submission.</param>
  /// <returns>Project status.</returns>
  private async Task<string> Status(int projectId, string? userId = null)
  {
    if (userId is not null)
      return await _reports.HasStudentSubmitted(projectId, userId)
        ? Stages.Completed
        : Stages.OnGoing;
    
    return await _reports.HasEveryStudentSubmitted(projectId)
      ? Stages.Completed
      : Stages.OnGoing;
  }
}
