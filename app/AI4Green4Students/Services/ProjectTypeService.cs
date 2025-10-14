namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Models.Field;
using Models.ProjectType;
using Models.Section;

public class ProjectTypeService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stage;
  private readonly SectionService _section;
  private readonly FieldService _field;
  public ProjectTypeService(ApplicationDbContext db, StageService stage, SectionService section, FieldService field)
  {
    _db = db;
    _stage = stage;
    _section = section;
    _field = field;
  }

  /// <summary>
  /// Create a new project type.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Project type.</returns>
  public async Task<ProjectTypeModel> Create(CreateProjectTypeModel model)
  {
    var isDuplicateName = await _db.ProjectTypes
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .AnyAsync();

    if (isDuplicateName)
    {
      throw new InvalidOperationException("Project type name must be unique.");
    }

    var draftStage = await _db.Stages
      .Where(x => x.Type.Value == ProjectTypeDefaults.StageType && x.DisplayName == Stages.Draft)
      .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Stage not found.");

    var entity = new ProjectType
    {
      Name = model.Name, Description = model.Description, Stage = draftStage
    };

    _db.ProjectTypes.Add(entity);
    await _db.SaveChangesAsync();

    if (model.Id is not null)
    {
      await Import(entity.Id, model.Id.Value);
    }

    return await Get(entity.Id);
  }

  /// <summary>
  /// Update a project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Project type.</returns>
  public async Task<ProjectTypeModel> Set(int id, CreateProjectTypeModel model)
  {
    var entity = await _db.ProjectTypes.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException("Project type not found.");

    entity.Name = model.Name;
    entity.Description = model.Description;

    _db.ProjectTypes.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Delete a project type.
  /// </summary>
  /// <param name="id">Project type id.</param>
  public async Task Delete(int id)
  {
    var entity = await _db.ProjectTypes.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException("Project type not found.");

    var isInUse = await _db.Projects.Where(x => x.ProjectType.Id == id).AnyAsync();
    if (isInUse)
    {
      throw new InvalidOperationException("Cannot delete a project type that is in use by a project.");
    }

    _db.ProjectTypes.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// List all project types with project counts.
  /// </summary>
  /// <returns>Project types.</returns>
  public async Task<List<ProjectTypeModel>> List()
  {
    var projectCounts = await _db.Projects
      .GroupBy(x => x.ProjectType.Id)
      .Select(x => new
      {
        ProjectTypeId = x.Key, Count = x.Count()
      })
      .ToDictionaryAsync(x => x.ProjectTypeId, x => x.Count);

    var list = await _db.ProjectTypes.AsNoTracking()
      .Include(x => x.Stage)
      .ToListAsync();

    var stageOrders = list.Select(x => x.Stage.SortOrder).Distinct().ToList();
    var permissions = await _stage.ListPermissionsByStages(stageOrders, ProjectTypeDefaults.StageType);

    return list.Select(x => new ProjectTypeModel(
      x,
      projectCounts.GetValueOrDefault(x.Id, 0),
      permissions.GetValueOrDefault(x.Stage.SortOrder, new List<string>())
    )).ToList();
  }

  /// <summary>
  /// Get a project type with project count.
  /// </summary>
  /// <param name="id">Project type id.</param>
  /// <returns>Project type.</returns>
  public async Task<ProjectTypeModel> Get(int id)
  {
    var projectType = await _db.ProjectTypes.AsNoTracking()
                        .Where(x => x.Id == id)
                        .Include(x => x.Stage)
                        .FirstOrDefaultAsync()
                      ?? throw new KeyNotFoundException("Project type not found.");

    var projectCount = await _db.Projects.CountAsync(p => p.ProjectType.Id == id);
    var stagePermissions = await _stage.ListPermissions(projectType.Stage.SortOrder, ProjectTypeDefaults.StageType);

    return new ProjectTypeModel(projectType, projectCount, stagePermissions);
  }

  /// <summary>
  /// Advance the stage of a project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <param name="set">Stage to advance to. If null, the next stage will be used.</param>
  public async Task AdvanceStage(int id, string? set = null)
  {
    var entity = await _db.ProjectTypes
                   .Include(x => x.Stage).ThenInclude(x => x.NextStage)
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException("Project type not found.");

    var nextStage = await _stage.GetStageToAdvanceTo(entity.Stage, ProjectTypeDefaults.StageType, set);
    if (nextStage is not null)
    {
      entity.Stage = nextStage;
      await _db.SaveChangesAsync();
    }
  }

  /// <summary>
  /// Import sections and fields from another project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <param name="fromId">Project type ID to import from.</param>
  public async Task Import(int id, int fromId)
  {
    var isValidSourceAndTarget = await _db.ProjectTypes
      .Where(x => x.Id == id || x.Id == fromId)
      .CountAsync() == 2;

    if (!isValidSourceAndTarget)
    {
      throw new KeyNotFoundException("Project type not found.");
    }

    var existingSections = await _section.ListByProjectType(fromId);
    foreach (var sectionType in existingSections.GroupBy(x => x.SectionType.Id).ToList())
    {
      var sectionsModel = sectionType
        .Select(x => new SaveSectionModel(null, x.Name, x.SortOrder))
        .ToList();

      await _section.Save(new SaveSectionsModel(id, sectionType.Key, sectionsModel));
    }

    var sections = await _section.ListByProjectType(id);
    foreach (var section in sections)
    {
      var fromSection = existingSections.FirstOrDefault(x =>
        x.SectionType.Id == section.SectionType.Id &&
        x.Name == section.Name
      );

      if (fromSection is null)
      {
        continue;
      }

      var fields = await _field.ListBySection(fromSection.Id);
      var fieldMap = fields.ToDictionary(x => x.Id, x => x);
      var childIds = new HashSet<int>(
        fields.Where(x => x.TriggerField is not null)
          .Select(x => x.TriggerField!.Id)
      );

      var parentFields = fields.Where(x => !childIds.Contains(x.Id)).ToList();

      var sectionFieldsModel = parentFields
        .Select(x => MapFieldToCreateModel(x, fieldMap))
        .ToList();

      await _field.SaveSectionFields(section.Id, sectionFieldsModel);
    }
  }

  /// <summary>
  /// Map field to a model.
  /// </summary>
  private CreateSectionFieldModel MapFieldToCreateModel(FieldModel field, Dictionary<int, FieldModel> fieldMap)
  {
    CreateSectionFieldModel? triggerField = null;

    if (field.TriggerField is not null && fieldMap.TryGetValue(field.TriggerField.Id, out var child))
    {
      triggerField = MapFieldToCreateModel(child, fieldMap);
    }

    return new CreateSectionFieldModel(
      null,
      field.InputType.Id,
      field.Mandatory,
      field.Name,
      field.DefaultResponse,
      field.SortOrder,
      field.Hidden,
      field.SelectFieldOptions?
        .Select(x => new CreateSelectFieldOptionModel(null, x.Name))
        .ToList()
      ?? new List<CreateSelectFieldOptionModel>(),
      field.TriggerField?.Value,
      triggerField
    );
  }
}
