namespace AI4Green4Students.Services;

using Constants;
using Data;
using Data.DefaultExperimentSeeding;
using Data.Entities.SectionTypeData;
using EmailServices;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Models.Emails;
using Models.Note;
using Models.Section;
using SectionTypeData;

public class NoteService : BaseSectionTypeService<Note>
{
  private readonly ApplicationDbContext _db;
  private readonly ProjectGroupEmailService _emailService;
  private readonly FieldResponseService _fieldResponses;
  private readonly StageService _stages;

  public NoteService(
    ApplicationDbContext db,
    StageService stages,
    SectionFormService sectionForm,
    FieldResponseService fieldResponses,
    ProjectGroupEmailService emailService) : base(db, sectionForm)
  {
    _db = db;
    _stages = stages;
    _fieldResponses = fieldResponses;
    _emailService = emailService;
  }

  /// <summary>
  /// List user's project notes.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">User id.</param>
  /// <returns>List user's notes.</returns>
  public async Task<List<NoteModel>> ListByUser(int id, string userId)
  {
    var notes = await Query().AsNoTracking()
      .Where(x => x.Plan.Project.Id == id && x.Plan.Owner.Id == userId)
      .ToListAsync();

    if (notes.Count == 0)
    {
      return new List<NoteModel>();
    }

    var stageOrders = notes.Select(x => x.Stage.SortOrder).Distinct().ToList();
    var permissions = await _stages.ListPermissionsByStages(stageOrders, SectionTypes.Note);
    var reactionNames = await ListReactionNames(id, notes.Select(x => x.Id).ToList());

    return notes.Select(x => new NoteModel(
      x,
      permissions.GetValueOrDefault(x.Stage.SortOrder, new List<string>()),
      reactionNames.GetValueOrDefault(x.Id, null)
    )).ToList();
  }

  /// <summary>
  /// Get a note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <returns>Note.</returns>
  public async Task<NoteModel> Get(int id)
  {
    var note = await Query().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id)
               ?? throw new KeyNotFoundException();

    var reactionNames = await ListReactionNames(note.Project.Id, [note.Id]);

