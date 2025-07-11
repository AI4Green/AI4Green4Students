namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.Entities;
using Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;
using Models.Plan;
using SectionTypeData;

public class PlanService : BaseSectionTypeService<Plan>
{
  private readonly ApplicationDbContext _db;
  private readonly FieldResponseService _fieldResponses;
  private readonly StageService _stages;

  public PlanService(
    ApplicationDbContext db,
    StageService stages,
    FieldResponseService fieldResponses,
    SectionFormService sectionForms
  ) : base(db, sectionForms)
  {
    _db = db;
    _stages = stages;
    _fieldResponses = fieldResponses;
  }

  /// <summary>
  /// List user's project plans.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>List user's plans.</returns>
  public async Task<List<PlanModel>> ListByUser(int id, string userId)
  {
    var plans = await Query().AsNoTracking().Where(x => x.Owner.Id == userId && x.Project.Id == id).ToListAsync();
    if (plans.Count == 0)
    {
      return new List<PlanModel>();
    }

    foreach (var plan in plans)
    {
      plan.Note ??= await CreateNote(plan.Id);
    }

    var stageOrders = plans.Select(x => x.Stage.SortOrder).Distinct().ToList();
    var planPermissions = await _stages.ListPermissionsByStages(stageOrders, SectionTypes.Plan);
    var notePermissions = await _stages.ListPermissionsByStages(stageOrders, SectionTypes.Note);

    var list = plans.Select(x => new PlanModel(
        x,
        planPermissions.GetValueOrDefault(x.Stage.SortOrder, new List<string>()),
        new PlanNoteModel(
          x.Note.Id,
          x.Note.Stage.DisplayName,
          notePermissions.GetValueOrDefault(x.Note.Stage.SortOrder, new List<string>())
        )))
      .ToList();

    return list;
  }

  /// <summary>
  /// Get a plan.
  /// </summary>
  /// <param name="id">Plan id.</param>
  /// <returns>Plan.</returns>
  public async Task<PlanModel> Get(int id)
  {
    var plan = await Query().AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync() ??
               throw new KeyNotFoundException();

    plan.Note ??= await CreateNote(id);

    return new PlanModel(
      plan,
      await _stages.ListPermissions(plan.Stage.SortOrder, SectionTypes.Plan),
      new PlanNoteModel(
        plan.Note.Id,
        plan.Note.Stage.DisplayName,
        await _stages.ListPermissions(plan.Note.Stage.SortOrder, SectionTypes.Note)
      ));
  }

