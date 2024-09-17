using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Project;
using AI4Green4Students.Models.ProjectGroup;
using AI4Green4Students.Models.Report;
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
  /// List instructor's projects
  /// </summary>
  /// <param name="userId">Instructor's user id</param>
  /// <returns>Project list</returns>
  public async Task<List<ProjectModel>> ListByInstructor(string userId)
  {
    var projects = await ProjectsQuery().AsNoTracking()
      .Where(x => x.Instructors.Any(y => y.Id == userId))
      .ToListAsync();

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
  /// List student's projects
  /// </summary>
  /// <param name="userId">Student's user id</param>
  /// <returns>Project list</returns>
  public async Task<List<ProjectModel>> ListByStudent(string userId)
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
  private async Task<ProjectModel> Get(int id)
  {
    var project = await ProjectsQuery().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync() ?? throw new KeyNotFoundException();
    
    return new ProjectModel(project)
    {
      ProjectGroups = project.ProjectGroups.Select(x => new ProjectGroupModel { Id = x.Id, Name = x.Name }).ToList(),
      Stage = await Status(project.Id)
    };
  }
  
  /// <summary>
  /// Get instructor's project based on project id.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Instructor user id</param>
  /// <returns>Project</returns>
  public async Task<ProjectModel> GetByInstructor(int id, string userId)
  {
    var project = await ProjectsQuery().AsNoTracking()
                    .Where(x => x.Id == id && x.Instructors.Any(y => y.Id == userId))
                    .SingleOrDefaultAsync() ?? throw new KeyNotFoundException();

    return new ProjectModel(project)
    {
      ProjectGroups = project.ProjectGroups.Select(x => new ProjectGroupModel { Id = x.Id, Name = x.Name }).ToList(),
      Stage = await Status(project.Id)
    };
  }
  
  /// <summary>
  /// Get student's project based on project id.
  /// </summary>
  /// <param name="id">Project id. </param>
  /// <param name="userId">Student user id </param>
  /// <returns>Project</returns>
  public async Task<ProjectModel> GetByStudent(int id, string userId)
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
    var entity = new Project { Name = model.Name };
    
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
    
    _db.Projects.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  public async Task<ProjectSummaryModel> GetStudentProjectSummary(int projectId, string userId, bool isOwner = false, bool isInstructor = false)
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
    var reports = isOwner || isInstructor ? await _reports.ListByUser(projectId, userId) : [];

    var owner = project.ProjectGroups.SelectMany(x => x.Students).First(y => y.Id == userId)
                ?? throw new KeyNotFoundException();
    
    return new ProjectSummaryModel(
      isInstructor ? literatureReviews.Where(x => x.Stage != Stages.Draft).ToList() : literatureReviews,
      isInstructor ? plans.Where(x => x.Stage != Stages.Draft).ToList() : plans,
      reports,
      new ProjectSummaryProjectModel(project.Id, project.Name),
      new ProjectSummaryProjectGroupModel(projectGroup.Id, projectGroup.Name),
      new ProjectSummaryAuthorModel(owner.Id, owner.FullName)
    );
  }

  /// <summary>
  /// Check if a given users belongs to same project group
  /// </summary>
  /// <param name="firstUserId">Id of the user to viewing.</param>
  /// <param name="secondUserId">Id of the user being viewed.</param>
  /// <param name = "projectId">Project id.</param>
  /// <returns>True if the both users are the member of the sane project group, false otherwise.</returns>
  public async Task<bool> IsInSameProjectGroup(string firstUserId, string secondUserId, int projectId)
    => await _db.ProjectGroups.AsNoTracking()
      .Where(x => x.Project.Id == projectId && x.Students.Any(y => y.Id == firstUserId))
      .AnyAsync(x => x.Students.Any(y => y.Id == secondUserId));

  /// <summary>
  /// Check if a given user is the instructor of a given project.
  /// </summary>
  /// <param name="userId">Instructor id to check.</param>
  /// <param name="projectId">Project id.</param>
  /// <returns>True if the user is the instructor, false otherwise.</returns>
  public async Task<bool> IsProjectInstructor(string userId, int projectId)
    => await _db.Projects.AsNoTracking()
      .AnyAsync(x => x.Id == projectId && x.Instructors.Any(y => y.Id == userId));

  
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