    return new NoteModel(
      note,
      await _stages.ListPermissions(note.Stage.SortOrder, SectionTypes.Note),
      reactionNames.GetValueOrDefault(note.Id, null)
    );
  }

  /// <summary>
  /// Lock all notes for a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  public async Task LockProjectGroupNotes(int id)
  {
    var notes = await _db.Notes.AsNoTracking()
      .Where(x => x.Project.ProjectGroups.Any(y => y.Id == id))
      .Select(x => new
      {
        x.Id, x.Stage.DisplayName
      })
      .ToListAsync();

    foreach (var note in notes)
    {
      if (note.DisplayName == Stages.Locked)
      {
        continue;
      }

      await _stages.Advance<Note>(note.Id, Stages.Locked);
    }
  }

  /// <summary>
  /// Advance the stage of a note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <param name="setStage">Stage to set.</param>
  public async Task AdvanceStage(int id, string? setStage = null)
  {
    var stage = await _stages.Advance<Note>(id, setStage);

    if (stage is null)
    {
      throw new InvalidOperationException();
    }
  }

  /// <summary>
  /// Request feedback for a specific note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <param name="request">Request context model.</param>
  public async Task RequestFeedback(int id, RequestContextModel request)
  {
    var entity = await _db.Notes.SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException("Note not found.");

    if (entity.FeedbackRequested)
    {
      throw new InvalidOperationException("Cannot request feedback - Note currently awaiting feedback");
    }

    entity.FeedbackRequested = true;
    _db.Notes.Update(entity);
    await _db.SaveChangesAsync();

    var model = await GetEntityModelForFeedback(id);

    foreach (var (_, name, email) in model.Instructors)
    {
      if (email is null)
      {
        continue;
      }

      var emailAddress = new EmailAddress(email)
      {
        Name = name
      };

      await _emailService.SendNoteFeedbackRequest(
        emailAddress,
        model.Owner.Name,
        model.Project.Name,
        ClientRoutes.NoteOverview(model.Project.Id, model.ProjectGroup.Id, id).ToLocalUrlString(request),
        name,
        model.Title
      );
    }
  }

  /// <summary>
  /// Complete feedback for a specific note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <param name="userId">Instructor user id.</param>
  /// <param name="request">Request context model.</param>
  public async Task CompleteFeedback(int id, string userId, RequestContextModel request)
  {
    var entity = await _db.Notes.SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException("Note not found.");

    if (!entity.FeedbackRequested)
    {
      throw new InvalidOperationException("Cannot complete feedback - Feedback has not been requested for this note");
    }

    entity.FeedbackRequested = false;
    _db.Notes.Update(entity);
    await _db.SaveChangesAsync();

    var model = await GetEntityModelForFeedback(id);
    var emailAddress = new EmailAddress(model.Owner.Email!)
    {
      Name = model.Owner.Name
    };

    await _emailService.SendNoteFeedbackComplete(
      emailAddress,
      model.Owner.Name,
      model.Project.Name,
      ClientRoutes.NoteOverview(model.Project.Id, model.ProjectGroup.Id, id).ToLocalUrlString(request),
      model.Instructors.First(x => x.Id == userId).Name,
      model.Title
    );
  }

  /// <summary>
  /// Get field response for a field from a note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <param name="fieldId">Field id.</param>
  /// <returns>Field response.</returns>
  public async Task<FieldResponseModel> GetFieldResponse(int id, int fieldId)
  {
    var list = await _fieldResponses.ListByField<Note>([id], fieldId);
    if (list.Count == 0)
    {
      throw new KeyNotFoundException("Field response not found.");
    }
    return list.First();
  }

  /// <summary>
  /// Base note query.
  /// </summary>
  private IQueryable<Note> Query()
    => _db.Notes
      .Include(x => x.Project)
      .Include(x => x.Stage)
      .Include(x => x.Owner)
      .Include(x => x.Plan.Project)
      .Include(x => x.Plan.Owner)
      .Include(x => x.Plan.Stage);

  /// <summary>
  /// List reaction names for a list of notes.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="ids">Note ids.</param>
  /// <returns>Dictionary of note ids and reaction names.</returns>
  private async Task<Dictionary<int, string?>> ListReactionNames(int projectId, List<int> ids)
  {
    var metadataSection = DefaultExperimentConstants.MetadataSection;
    var reactionNameField = DefaultExperimentConstants.ReactionNameField;

    var reactionNameFieldId = await GetFieldId(projectId, metadataSection, reactionNameField);
    if (!reactionNameFieldId.HasValue)
    {
      return ids.ToDictionary(id => id, _ => (string?)null);
    }

    var response = await _fieldResponses.ListByField<Note>(ids, reactionNameFieldId.Value);
    return response.ToDictionary(x => x.EntityId, x => x.Value?.ToString());
  }

  /// <summary>
  /// Get field id using the field name, section name and project id.
  /// Field name and section type are enough in most cases,
  /// but project id and section name absolutely ensure uniqueness.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="sectionName">Section name.</param>
  /// <param name="fieldName">Field name to get the id for.</param>
  /// <returns>Field id.</returns>
  private async Task<int?> GetFieldId(int projectId, string sectionName, string fieldName)
    => (await _db.Fields.AsNoTracking()
      .SingleOrDefaultAsync(x =>
        x.Section.Project.Id == projectId &&
        x.Section.SectionType.Name == SectionTypes.Note &&
        x.Section.Name == sectionName &&
        x.Name.ToLower() == fieldName.ToLower()
      ))?.Id;

  /// <summary>
  /// Get a note feedback model.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <returns>Feedback model.</returns>
  private async Task<NoteFeedbackModel> GetEntityModelForFeedback(int id)
  {
    var note = await _db.Notes.AsNoTracking()
                 .Include(x => x.Owner)
                 .ThenInclude(y => y.ProjectGroups).ThenInclude(z => z.Project)
                 .Include(x => x.Project).ThenInclude(y => y.Instructors)
                 .Include(x => x.Plan)
                 .AsSplitQuery()
                 .FirstOrDefaultAsync(x => x.Id == id)
               ?? throw new KeyNotFoundException("Note not found.");

    return new NoteFeedbackModel(note);
  }
}