  /// <summary>
  /// Create a new plan.
  /// </summary>
  /// <param name="userId">User id.</param>
  /// <param name="model">Create model</param>
  /// <returns>Newly created plan.</returns>
  public async Task<PlanModel> Create(string userId, CreatePlanModel model)
  {
    var user = await _db.Users.FindAsync(userId) ?? throw new KeyNotFoundException();
    var pg = await GetProjectGroup(model.ProjectGroupId, userId);
    var draftStage = await GetStage(SectionTypes.Plan, Stages.Draft);
    var noteDraftStage = await GetStage(SectionTypes.Note, Stages.Draft);

    var entity = new Plan
    {
      Title = model.Title,
      Owner = user,
      Project = pg.Project,
      Stage = draftStage,
      Note = new Note
      {
        Owner = user, Project = pg.Project, Stage = noteDraftStage
      }
    };

    entity.FieldResponses = await _fieldResponses.CreateResponses<Plan>(entity.Id, pg.Project.Id);
    entity.Note.FieldResponses = await _fieldResponses.CreateResponses<Note>(entity.Note.Id, pg.Project.Id);

    _db.Plans.Add(entity);
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Advance the stage of a plan.
  /// </summary>
  /// <param name="id">Plan id..</param>
  /// <param name="userId">User advancing.</param>
  /// <param name="setStage">Stage to set.</param>
  public async Task AdvanceStage(int id, string userId, string? setStage = null)
  {
    var plan = await _db.Plans.AsNoTracking()
                 .Include(x => x.Stage)
                 .Include(x => x.Note)
                 .FirstOrDefaultAsync(x => x.Id == id)
               ?? throw new KeyNotFoundException();

    var stage = await _stages.Advance<Plan>(id, setStage);

    if (stage is null)
    {
      throw new InvalidOperationException();
    }

    if (stage.DisplayName == Stages.Approved)
    {
      await CopyReactionSchemeToNote(plan.Id);
      await _stages.Advance<Note>(plan.Note.Id, Stages.InProgress);
    }

    await _stages.SendAdvancementEmail<Plan>(id, userId, plan.Stage.DisplayName);
  }

  /// <summary>
  /// Create a new note for a plan if it doesn't exist.
  /// </summary>
  /// <param name="id">Plan id.</param>
  /// <returns>Note.</returns>
  private async Task<Note> CreateNote(int id)
  {
    var entity = await _db.Notes.Include(x => x.Stage).FirstOrDefaultAsync(x => x.PlanId == id);
    if (entity is not null)
    {
      return entity;
    }

    var note = new Note
    {
      PlanId = id
    };

    await _db.Notes.AddAsync(note);
    await _db.SaveChangesAsync();
    return note;
  }

  /// <summary>
  /// Copies the reaction scheme from a plan to its associated note.
  /// If the note doesn't have a reaction scheme, creates one.
  /// If it exists, update it with the plan's scheme.
  /// </summary>
  private async Task CopyReactionSchemeToNote(int planId)
  {
    var plan = await Get(planId);

    var planReactionScheme = await GetReactionSchemeFieldResponse<Plan>(plan.Id);
    var reactionScheme = planReactionScheme?.FieldResponseValues.MaxBy(x => x.ResponseDate)?.Value;
    if (planReactionScheme is null || reactionScheme is null)
    {
      return;
    }

    var noteReactionScheme = await GetReactionSchemeFieldResponse<Note>(plan.Note.Id);
    if (noteReactionScheme is null)
    {
      await CreateNoteReactionScheme(plan.ProjectId, plan.Note.Id, reactionScheme);
      return;
    }

    await UpdateNoteReactionScheme(noteReactionScheme, reactionScheme);
  }

  /// <summary>
  /// Creates a reaction scheme field response for a note.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="noteId">Note id.</param>
  /// <param name="value">Reaction scheme value.</param>
  private async Task CreateNoteReactionScheme(int projectId, int noteId, string value)
  {
    var reactionSchemeField = await _db.Fields
      .Where(x => x.InputType.Name == InputTypes.ReactionScheme && x.Section.Project.Id == projectId)
      .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Reaction scheme field not found.");

    var note = await _db.Notes.Where(x => x.Id == noteId).FirstOrDefaultAsync() ??
               throw new KeyNotFoundException("Note not found.");

    var fieldResponse = await _fieldResponses.Create(reactionSchemeField, value);
    note.FieldResponses = [fieldResponse];

    _db.Update(note);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Updates a reaction scheme field response for a note.
  /// </summary>
  /// <param name="response">Field response.</param>
  /// <param name="newValue">New value.</param>
  private async Task UpdateNoteReactionScheme(FieldResponse response, string newValue)
  {
    var latestValue = response.FieldResponseValues.MaxBy(x => x.ResponseDate);
    if (latestValue is null)
    {
      await _db.AddAsync(new FieldResponseValue
      {
        FieldResponse = response, Value = newValue
      });
    }
    else
    {
      latestValue.Value = newValue;
      _db.Update(response);
    }
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Gets the reaction scheme field response for a section type.
  /// </summary>
  /// <typeparam name="T">Section type. Either Plan or Note.</typeparam>
  /// <param name="id">Section type id.</param>
  /// <returns>Reaction scheme field response.</returns>
  private async Task<FieldResponse?> GetReactionSchemeFieldResponse<T>(int id) where T : BaseSectionTypeData
    => await _db.Set<T>()
      .Where(x => x.Id == id)
      .SelectMany(x => x.FieldResponses)
      .Where(x => x.Field.InputType.Name == InputTypes.ReactionScheme)
      .Include(x => x.FieldResponseValues)
      .SingleOrDefaultAsync();

  /// <summary>
  /// Base plan query.
  /// </summary>
  private IQueryable<Plan> Query()
    => _db.Plans
      .Include(x => x.Project)
      .Include(x => x.Owner)
      .Include(x => x.Stage)
      .Include(x => x.Note)
      .Include(x => x.Note.Stage);
}
