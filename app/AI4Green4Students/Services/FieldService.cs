namespace AI4Green4Students.Services;

using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Models.Field;

public class FieldService
{
  private readonly ApplicationDbContext _db;

  public FieldService(ApplicationDbContext db) => _db = db;

  /// <summary>
  /// Create a new field.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Field.</returns>
  public async Task<FieldModel> Create(CreateFieldModel model)
  {
    var existingField = await _db.Fields.AsNoTracking()
      .Where(x => EF.Functions.ILike(x.Name, model.Name) && x.Section.Id == model.Section)
      .FirstOrDefaultAsync();

    if (existingField is not null)
    {
      return await Set(existingField.Id, model);
    }

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

    // handle trigger target
    if (model.TriggerCause is not null && model.TriggerTarget is not null)
    {
      var createModel = await Create(model.TriggerTarget);
      entity.TriggerTarget = await _db.Fields.SingleAsync(x => x.Id == createModel.Id);
    }

    await _db.Fields.AddAsync(entity);

    // handle field options
    foreach (var name in model.SelectFieldOptions)
    {
      entity.SelectFieldOptions.Add(new SelectFieldOption
      {
        Name = name
      });
    }

    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  /// <summary>
  /// Update an existing field.
  /// </summary>
  /// <param name="id">Field ID.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Updated field.</returns>
  private async Task<FieldModel> Set(int id, CreateFieldModel model)
  {
    var section = await _db.Sections.FirstOrDefaultAsync(x => x.Id == model.Section)
                  ?? throw new KeyNotFoundException("Section not found");

    var inputType = await _db.InputTypes.FirstOrDefaultAsync(x => x.Id == model.InputType)
                    ?? throw new KeyNotFoundException("Input type not found");

    var entity = await _db.Fields
                   .Include(x => x.SelectFieldOptions)
                   .Include(x => x.Section)
                   .Include(x => x.InputType)
                   .Include(x => x.TriggerTarget)
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    entity.Name = model.Name;
    entity.SortOrder = model.SortOrder;
    entity.Mandatory = model.Mandatory;
    entity.Hidden = model.Hidden;
    entity.TriggerCause = model.TriggerCause;
    entity.DefaultResponse = model.DefaultValue;
    entity.Section = section;
    entity.InputType = inputType;

    // Handle trigger target
    if (model.TriggerCause is not null && model.TriggerTarget is not null)
    {
      var triggerTarget = await Create(model.TriggerTarget);
      entity.TriggerTarget = await _db.Fields.FindAsync(triggerTarget.Id);
    }
    else
    {
      entity.TriggerTarget = null;
    }

    // handle field options
    var existingOptionNames = entity.SelectFieldOptions.Select(x => x.Name).ToList();
    foreach (var name in model.SelectFieldOptions)
    {
      if (!existingOptionNames.Contains(name))
      {
        entity.SelectFieldOptions.Add(new SelectFieldOption
        {
          Name = name
        });
      }
    }

    // remove options that are no longer valid
    foreach (var existingOption in entity.SelectFieldOptions.ToList())
    {
      if (!model.SelectFieldOptions.Contains(existingOption.Name))
      {
        entity.SelectFieldOptions.Remove(existingOption);
      }
    }

    _db.Fields.Update(entity);
    await _db.SaveChangesAsync();

    return await Get(entity.Id);
  }

  /// <summary>
  /// Get a field by id.
  /// </summary>
  /// <param name="id">Field ID.</param>
  /// <returns>Field.</returns>
  public async Task<FieldModel> Get(int id)
    => await _db.Fields.AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.Section)
         .Include(x => x.InputType)
         .Include(x => x.TriggerTarget)
         .Include(x => x.SelectFieldOptions)
         .Select(x => new FieldModel(x))
         .SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Get a field by field response ID.
  /// </summary>
  /// <param name="id">Field response ID.</param>
  /// <returns>Field.</returns>
  public async Task<FieldModel> GetByFieldResponse(int id)
    => await _db.FieldResponses
         .AsNoTracking()
         .Where(x => x.Id == id)
         .Include(x => x.Field)
         .Include(x => x.Field.Section)
         .Include(x => x.Field.InputType)
         .Include(x => x.Field.SelectFieldOptions)
         .Select(x => x.Field)
         .Select(x => new FieldModel(x))
         .SingleOrDefaultAsync()
       ?? throw new KeyNotFoundException();

  /// <summary>
  /// Get a field by name for a given section type and project.
  /// </summary>
  /// <param name="projectId">Project id</param>
  /// <param name="sectionType">Section type name (e.g Plan, Note)</param>
  /// <param name="name">Field name</param>
  /// <remarks>Assumes field names are unique within a section type</remarks>
  /// <returns>Field matching the name</returns>
  public async Task<FieldModel> GetByName(int projectId, string sectionType, string name)
  {
    var fields = await ListBySectionType(sectionType, projectId);
    return fields.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException("Field not found");
  }

  /// <summary>
  /// List fields by section type for a given project.
  /// </summary>
  /// <param name="sectionType">Section type name (e.g. Plan, Note).</param>
  /// <param name="projectId">Project ID.</param>
  /// <returns>Fields.</returns>
  public async Task<List<FieldModel>> ListBySectionType(string sectionType, int projectId)
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectType)
                    .FirstOrDefaultAsync(x => x.Id == projectId)
                  ?? throw new KeyNotFoundException("Project not found");

    return await ListByProjectType(project.ProjectType.Id, sectionType);
  }

  /// <summary>
  /// List fields by section.
  /// </summary>
  /// <param name="id">Section ID.</param>
  /// <returns>Fields.</returns>
  public async Task<List<FieldModel>> ListBySection(int id)
    => await _db.Fields.AsNoTracking()
      .Include(x => x.Section)
      .Include(x => x.InputType)
      .Include(x => x.SelectFieldOptions)
      .Include(x => x.TriggerTarget)
      .Where(x => x.Section.Id == id)
      .Select(x => new FieldModel(x))
      .ToListAsync();

  /// <summary>
  /// List fields by project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <param name="sectionTypeName">Section type name.</param>
  /// <returns>Fields.</returns>
  public async Task<List<FieldModel>> ListByProjectType(int id, string? sectionTypeName = null)
    => await _db.Fields.AsNoTracking()
      .Include(x => x.Section)
      .Include(x => x.InputType)
      .Include(x => x.SelectFieldOptions)
      .Include(x => x.TriggerTarget)
      .Where(x => x.Section.ProjectType.Id == id &&
                  (sectionTypeName == null || EF.Functions.ILike(x.Section.SectionType.Name, sectionTypeName)))
      .Select(x => new FieldModel(x))
      .ToListAsync();
}
