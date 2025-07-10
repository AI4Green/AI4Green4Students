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
  {
    var inputTypes = await _db.InputTypes.AsNoTracking().ToListAsync();
    return inputTypes.ConvertAll<InputTypeModel>(x => new InputTypeModel(x));
  }

  /// <summary>
  /// Create or update an input type.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Input type model.</returns>
  public async Task<InputTypeModel> Create(CreateInputType model)
  {
    var isExistingValue = await _db.InputTypes
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();

    if (isExistingValue is not null)
    {
      return await Set(isExistingValue.Id, model);
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
  /// Get an input type by id.
  /// </summary>
  /// <param name="id">Input type id.</param>
  /// <returns>Input type.</returns>
  private async Task<InputTypeModel> Get(int id)
  {
    var result =
      await _db.InputTypes.AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync()
      ?? throw new KeyNotFoundException();

    return new InputTypeModel(result);
  }

  /// <summary>
  /// Update an input type.
  /// </summary>
  /// <param name="id">Input type id.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Input type model.</returns>
  private async Task<InputTypeModel> Set(int id, CreateInputType model)
  {
    var entity = await _db.InputTypes.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Name = model.Name;

    _db.InputTypes.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
}
