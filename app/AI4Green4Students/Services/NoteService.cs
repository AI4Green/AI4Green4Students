using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.DefaultExperimentSeeding;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Note;
using AI4Green4Students.Models.Section;
using Microsoft.EntityFrameworkCore;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Services.EmailServices;
using AI4Green4Students.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;


namespace AI4Green4Students.Services;

public class NoteService
{
  private readonly ApplicationDbContext _db;
  private readonly StageService _stages;
  private readonly SectionFormService _sectionForm;
  private readonly FieldResponseService _fieldResponses;
  private readonly ProjectGroupEmailService _emailService;
  private readonly UserManager<ApplicationUser> _users;
  private readonly ActionContext _actionContext;

  public NoteService(
    ApplicationDbContext db,
    StageService stages,
    SectionFormService sectionForm,
    FieldResponseService fieldResponses,
    ProjectGroupEmailService emailService,
    UserManager<ApplicationUser> users,
    IActionContextAccessor actionContextAccessor)
  {
    _db = db;
    _stages = stages;
    _sectionForm = sectionForm;
    _fieldResponses = fieldResponses;
    _emailService = emailService;
    _users = users;
    _actionContext = actionContextAccessor.ActionContext
                     ?? throw new InvalidOperationException("Failed to get the ActionContext");
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
    var notes = await NotesQuery().AsNoTracking()
      .Where(x => x.Plan.Project.Id == projectId && x.Plan.Owner.Id == userId)
      .ToListAsync();

    var list = new List<NoteModel>();
    foreach (var note in notes)
    {
      var permissions = await _stages.GetStagePermissions(note.Stage, StageTypes.Note);
      var model = new NoteModel(note)
      {
        ReactionName = await GetReactionName(projectId, note.Id),
        Permissions = permissions
      };
      list.Add(model);
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
    var note = await NotesQuery().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();
    var permissions = await _stages.GetStagePermissions(note.Stage, StageTypes.Note);
    return new NoteModel(note)
    {
      ReactionName = await GetReactionName(note.Project.Id, note.Id),
      Permissions = permissions
    };
  }

  /// <summary>
  /// Lock all notes for a given project group by setting their stage to Locked.
  /// </summary>
  /// <param name="projectGroupId">Project group id to lock notes for.</param>
  /// <returns>Task representing the asynchronous operation.</returns>
  public async Task LockProjectGroupNotes(int projectGroupId)
  {
    var notes = await _db.Notes.AsNoTracking()
      .Where(x => x.Project.ProjectGroups.Any(y => y.Id == projectGroupId))
      .Select(x => new
      {
        x.Id,
        x.Stage.DisplayName
      })
      .ToListAsync();

    foreach (var note in notes)
    {
      if (note.DisplayName == NoteStages.Locked) continue;
      await _stages.AdvanceStage<Note>(note.Id, StageTypes.Note, NoteStages.Locked);
    }
  }
  
  /// <summary>
  /// Get a note section including its fields, last field response and comments.
  /// </summary>
  /// <param name="sectionId">Id of the section to get</param>
  /// <param name="noteId">Id of the note to get the field responses for</param>
  /// <returns>Note section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int noteId, int sectionId)
  {
    var fieldsResponses = await _fieldResponses.ListBySection<Note>(noteId, sectionId);
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
      FieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _fieldResponses.GenerateFieldResponseSubmissionModel(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses, true)
    };
    
    var note = await Get(model.RecordId);
    var fieldResponses = await _fieldResponses.ListBySection<Note>(submission.RecordId, submission.SectionId);

    var updatedValues = _fieldResponses.UpdateDraft(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();
    
    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId, submission.SectionId);
    
    var entity = await _db.Notes.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    var newFieldResponses = await _fieldResponses.CreateResponses<Note>(note.Id, note.ProjectId, SectionTypes.Note, submission.NewFieldResponses);
    entity.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

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
      .AnyAsync(x => x.Id == noteId && x.Owner.Id == userId);

  /// <summary>
  /// Check if a given user is the member of a given project group.
  /// </summary>
  /// <param name="userId">Id of the user viewing.</param>
  /// <param name="noteId">Note id.</param>
  /// <returns>True if the user viewing is the member of the project group, false otherwise.</returns>
  public async Task<bool> IsInSameProjectGroup(string userId, int noteId)
  {
    var note = await Get(noteId);
    
    // Check if both the owner and the viewer are in the same project group
    return await _db.ProjectGroups.AsNoTracking()
      .Where(x => x.Project.Id == note.ProjectId && x.Students.Any(y => y.Id == note.Plan.OwnerId))
      .AnyAsync(x => x.Students.Any(y => y.Id == userId));
  }

  /// <summary>
  /// Check if a given user is the project instructor.
  /// </summary>
  /// <param name="userId">Instructor id to check.</param>
  /// <param name="noteId">Note id.</param>
  /// <returns>True if the user is the instructor, false otherwise.</returns>
  public async Task<bool> IsProjectInstructor(string userId, int noteId)
  {
    var note = await Get(noteId);
    return await _db.Projects.AsNoTracking()
      .AnyAsync(x => x.Id == note.ProjectId && x.Instructors.Any(y => y.Id == userId));
  }

  /// <summary>
  /// Check if a given user is the project group instructor
  /// </summary>
  /// <param name="userId">Instructor id to check.</param>
  /// <param name="projectGroupId">Project group id.</param>
  /// <returns>True if the user is the instructor, false otherwise.</returns>
  public async Task<bool> IsProjectGroupInstructor(string userId, int projectGroupId)
  {
    return await _db.ProjectGroups.AsNoTracking()
      .AnyAsync(x => x.Id == projectGroupId && x.Project.Instructors.Any(y => y.Id == userId));
  }
  
  /// <summary>
  /// Advance the stage of a note.
  /// </summary>
  /// <param name="id">Id of the note to advance the stage for.</param>
  /// <param name="setStage">Stage to set the note to. (Optional)</param>
  /// <returns>Plan with the updated stage.</returns>
  public async Task<NoteModel?> AdvanceStage(int id, string? setStage = null)
  {
    var entity = await _stages.AdvanceStage<Note>(id, StageTypes.Note, setStage);
    
    if (entity?.Stage is null) return null;

    return await Get(id);
  }
  
  /// <summary>
  /// Get field response for a field from a note.
  /// </summary>
  /// <param name="noteId">Note id.</param>
  /// <param name="fieldId">Field id to get the response for.</param>
  /// <returns>Field response.</returns>
  public async Task<FieldResponseModel> GetFieldResponse(int noteId, int fieldId)
    => await _fieldResponses.GetByField<Note>(noteId, fieldId);
  
  /// <summary>
  /// Construct a query to fetch Note along with its related entities.
  /// </summary>
  /// <returns>An IQueryable of Note entities.</returns>
  private IQueryable<Note> NotesQuery()
  {
    return _db.Notes
      .Include(x => x.Project)
      .Include(x => x.Stage)
      .Include(x => x.Owner)
      .Include(x => x.Plan.Project)
      .Include(x => x.Plan.Owner)
      .Include(x => x.Plan.Stage);
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
  /// Request feedback for a specific note.
  /// </summary>
  /// <param name="noteId">The ID of the note to request feedback for.</param>
  /// <returns>Task representing the asynchronous operation.</returns>
  public async Task RequestFeedback(int noteId)
  {
    var note = await _db.Notes
                 .Include(n => n.Owner)
                 .Include(n => n.Owner.ProjectGroups)
                 .Include(n => n.Project)
                 .Include(n => n.Project.Instructors)
                 .Include(n => n.Plan)
                 .SingleOrDefaultAsync(n => n.Id == noteId)
               ?? throw new KeyNotFoundException();
    
    if (note.FeedbackRequested) throw new InvalidOperationException("Cannot request feedback - Note currently awaiting feedback.");
    
    note.FeedbackRequested = true;
    _db.Notes.Update(note);
    await _db.SaveChangesAsync();
    
    // Get required data for sending the email
    var projectGroupId = note.Owner.ProjectGroups.SingleOrDefault();
    if (projectGroupId is null) throw new KeyNotFoundException("Note owner is not in a project group");
    
    var projectName = note.Project.Name;
    var studentName = note.Owner.FullName;
    var projectId = note.Project.Id;
    var planName = note.Plan.Title;
    var noteUrl = ClientRoutes.NoteOverview(projectId, projectGroupId.Id, noteId).ToLocalUrlString(_actionContext.HttpContext.Request);
    
    // Send feedback request email to all instructors of the project
    var instructors = note.Project.Instructors;
    foreach (var instructor in instructors)
    {
      if (instructor.Email is null || !await _users.IsInRoleAsync(instructor, Roles.Instructor)) continue; // skip if email is not set or user is not an instructor

      var emailAddress = new EmailAddress(instructor.Email) { Name = instructor.FullName };
      await _emailService.SendNoteFeedbackRequest(emailAddress, studentName, projectName, noteUrl, instructor.FullName, planName);
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
