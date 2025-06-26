namespace AI4Green4Students.Services.SectionTypeData;

using Data.Entities;
using Constants;
using Data;
using Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;
using Models.Section;

public abstract class BaseSectionTypeService<T> where T : CoreSectionTypeData
{
  private readonly ApplicationDbContext _db;
  private readonly SectionFormService _sectionForm;

  protected BaseSectionTypeService(ApplicationDbContext db, SectionFormService sectionForm)
  {
    _db = db;
    _sectionForm = sectionForm;
  }

  /// <summary>
  /// List summary.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <returns>List.</returns>
  public async Task<List<SectionSummaryModel>> ListSummary(int id)
    => await _sectionForm.ListSummary<T>(id);

  /// <summary>
  /// Get a section form.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <param name="sectionId">Section id.</param>
  /// <returns>Section form.</returns>
  public async Task<SectionFormModel> GetSectionForm(int id, int sectionId)
    => await _sectionForm.GetSectionForm<T>(id, sectionId);

  /// <summary>
  /// Save section form.
  /// </summary>
  /// <param name="model">Section form payload model.</param>
  /// <returns>Section form.</returns>
  public async Task<SectionFormModel> SaveSectionForm(SectionFormPayloadModel model)
    => await _sectionForm.SaveForm<T>(model);

  /// <summary>
  /// Delete entity.
  /// </summary>
  /// <param name="userId">Owner id.</param>
  /// <param name="id">Entity id.</param>
  public async Task Delete(int id, string userId)
  {
    var entity = await _db.Set<T>().Where(x => x.Id == id && x.Owner.Id == userId).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    _db.Set<T>().Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Check if a given user is the owner of a given entity.
  /// </summary>
  /// <param name="userId">User id.</param>
  /// <param name="id">Entity id.</param>
  /// <returns>Result.</returns>
  public async Task<bool> IsOwner(string userId, int id)
    => await _db.Set<T>().AsNoTracking().AnyAsync(x => x.Id == id && x.Owner.Id == userId);

  /// <summary>
  /// Check if a given user is the member of a given project group.
  /// </summary>
  /// <param name="userId">User id.</param>
  /// <param name="id">Entity id.</param>
  /// <returns>Result.</returns>
  public async Task<bool> IsInSameProjectGroup(string userId, int id)
  {
    var entity = await GetEntity(id);

    // Check if both the owner and the viewer are in the same project group
    return await _db.ProjectGroups.AsNoTracking()
      .Where(x =>
        x.Project.Id == entity.Project.Id &&
        x.Students.Any(y => y.Id == entity.Owner.Id)
      )
      .AnyAsync(x => x.Students.Any(y => y.Id == userId));
  }

  /// <summary>
  /// Check if a given user is the project instructor.
  /// </summary>
  /// <param name="userId">User id.</param>
  /// <param name="id">Entity id.</param>
  /// <returns>Result.</returns>
  public async Task<bool> IsProjectInstructor(string userId, int id)
  {
    var entity = await GetEntity(id);

    var query = _db.Projects.AsNoTracking()
      .Where(x =>
        x.Id == entity.Project.Id &&
        x.Instructors.Any(y => y.Id == userId)
      );

    if (entity is Note or Report)
    {
      return await query.AnyAsync();
    }

    return await query.Where(x => entity.Stage.DisplayName != Stages.Draft).AnyAsync();
  }

  /// <summary>
  /// Get a project group. User must be a member of the project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>Project group.</returns>
  protected async Task<ProjectGroup> GetProjectGroup(int id, string userId)
    => await _db.ProjectGroups.Where(x => x.Id == id && x.Students.Any(y => y.Id == userId))
         .Include(x => x.Project)
         .SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Get stage.
  /// </summary>
  /// <param name="type">Stage type.</param>
  /// <param name="name">Stage name.</param>
  /// <returns>Stage.</returns>
  protected async Task<Stage> GetStage(string type, string? name)
  {
    var query = _db.Stages.Where(x => x.Type.Value == type);
    if (name is not null)
    {
      query = query.Where(x => x.DisplayName == name);
    }
    return await query.SingleOrDefaultAsync() ?? throw new KeyNotFoundException();
  }

  /// <summary>
  /// Get entity.
  /// </summary>
  /// <param name="id">Entity id.</param>
  /// <returns>Entity.</returns>
  private async Task<T> GetEntity(int id)
    => await BaseQuery().Where(x => x.Id == id).SingleOrDefaultAsync() ?? throw new KeyNotFoundException();

  /// <summary>
  /// Base query for all section type data.
  /// </summary>
  /// <returns>Base query.</returns>
  private IQueryable<T> BaseQuery()
    => _db.Set<T>()
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage);
}
