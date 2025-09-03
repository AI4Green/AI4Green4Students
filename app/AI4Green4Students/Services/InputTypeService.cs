namespace AI4Green4Students.Services;

using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Models.InputType;

public class InputTypeService
{
  private readonly ApplicationDbContext _db;

  public InputTypeService(ApplicationDbContext db) => _db = db;

  /// <summary>
  /// List input types.
  /// </summary>
  /// <returns>Input types.</returns>
  public async Task<List<InputTypeModel>> List()
    => await _db.InputTypes.AsNoTracking()
      .Select(x => new InputTypeModel(x))
      .ToListAsync();

  /// <summary>
  /// Create an input type.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Input type.</returns>
  public async Task<InputTypeModel> Create(CreateInputTypeModel model)
  {
    var existingInputType = await _db.InputTypes
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();

    if (existingInputType is not null)
    {
      return await Set(existingInputType.Id, model);
    }

    var entity = new InputType
    {
      Name = model.Name
    };

    await _db.InputTypes.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  /// <summary>
  /// Get an input type by ID.
  /// </summary>
  /// <param name="id">Input type ID.</param>
  /// <returns>Input type.</returns>
  public async Task<InputTypeModel> Get(int id)
    => await _db.InputTypes.AsNoTracking()
         .Where(x => x.Id == id)
         .Select(x => new InputTypeModel(x))
         .FirstOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Update an input type.
  /// </summary>
  /// <param name="id">Input type ID.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Input type.</returns>
  private async Task<InputTypeModel> Set(int id, CreateInputTypeModel model)
  {
    var entity = await _db.InputTypes.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Name = model.Name;

    _db.InputTypes.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
}
