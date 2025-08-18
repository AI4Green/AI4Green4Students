using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class SectionService
{
  private readonly ApplicationDbContext _db;

  public SectionService(ApplicationDbContext db)
  {
    _db = db;
  }

  /// <summary>
  /// Get all sections including their type.
  /// </summary>
  /// <returns>Sections list</returns>
  public async Task<List<SectionModel>> List()
    => await _db.Sections.AsNoTracking()
      .Include(x => x.SectionType)
      .Include(x => x.ProjectType)
      .Select(x => new SectionModel(x))
      .ToListAsync();
  /// <summary>
  /// Get all sections of a project.
  /// </summary>
  /// <param name="id">Project id</param>
  /// <returns>Sections list of a specific type</returns>
  public async Task<List<SectionModel>> ListByProject(int id)
  {
    var project = await GetProject(id);
    return await _db.Sections.AsNoTracking()
      .Where(x => x.ProjectType.Id == project.ProjectType.Id)
      .Include(x => x.SectionType)
      .Include(x => x.ProjectType)
      .Select(x => new SectionModel(x))
      .ToListAsync();
  }

  /// <summary>
  /// Get all sections of a specific type.
  /// </summary>
  /// <param name="sectionType">Section type name</param>
  /// <param name="projectId">Project id</param>
  /// <returns>Sections list of a specific type</returns>
  public async Task<List<SectionModel>> ListBySectionTypeName(string sectionType, int projectId)
  {
    var project = await GetProject(projectId);
    return await _db.Sections.AsNoTracking()
      .Where(x =>
        EF.Functions.ILike(x.SectionType.Name, sectionType) &&
        x.ProjectType.Id == project.ProjectType.Id)
      .Include(x => x.SectionType)
      .Include(x => x.ProjectType)
      .Select(x => new SectionModel(x))
      .ToListAsync();
  }

  /// <summary>
  /// Create a new section. Section are associated to a project.
  /// If a section name already exists, the existing section is updated.
  /// </summary>
  /// <param name="model">DTO model for creating a new section</param>
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
      return await Set(existing.Id, model); // Update existing Section if it exists

    // Else, create new Section
    var entity = new Section()
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
  /// <param name="id">Id of the section to update</param>
  /// <param name="model">DTO model for updating a section</param>
  /// <returns>Updated section</returns>
  public async Task<SectionModel> Set(int id, CreateSectionModel model)
  {
    var entity = await _db.Sections
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if section does not exist

    entity.SectionType = await _db.SectionTypes.SingleOrDefaultAsync(x => x.Id == model.SectionTypeId)
                         ?? throw new KeyNotFoundException();
    entity.Name = model.Name;
    entity.SortOrder = model.SortOrder;

    _db.Sections.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Get a section by its id.
  /// </summary>
  /// <param name="id">Id of the section to get</param>
  /// <returns>Section matching the id</returns>
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
