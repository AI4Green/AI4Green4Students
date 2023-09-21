using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Experiment;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ExperimentTypeService
{
  private readonly ApplicationDbContext _db;
  
  public ExperimentTypeService(ApplicationDbContext db)
  {
    _db = db;
  }
  
  public async Task<List<ExperimentTypeModel>> List()
  => await _db.ExperimentTypes
      .AsNoTracking()
      .Select(x => new ExperimentTypeModel
      {
        Id = x.Id,
        Name = x.Name
      })
      .ToListAsync();
  
  public async Task<ExperimentTypeModel> Get(int id)
  {
    var result = await _db.ExperimentTypes
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ExperimentTypeModel
    {
      Id = result.Id,
      Name = result.Name
    };
  }
  
  public async Task<ExperimentTypeModel> Create (CreateExperimentTypeModel model)
  {
    var isExistingValue = await _db.ExperimentTypes
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();
    
    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model); // Update existing Experiment type if it exists
    
    // Else, create new Experiment type
    var entity = new ExperimentType()
    {
      Name = model.Name,
    };
    
    await _db.ExperimentTypes.AddAsync(entity);
    await _db.SaveChangesAsync();
    
    return await Get(entity.Id);
  }
  
  public async Task<ExperimentTypeModel> Set (int id, CreateExperimentTypeModel model)
  {
    var entity = await _db.ExperimentTypes
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if Experiment type does not exist
    
    entity.Name = model.Name;
    
    _db.ExperimentTypes.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
}
