namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Models.Project;
using ProjectGroupModel=Models.Project.ProjectGroupModel;

public class ProjectService
{
  private readonly ApplicationDbContext _db;
  private readonly LiteratureReviewService _literatureReviews;
  private readonly PlanService _plans;
  private readonly ReportService _reports;

  public ProjectService(
    ApplicationDbContext db,
    LiteratureReviewService literatureReviews,
    PlanService plans,
    ReportService reports
  )
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
    var projects = await _db.Projects.AsNoTracking()
      .Include(x => x.ProjectGroups)
      .Where(x => x.Instructors.Any(y => y.Id == userId))
      .ToListAsync();

    var list = new List<ProjectModel>();
    foreach (var project in projects)
    {
      list.Add(new ProjectModel(project)
      {
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
    var userProjects = await _db.Projects.AsNoTracking()
      .Include(x => x.ProjectGroups).ThenInclude(y => y.Students)
      .Where(x => x.ProjectGroups.Any(y => y.Students.Any(z => z.Id == userId)))
      .AsSplitQuery()
      .ToListAsync();

    var list = new List<ProjectModel>();
    foreach (var project in userProjects)
    {
      list.Add(new ProjectModel(project)
      {
        ProjectGroups = project.ProjectGroups
          .Where(pg => pg.Students.Any(s => s.Id == userId))
          .Select(x => new ProjectGroupModel(x.Id, x.Name)).ToList(),
        Stage = await Status(project.Id, userId)
      });
    }

    return list;
  }

  /// <summary>
  /// Get a project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>Project.</returns>
  private async Task<ProjectModel> Get(int id)
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectGroups)
                    .Where(x => x.Id == id).SingleOrDefaultAsync()
                  ?? throw new KeyNotFoundException();

    return new ProjectModel(project)
    {
      Stage = await Status(project.Id)
    };
  }

  /// <summary>
  /// Get instructor's project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Instructor user id.</param>
  /// <returns>Project.</returns>
  public async Task<ProjectModel> GetByInstructor(int id, string userId)
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectGroups)
                    .Where(x => x.Id == id && x.Instructors.Any(y => y.Id == userId))
                    .SingleOrDefaultAsync()
                  ?? throw new KeyNotFoundException();

    return new ProjectModel(project)
    {
      Stage = await Status(project.Id)
    };
  }

  /// <summary>
  /// Get student's project.
  /// </summary>
  /// <param name="id">Project id. </param>
  /// <param name="userId">Student id.</param>
  /// <returns>Project.</returns>
  public async Task<ProjectModel> GetByStudent(int id, string userId)
  {
    var result = await _db.Projects.AsNoTracking()
                   .Where(x => x.Id == id && x.ProjectGroups.Any(y => y.Students.Any(z => z.Id == userId)))
                   .Select(x => new
                   {
                     Project = x,
                     ProjectGroups = x.ProjectGroups.Where(pg => pg.Students.Any(s => s.Id == userId)).ToList()
                   })
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    var projectModel = new ProjectModel(result.Project)
    {
      ProjectGroups = result.ProjectGroups.Select(x => new ProjectGroupModel(x.Id, x.Name)).ToList(),
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
    var entity = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();

    _db.Projects.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Create project.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Project.</returns>
  public async Task<ProjectModel> Create(CreateProjectModel model)
  {
    var isExistingValue = await _db.Projects
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();

    if (isExistingValue is not null)
    {
      return await Set(isExistingValue.Id, model);
    }

    var instructors = _db.Users.Where(x => model.InstructorIds.Contains(x.Id)).ToList();
    var entity = new Project
    {
      Name = model.Name, Instructors = instructors
    };

    await _db.Projects.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  /// <summary>
  /// Update project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Updated Project.</returns>
  public async Task<ProjectModel> Set(int id, CreateProjectModel model)
  {
    var entity = await _db.Projects.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Name = model.Name;

    _db.Projects.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Get student's project summary.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Student id.</param>
  /// <param name="isOwner">Is an entity owner?</param>
  /// <param name="isInstructor">Is instructor?</param>
  /// <returns>Project summary.</returns>
  public async Task<ProjectSummaryModel> GetStudentProjectSummary(
    int id,
    string userId,
    bool isOwner = false,
    bool isInstructor = false
  )
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectGroups).ThenInclude(x => x.Students)
                    .AsSplitQuery()
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync()
                  ?? throw new KeyNotFoundException();

    var projectGroup = project.ProjectGroups.FirstOrDefault(x => x.Students.Any(y => y.Id == userId));
    if (projectGroup is null)
    {
      throw new KeyNotFoundException();
    }

    var literatureReviews = await _literatureReviews.ListByUser(id, userId);
    var plans = await _plans.ListByUser(id, userId);
    var reports = isOwner || isInstructor ? await _reports.ListByUser(id, userId) : [];

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
  /// Check if given users belong to the same project group.
  /// </summary>
  /// <param name="viewerId">User id.</param>
  /// <param name="targetUserId">User id.</param>
  /// <param name="projectId">Project id.</param>
  /// <returns>Result.</returns>
  public async Task<bool> IsInSameProjectGroup(string viewerId, string targetUserId, int projectId)
    => await _db.ProjectGroups.AsNoTracking()
      .Where(x => x.Project.Id == projectId && x.Students.Any(y => y.Id == viewerId))
      .AnyAsync(x => x.Students.Any(y => y.Id == targetUserId));

  /// <summary>
  /// Check if a given user is the instructor of a given project.
  /// </summary>
  /// <param name="userId">Instructor id.</param>
  /// <param name="projectId">Project id.</param>
  /// <returns>Result.</returns>
  public async Task<bool> IsProjectInstructor(string userId, int projectId)
    => await _db.Projects.AsNoTracking()
      .AnyAsync(x => x.Id == projectId && x.Instructors.Any(y => y.Id == userId));

  /// <summary>
  /// Get project status.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="userId">User id. If provided, status is based on user's submission.</param>
  /// <returns>Project status.</returns>
  private async Task<string> Status(int projectId, string? userId = null)
  {
    if (userId is not null)
    {
      return await _reports.HasStudentSubmitted(projectId, userId)
        ? Stages.Completed
        : Stages.OnGoing;
    }

    return await _reports.HasEveryStudentSubmitted(projectId)
      ? Stages.Completed
      : Stages.OnGoing;
  }
}
