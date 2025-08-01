using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Models.SectionType;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class SectionTypeService
{
  private readonly ApplicationDbContext _db;

  public SectionTypeService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<SectionTypeModel>> List()
    => 
      await _db.SectionTypes
      .AsNoTracking()
      .Select(x => new SectionTypeModel(x))
      .ToListAsync();
  
  public async Task<List<SectionTypeModel>> ListByProject(int projectId)
    => 
      await _db.SectionTypes
        .AsNoTracking()
        .Where(x => x.Sections.Any(y => y.Project.Id == projectId))
        .Select(x => new SectionTypeModel(x))
        .ToListAsync();
  
  public async Task<SectionTypeModel> Get(int id)
  {
    var result = await _db.SectionTypes
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new SectionTypeModel(result);
  }
  
  /// <summary>
  /// Get a section type entity by name.
  /// </summary>
  /// <param name="sectionType">Section type name</param>
  /// <returns>Section type entity</returns>
  public async Task<SectionTypeModel> GetSectionType(string sectionType)
  {
    var entity = await _db.SectionTypes
      .AsNoTracking()
      .Where(x=>x.Name == sectionType)
      .SingleAsync();

    return new SectionTypeModel(entity);
  }

  public async Task<SectionTypeModel> Create(CreateSectionTypeModel model)
  {
    var isExistingValue = await _db.SectionTypes
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();

    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model);

    var entity = new SectionType()
    {
      Name = model.Name,
    };

    await _db.SectionTypes.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  public async Task<SectionTypeModel> Set(int id, CreateSectionTypeModel model)
  {
    var entity = await _db.SectionTypes
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Name = model.Name;

    _db.SectionTypes.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
}
