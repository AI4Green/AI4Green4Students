using AI4Green4Students.Constants;
using AI4Green4Students.Data;
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
  /// Get a note by its id.
  /// </summary>
  /// <param name="id">Id of the note</param>
  /// <returns>Note matching the id.</returns>
  public async Task<NoteModel> Get(int id)
  {
    var note = await _db.Notes.AsNoTracking()
                 .Where(x => x.Id == id)
                 .Include(x => x.Plan)
                 .ThenInclude(y=>y.Owner)
                 .Include(x=> x.Plan)
                 .ThenInclude(y=>y.Project)
                 .Include(x => x.Plan)
                 .ThenInclude(y => y.Stage)
                 .SingleOrDefaultAsync()
               ?? throw new KeyNotFoundException();

    return new NoteModel(note);
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
}
