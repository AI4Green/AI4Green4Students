namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Models.Section;

public class SectionService
{
  private readonly ApplicationDbContext _db;

  public SectionService(ApplicationDbContext db) => _db = db;

  /// <summary>
  /// List all sections including their type.
  /// </summary>
  /// <returns>Sections.</returns>
  public async Task<List<SectionModel>> List()
    => await _db.Sections.AsNoTracking()
      .Include(x => x.SectionType)
      .Include(x => x.ProjectType)
      .Select(x => new SectionModel(x))
      .ToListAsync();

  /// <summary>
  /// List sections by project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <returns>Sections.</returns>
  public async Task<List<SectionModel>> ListByProjectType(int id)
    => await _db.Sections.AsNoTracking()
      .Where(x => x.ProjectType.Id == id)
      .Include(x => x.SectionType)
      .Include(x => x.ProjectType)
      .Select(x => new SectionModel(x))
      .ToListAsync();

  /// <summary>
  /// List sections by project type and section type name.
  /// </summary>
  /// <param name="id"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public async Task<List<SectionModel>> ListBySectionTypeName(int id, string name)
    => await _db.Sections.AsNoTracking()
      .Where(x => EF.Functions.ILike(x.SectionType.Name, name) && x.ProjectType.Id == id)
      .Include(x => x.SectionType)
      .Include(x => x.ProjectType)
      .Select(x => new SectionModel(x))
      .ToListAsync();

  /// <summary>
  /// List project sections.
  /// </summary>
  /// <param name="id">Project ID.</param>
  /// <returns>Sections.</returns>
  public async Task<List<SectionModel>> ListByProject(int id)
  {
    var project = await GetProject(id);
    return await ListByProjectType(project.ProjectType.Id);
  }

  /// <summary>
  /// List project sections of a specific section type.
  /// </summary>
  /// <param name="sectionType">Section type name</param>
  /// <param name="projectId">Project id</param>
  /// <returns>Sections list of a specific type</returns>
  public async Task<List<SectionModel>> ListBySectionTypeName(string sectionType, int projectId)
  {
    var project = await GetProject(projectId);
    return await ListBySectionTypeName(project.ProjectType.Id, sectionType);
  }

  /// <summary>
  /// Create a new section. Update an existing section the same name.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Newly created section</returns>
  public async Task<SectionModel> Create(CreateSectionModel model)
  {
    var existing = await _db.Sections.AsNoTracking()
      .Where(x =>
        EF.Functions.ILike(x.Name, model.Name) &&
        x.SectionType.Id == model.SectionTypeId &&
        x.ProjectType.Id == model.ProjectTypeId
      )
      .FirstOrDefaultAsync();

    if (existing is not null)
    {
      return await Set(existing.Id, model);
    }

    var entity = new Section
    {
      Name = model.Name,
      ProjectType = await _db.ProjectTypes.SingleOrDefaultAsync(x => x.Id == model.ProjectTypeId)
                    ?? throw new KeyNotFoundException(),
      SectionType = await _db.SectionTypes.SingleOrDefaultAsync(x => x.Id == model.SectionTypeId)
                    ?? throw new KeyNotFoundException(),
      SortOrder = model.SortOrder
    };

    await _db.Sections.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  /// <summary>
  /// Update an existing section.
  /// </summary>
  /// <param name="id">Section ID.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Updated section.</returns>
  public async Task<SectionModel> Set(int id, CreateSectionModel model)
  {
    var entity = await _db.Sections
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.SectionType = await _db.SectionTypes.SingleOrDefaultAsync(x => x.Id == model.SectionTypeId)
                         ?? throw new KeyNotFoundException();
    entity.Name = model.Name;
    entity.SortOrder = model.SortOrder;

    _db.Sections.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Get a section.
  /// </summary>
  /// <param name="id">Section ID.</param>
  /// <returns>Section.</returns>
  public async Task<SectionModel> Get(int id)
    => await _db.Sections
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.SectionType)
         .Include(x => x.ProjectType)
         .Select(x => new SectionModel(x))
         .SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Save sections.
  /// </summary>
  /// <param name="model">Save model.</param>
  public async Task Save(SaveSectionsModel model)
  {
    var projectType = await _db.ProjectTypes
                        .Include(x => x.Stage)
                        .SingleOrDefaultAsync(x => x.Id == model.ProjectTypeId)
                      ?? throw new KeyNotFoundException();

    if (projectType.Stage.DisplayName != Stages.Draft)
    {
      throw new InvalidOperationException("Only draft project types can be modified.");
    }

    var sectionType = await _db.SectionTypes.SingleOrDefaultAsync(x => x.Id == model.SectionTypeId)
                      ?? throw new KeyNotFoundException();

    var existingSections = await _db.Sections
      .Where(x => x.ProjectType.Id == model.ProjectTypeId && x.SectionType.Id == model.SectionTypeId)
      .ToListAsync();

    foreach (var sectionModel in model.Sections)
    {
      if (sectionModel.Id.HasValue)
      {
        var existing = existingSections.FirstOrDefault(x => x.Id == sectionModel.Id.Value);
        if (existing is null)
        {
          continue;
        }
        existing.Name = sectionModel.Name;
        existing.SortOrder = sectionModel.SortOrder;

        _db.Sections.Update(existing);
      }
      else
      {
        var newSection = new Section
        {
          Name = sectionModel.Name,
          SortOrder = sectionModel.SortOrder,
          ProjectType = projectType,
          SectionType = sectionType
        };

        await _db.Sections.AddAsync(newSection);
      }
    }

    var ids = model.Sections.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToList();
    var toDelete = existingSections.Where(x => !ids.Contains(x.Id)).ToList();
    if (toDelete.Count != 0)
    {
      _db.Sections.RemoveRange(toDelete);
    }

    await _db.SaveChangesAsync();
  }

  private async Task<Project> GetProject(int id)
    => await _db.Projects
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.ProjectType)
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException("Project not found");
}
