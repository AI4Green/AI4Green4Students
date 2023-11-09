using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.InputType;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class InputTypeService
{
  private readonly ApplicationDbContext _db;

  public InputTypeService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<InputTypeModel> Get(int id)
  {
    var result = await _db.InputTypes
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new InputTypeModel(result);
  }

  public async Task<InputTypeModel> Create(CreateInputType model)
  {
    var isExistingValue = await _db.InputTypes
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();

    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model); // Update existing InputType if it exists

    // Else, create new InputType
    var entity = new InputType()
    {
      Name = model.Name,
    };

    await _db.InputTypes.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  public async Task<InputTypeModel> Set(int id, CreateInputType model)
  {
    var entity = await _db.InputTypes
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if inputtype does not exist

    entity.Name = model.Name;

    _db.InputTypes.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
}
