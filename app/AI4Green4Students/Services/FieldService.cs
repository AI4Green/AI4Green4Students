using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Field;
using AI4Green4Students.Models.Project;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class FieldService
{
  private readonly ApplicationDbContext _db;

  public FieldService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<FieldModel> Create(CreateFieldModel model)
  {
    var isExistingValue = await  _db.Fields.Where(x => x.Name == model.Name && x.Section.Id == model.Section).FirstOrDefaultAsync();

    if (isExistingValue is not null)
      return await Set(isExistingValue.Id, model);

    //Else, create a new Field
    var entity = new Field()
    {
      Name = model.Name,
      SortOrder = model.SortOrder,
      Mandatory = model.Mandatory,
      Section = _db.Sections.Single(x => x.Id == model.Section),
      InputType = _db.InputTypes.Single(x => x.Id == model.InputType),
      DefaultResponse = model.DefaultValue
    };

    //TODO add field options as an entity, should save those

    //todo check for any trigger fields to be created here before we add and save the parent entity (just call create method again).

    await _db.Fields.AddAsync(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);

  }

  public async Task<FieldModel> Set(int id, CreateFieldModel model)
  {
    var entity = await _db.Fields
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if project does not exist

    entity.Name = model.Name;

    _db.Fields.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  public async Task<FieldModel> Get(int id)
  {
    var result = await _db.Fields
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .Include(x => x.InputType)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new FieldModel(result);
  }

}
