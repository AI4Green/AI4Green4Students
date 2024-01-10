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
      Hidden = model.Hidden,
      Section = _db.Sections.Single(x => x.Id == model.Section),
      InputType = _db.InputTypes.Single(x => x.Id == model.InputType),
      TriggerCause = model.TriggerCause,
      DefaultResponse = model.DefaultValue
    };

    //check for any trigger fields to be created here before we add and save the parent entity (just call create method again).
    if(model.TriggerCause != null && model.TriggerTarget != null)
    {
      var createModel = await Create(model.TriggerTarget);
      entity.TriggerTarget = _db.Fields.Single(x => x.Id == createModel.Id);
    }

    await _db.Fields.AddAsync(entity);


    //add field options as an entity, should save those
    foreach (var name in model.SelectFieldOptions)
    {
      var fieldOptionEntity = new SelectFieldOption()
      {
        Field = entity,
        Name = name
      };
      await _db.SelectFieldOptions.AddAsync(fieldOptionEntity);
    }

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
                   .Include(x => x.TriggerTarget)
                   .Include(x => x.SelectFieldOptions)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new FieldModel(result);
  }

}
