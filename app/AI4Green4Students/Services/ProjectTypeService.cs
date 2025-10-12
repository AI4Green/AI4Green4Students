namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Models.ProjectType;

public class ProjectTypeService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stage;
  public ProjectTypeService(ApplicationDbContext db, StageService stage)
  {
    _db = db;
    _stage = stage;
  }

  /// <summary>
  /// Create a new project type.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Project type.</returns>
  public async Task<ProjectTypeModel> Create(CreateProjectTypeModel model)
  {
    var existing = await _db.Projects
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();

    if (existing is not null)
    {
      return await Set(existing.Id, model);
    }

    var draftStage = await _db.Stages
      .Where(x => x.Type.Value == ProjectTypeDefaults.StageType && x.DisplayName == Stages.Draft)
      .FirstOrDefaultAsync();

    if (draftStage is null)
    {
      throw new KeyNotFoundException("Stage not found.");
    }

    var entity = new ProjectType
    {
      Name = model.Name, Description = model.Description, Stage = draftStage
    };

    _db.ProjectTypes.Add(entity);
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Update a project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Project type.</returns>
  public async Task<ProjectTypeModel> Set(int id, CreateProjectTypeModel model)
  {
    var entity = await _db.ProjectTypes.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException("Project type not found.");

    entity.Name = model.Name;
    entity.Description = model.Description;

    _db.ProjectTypes.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Delete a project type.
  /// </summary>
  /// <param name="id">Project type id.</param>
  public async Task Delete(int id)
  {
    var entity = await _db.ProjectTypes.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException("Project type not found.");

    var isInUse = await _db.Projects.Where(x => x.ProjectType.Id == id).AnyAsync();
    if (isInUse)
    {
      throw new InvalidOperationException("Cannot delete a project type that is in use by a project.");
    }

    _db.ProjectTypes.Remove(entity);
    await _db.SaveChangesAsync();
  }

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

    var list = await _db.ProjectTypes.AsNoTracking()
      .Include(x => x.Stage)
      .ToListAsync();

    var stageOrders = list.Select(x => x.Stage.SortOrder).Distinct().ToList();
    var permissions = await _stage.ListPermissionsByStages(stageOrders, ProjectTypeDefaults.StageType);

    return list.Select(x => new ProjectTypeModel(
      x,
      projectCounts.GetValueOrDefault(x.Id, 0),
      permissions.GetValueOrDefault(x.Stage.SortOrder, new List<string>())
    )).ToList();
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
    var stagePermissions = await _stage.ListPermissions(projectType.Stage.SortOrder, ProjectTypeDefaults.StageType);

    return new ProjectTypeModel(projectType, projectCount, stagePermissions);
  }
}
