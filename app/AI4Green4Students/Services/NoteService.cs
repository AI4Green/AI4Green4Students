using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.DefaultExperimentSeeding;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Note;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class NoteService
{
  private readonly ApplicationDbContext _db;
  private readonly SectionFormService _sectionForm;

  public NoteService(ApplicationDbContext db, SectionFormService sectionForm)
  {
    _db = db;
    _sectionForm = sectionForm;
  }

  /// <summary>
  /// Get a list of project notes for a given user.
  /// </summary>
  /// <param name="projectId">Id of the project to get notes for.</param>
  /// <param name="userId">Id of the user to get notes for.</param>
  /// <returns>List of project notes of the user.</returns>
  /// <remarks>Note is associated with a plan</remarks>
  public async Task<List<NoteModel>> ListByUser(int projectId, string userId)
  {
    var notes = await GetNotesQuery()
      .Where(x => x.Plan.Project.Id == projectId && x.Plan.Owner.Id == userId)
      .ToListAsync();

    var list = new List<NoteModel>();
    foreach (var note in notes)
    {
      list.Add(new NoteModel(note) { ReactionName = await GetReactionName(projectId, note.Id) });
    }

    return list;
  }

  /// <summary>
  /// Get a note by its id.
  /// </summary>
  /// <param name="id">Id of the note</param>
  /// <returns>Note matching the id.</returns>
  public async Task<NoteModel> Get(int id)
  {
    var note = await GetNotesQuery().SingleOrDefaultAsync(x => x.Id == id)
               ?? throw new KeyNotFoundException();

    return new NoteModel(note) { ReactionName = await GetReactionName(note.Plan.Project.Id, note.Id) };
  }

  /// <summary>
  /// Get a note section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="noteId">Id of the note to get the field responses for</param>
  /// <returns>Note section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int noteId, int sectionId)
  {
    var fieldsResponses = await _sectionForm.ListBySection<Note>(noteId, sectionId);
    return await _sectionForm.GetFormModel(sectionId, fieldsResponses);
  }

  /// <summary>
  /// Save note section form. Also, create new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns>Updated note section form model.</returns>
  public async Task<SectionFormModel> SaveForm(SectionFormPayloadModel model)
  {
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _sectionForm.GenerateFieldResponses(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _sectionForm.GenerateFieldResponses(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses)
    };
    
    var fieldResponses = await _sectionForm.ListBySection<Note>(submission.RecordId, submission.SectionId);

    var updatedValues = _sectionForm.UpdateDraftFieldResponses(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();
    
    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId, submission.SectionId);
    
    var entity = await _db.Notes.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    entity.FieldResponses = await _sectionForm.CreateFieldResponse(submission.RecordId, SectionTypes.Note, submission.NewFieldResponses);

    return await GetSectionForm(model.RecordId, model.SectionId);
  }
  
  /// <summary>
  /// Check if a given user is the owner.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="noteId">Id of the note to check the user against.</param>
  /// <returns>True if the user is the owner of the note's plan, false otherwise.</returns>
  public async Task<bool> IsNoteOwner(string userId, int noteId)
    => await _db.Notes
      .AsNoTracking()
      .AnyAsync(x => x.Id == noteId && x.Plan.Owner.Id == userId);

  /// <summary>
  /// Get field response for a field from a plan.
  /// </summary>
  /// <param name="noteId">Note id.</param>
  /// <param name="fieldId">Field id to get the response for.</param>
  /// <returns>Field response.</returns>
  public async Task<FieldResponseModel> GetFieldResponse(int noteId, int fieldId)
    => await _sectionForm.GetFieldResponse<Note>(noteId, fieldId);
  
  /// <summary>
  /// Construct a query to fetch Notes with related entities.
  /// </summary>
  /// <returns>An IQueryable of Note entities.</returns>
  private IQueryable<Note> GetNotesQuery()
  {
    return _db.Notes.AsNoTracking()
      .Include(x => x.Plan)
      .ThenInclude(y => y.Owner)
      .Include(x => x.Plan)
      .ThenInclude(y => y.Project)
      .Include(x => x.Plan)
      .ThenInclude(y => y.Stage);
  }
  
  /// <summary>
  /// Get reaction name from a note.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="noteId">Note id.</param>
  /// <returns>Reaction name.</returns>
  /// <remarks>Assumes the field response to be deserialized as a string.
  /// </remarks>
  private async Task<string?> GetReactionName(int projectId, int noteId)
  {
    var metadataSection = DefaultExperimentConstants.MetadataSection;
    var reactionNameField = DefaultExperimentConstants.ReactionNameField;

    var reactionNameFieldId = await GetFieldId(projectId, metadataSection, reactionNameField);
    if (!reactionNameFieldId.HasValue) return null;
    try
    {
      var response = await GetFieldResponse(noteId, reactionNameFieldId.Value);
      return response.Value.ToString();
    }
    catch (KeyNotFoundException)
    {
      return null; // this is where the field response is not found
    }
  }

  /// <summary>
  /// Get field id using the field name, section name and project id.
  /// Field name and section type are sufficient in most cases but project id and section name absolutely ensure uniqueness.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="sectionName">Note section name.</param>
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
}
