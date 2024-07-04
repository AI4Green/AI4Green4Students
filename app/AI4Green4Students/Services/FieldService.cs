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

  /// <summary>
  /// Create a new field.
  /// </summary>
  /// <param name="model">Field model</param>
  /// <returns>Created field model</returns>
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

  /// <summary>
  /// Update an existing field.
  /// </summary>
  /// <param name="entity">Field entity to update</param>
  /// <param name="model">Field Model to update the field with</param>
  /// <returns>Updated field model</returns>
  private async Task<FieldModel> Set(Field entity, CreateFieldModel model)
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

  /// <summary>
  /// Get a field by id.
  /// </summary>
  /// <param name="id">Field id</param>
  /// <returns>Field model matching the id</returns>
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

  /// <summary>
  /// Get a field by name for a given section type and project.
  /// </summary>
  /// <param name="projectId">Project id</param>
  /// <param name="sectionType">Section type name (e.g Plan, Note)</param>
  /// <param name="fieldName">Field name</param>
  /// <remarks>Assumes field names are unique within a section type</remarks>
  /// <returns>Field matching the name</returns>
  public async Task<FieldModel> GetByName(int projectId, string sectionType, string fieldName)
  {
    var fields = await ListBySectionType(sectionType, projectId);
    return new FieldModel(fields.SingleOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                          ?? throw new KeyNotFoundException()
    );
  }
  
  /// <summary>
  /// Get a list of fields for a given section type and project.
  /// </summary>
  /// <param name="sectionType">Section type name (e.g Plan, Note)</param>
  /// <param name="projectId">Project id.
  /// Ensures only fields associated with the project and section type are returned
  /// </param>
  /// <returns>Section type fields list</returns>
  public async Task<List<Field>> ListBySectionType(string sectionType, int projectId)
    => await _db.Fields
      .Include(x => x.Section)
      .Include(x => x.InputType)
      .Include(x => x.SelectFieldOptions)
      .Include(x => x.TriggerTarget)
      .Where(x => x.Section.SectionType.Name == sectionType && x.Section.Project.Id == projectId)
      .ToListAsync();
  
  /// <summary>
  /// Get a list of fields for a given section.
  /// </summary>
  /// <param name="sectionId">Section id</param>
  /// <returns>Section fields list</returns>
  public async Task<List<Field>> ListBySection(int sectionId)
    => await _db.Fields
      .AsNoTracking()
      .Include(x => x.InputType)
      .Include(x => x.SelectFieldOptions)
      .Include(x => x.TriggerTarget)
      .Where(x => x.Section.Id == sectionId)
      .ToListAsync();
  
  public async Task Delete(int id)
  {
    var entity = await _db.Fields.FindAsync(id) ?? throw new KeyNotFoundException();
    _db.Fields.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
