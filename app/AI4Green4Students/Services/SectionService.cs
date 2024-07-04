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
      .Select(x => new SectionModel(x)).ToListAsync();

  /// <summary>
  /// Get all sections of a specific type.
  /// </summary>
  /// <param name="sectionTypeId">Section type id</param>
  /// <returns>Sections list of a specific type</returns>
  public async Task<List<SectionModel>> ListBySectionType(int sectionTypeId)
    => await _db.Sections.AsNoTracking()
      .Where(x => x.SectionType.Id == sectionTypeId)
      .Include(x => x.SectionType)
      .Select(x => new SectionModel(x))
      .ToListAsync();
  
  /// <summary>
  /// Get all sections of a specific type.
  /// </summary>
  /// <param name="sectionType">Section type name</param>
  /// <param name="projectId">Project id</param>
  /// <returns>Sections list of a specific type</returns>
  public async Task<List<SectionModel>> ListBySectionTypeName(string sectionType, int projectId)
    => await _db.Sections.AsNoTracking()
      .Where(x => x.SectionType.Name == sectionType && x.Project.Id == projectId)
      .Include(x => x.SectionType)
      .Select(x => new SectionModel(x))
      .ToListAsync();

  /// <summary>
  /// Create a new section. Section are associated to a project.
  /// If a section name already exists, the existing section is updated.
  /// </summary>
  /// <param name="model">DTO model for creating a new section</param>
  /// <returns>Newly created section</returns>
  public async Task<SectionModel> Create(CreateSectionModel model)
  {
    var isExistingValue = await _db.Sections
      .Where(x => EF.Functions.ILike(x.Name, model.Name) && x.SectionType.Id == model.SectionTypeId)
      .Include(x => x.Project)
      .FirstOrDefaultAsync();

    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model); // Update existing Section if it exists

    // Else, create new Section
    var entity = new Section()
    {
      Name = model.Name,
      Project = await _db.Projects.SingleOrDefaultAsync(x => x.Id == model.ProjectId)
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

    entity.Project = await _db.Projects.SingleOrDefaultAsync(x => x.Id == model.ProjectId)
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
  /// Get a section by its id.
  /// </summary>
  /// <param name="id">Id of the section to get</param>
  /// <returns>Section matching the id</returns>
  public async Task<SectionModel> Get(int id)
    => await _db.Sections
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.SectionType)
         .Select(x => new SectionModel(x))
         .SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();
}
