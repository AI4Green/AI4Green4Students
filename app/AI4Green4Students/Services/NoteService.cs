using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Note;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class NoteService
{
  private readonly ApplicationDbContext _db;
  private readonly SectionService _sections;

  public NoteService(ApplicationDbContext db, SectionService sections)
  {
    _db = db;
    _sections = sections;
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
                 .SingleOrDefaultAsync()
               ?? throw new KeyNotFoundException();

    return new NoteModel(note);
  }

  /// <summary>
  /// Get note field responses for a given note. 
  /// </summary>
  /// <param name="noteId">Id of the note to get field responses for.</param>
  /// <returns> A list of note field responses. </returns>
  public async Task<List<FieldResponse>> GetNoteFieldResponses(int noteId)
  {
    var excludedInputTypes = new List<string> { InputTypes.Content, InputTypes.Header };
    return await _db.Notes
             .AsNoTracking()
             .Where(x => x.Id == noteId)
             .SelectMany(x => x.NoteFieldResponses
               .Select(y => y.FieldResponse))
             .Where(fr => !excludedInputTypes.Contains(fr.Field.InputType.Name))
             .Include(x => x.FieldResponseValues)
             .Include(x => x.Field)
             .ThenInclude(x => x.Section)
             .ToListAsync()
           ?? throw new KeyNotFoundException();
  }


  /// <summary>
  /// Get a note section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="noteId">Id of the note to get the field responses for</param>
  /// <returns>Note section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetNoteFormModel(int sectionId, int noteId)
  {
    var section = await _sections.Get(sectionId);
    var sectionFields = await _sections.GetSectionFields(sectionId);
    var noteFieldResponses = await GetNoteFieldResponses(noteId);
    
    return _sections.GetFormModel(section, sectionFields, noteFieldResponses);
  }

  /// <summary>
  /// Save note section form. Also, create new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns>Updated note section form model.</returns>
  public async Task<SectionFormModel> SaveNote(SectionFormSubmissionModel model)
  {
    var selectedFieldResponses = await _sections.GetSectionFieldResponses(model.SectionId, model.RecordId);

    _sections.UpdateDraftFieldResponses(model, selectedFieldResponses);

    await _db.SaveChangesAsync();

    if (model.NewFieldResponses.Count != 0)
    {
      var fields = await _sections.GetSectionFields(model.SectionId);
      var selectedFields = fields.Where(x => model.NewFieldResponses.Any(y=>y.Id == x.Id)).ToList();
      var note = await _db.Notes.FindAsync(model.RecordId) ?? throw new KeyNotFoundException();
      await _sections.CreateFieldResponses(note, selectedFields, model.NewFieldResponses);
    }

    return await GetNoteFormModel(model.SectionId, model.RecordId);
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
