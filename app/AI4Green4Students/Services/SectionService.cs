using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

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
      SortOrder = model.SortOrder,
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

  private async Task<Project> GetProject(int id)
    => await _db.Projects
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.ProjectType)
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException("Project not found");
}
