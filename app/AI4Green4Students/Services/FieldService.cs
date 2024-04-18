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
    var existingField = await  _db.Fields
      .Include(x=> x.SelectFieldOptions)
      .Where(x => x.Name == model.Name && x.Section.Id == model.Section).FirstOrDefaultAsync();

    if (existingField is not null)
      return await Set(existingField, model);

    //Else, create a new Field
    var entity = new Field
    {
      Name = model.Name,
      SortOrder = model.SortOrder,
      Mandatory = model.Mandatory,
      Hidden = model.Hidden,
      Section = await _db.Sections.SingleAsync(x => x.Id == model.Section),
      InputType = await _db.InputTypes.SingleAsync(x => x.Id == model.InputType),
      TriggerCause = model.TriggerCause,
      DefaultResponse = model.DefaultValue
    };

    //check for any trigger fields to be created here before we add and save the parent entity (just call create method again).
    if(model.TriggerCause is not null && model.TriggerTarget is not null)
    {
      var createModel = await Create(model.TriggerTarget);
      entity.TriggerTarget = await _db.Fields.SingleAsync(x => x.Id == createModel.Id);
    }

    await _db.Fields.AddAsync(entity);
    
    //add field options as an entity, should save those
    foreach (var name in model.SelectFieldOptions)
    {
      entity.SelectFieldOptions.Add(new SelectFieldOption { Name = name });
    }

    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  public async Task<FieldModel> Set(Field entity, CreateFieldModel model)
  {
    entity.Name = model.Name;
    entity.SortOrder = model.SortOrder;
    entity.Mandatory = model.Mandatory;
    entity.Hidden = model.Hidden;
    entity.TriggerCause = model.TriggerCause;
    entity.DefaultResponse = model.DefaultValue;
    entity.Section = await _db.Sections.SingleAsync(x => x.Id == model.Section);
    entity.InputType = await _db.InputTypes.SingleAsync(x => x.Id == model.InputType);

    // Handle trigger target
    if (model.TriggerCause is not null && model.TriggerTarget is not null)
    {
      var triggerTarget = await Create(model.TriggerTarget);
      entity.TriggerTarget = await _db.Fields.FindAsync(triggerTarget.Id);
    }
    else entity.TriggerTarget = null;
    
    // Update field options
    var existingOptionNames = entity.SelectFieldOptions.Select(opt => opt.Name).ToList();
    foreach (var name in model.SelectFieldOptions)
    {
      if (!existingOptionNames.Contains(name))
        entity.SelectFieldOptions.Add(new SelectFieldOption { Name = name });
    }

    // Remove options that are not present in the model
    foreach (var existingOption in entity.SelectFieldOptions.ToList())
    {
      if (!model.SelectFieldOptions.Contains(existingOption.Name))
        entity.SelectFieldOptions.Remove(existingOption);
    }
    
    _db.Fields.Update(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
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

  public async Task<List<FieldModel>> List()
  {
    var result = await _db.Fields
                   .AsNoTracking()
                   .Include(x => x.InputType)
                   .Include(x => x.TriggerTarget)
                   .Include(x => x.SelectFieldOptions)
                   .ToListAsync();

    return result.Select(x => new FieldModel(x)).ToList();
  }
  
  public async Task Delete(int id)
  {
    var entity = await _db.Fields.FindAsync(id) ?? throw new KeyNotFoundException();
    _db.Fields.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
