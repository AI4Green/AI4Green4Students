using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Note;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Models.Stage;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
  private readonly NoteService _notes;
  private readonly UserManager<ApplicationUser> _users;

  public NotesController(NoteService notes, UserManager<ApplicationUser> users)
  {
    _notes = notes;
    _users = users;
  }
  
  /// <summary>
  /// Get user's notes list for a given project.
  /// </summary>
  /// <param name="projectId"></param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnExperiments))]
  [HttpGet]
  public async Task<ActionResult<List<NoteModel>>> List(int projectId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _notes.ListByUser(projectId, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Get note. Only the owner or instructor can view the note.
  /// </summary>
  /// <param name="noteId">Id of the note.</param>
  /// <returns>Plan</returns>
  [HttpGet("{noteId}")]
  public async Task<ActionResult<NoteModel>> Get(int noteId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      var isAuthorised = await _notes.IsNoteOwner(userId, noteId) ||
                         await _notes.IsProjectInstructor(userId, noteId) ||
                         await _notes.IsInSameProjectGroup(userId, noteId);

      if (!isAuthorised) return Forbid();
      var note = await _notes.Get(noteId);
      return note.Plan.Stage == PlanStages.Approved ? note : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get field response for a field from a plan.
  /// </summary>
  /// <param name="noteId">Note id.</param>
  /// <param name="fieldId">Field id.</param>
  /// <returns>Field response.</returns>
  [HttpGet("field-response/{noteId}/{fieldId}")]
  public async Task<ActionResult<FieldResponseModel>> GetFieldResponse(int noteId, int fieldId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      var isAuthorised = await _notes.IsNoteOwner(userId, noteId) ||
                         await _notes.IsProjectInstructor(userId, noteId) ||
                         await _notes.IsInSameProjectGroup(userId, noteId);
      
      return isAuthorised ? await _notes.GetFieldResponse(noteId, fieldId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get note section form, which includes section fields and its responses.
  /// </summary>
  /// <param name="sectionId"> Id of section to get form for. </param>
  /// <param name="noteId"> Id of student's note to get field responses for. </param>
  /// <returns>Note section form for the given note matching the given section.</returns> 
  [HttpGet("form/{noteId}/{sectionId}")]
  public async Task<ActionResult<SectionFormModel>> GetSectionForm(int noteId, int sectionId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      var isAuthorised = await _notes.IsNoteOwner(userId, noteId) ||
                         await _notes.IsProjectInstructor(userId, noteId) ||
                         await _notes.IsInSameProjectGroup(userId, noteId);

      return isAuthorised ? await _notes.GetSectionForm(noteId, sectionId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Save the field responses for a section accordingly to the section type.
  /// </summary>
  /// <param name="model"> Section form payload model. </param>
  /// <returns> saved section form data.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPut("save-form")]
  [Consumes("multipart/form-data")]
  public async Task<ActionResult<SectionFormModel>> SaveSectionForm([FromForm] SectionFormPayloadModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = userId is not null &&
                         User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.CreateExperiments) &&
                         await _notes.IsNoteOwner(userId, model.RecordId);

      if (!isAuthorised) return Forbid();

      return await _notes.SaveForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Lock all notes for a given project group, setting their stage to Locked.
  /// Only accessible to users with the CanLockDownOwnProject permission.
  /// </summary>
  /// <param name="projectGroupId">ID of the project group whose notes are to be locked.</param>
  /// <returns>Action result indicating the outcome of the operation.</returns>
  [Authorize(nameof(AuthPolicies.CanLockProjectGroupNotes))]
  [HttpPost("lock-notes/{projectGroupId}")]
  public async Task<ActionResult> LockProjectGroupNotes(int projectGroupId)
  {
    var userId = _users.GetUserId(User);
    if (userId is null) return Forbid();

    var isAuthorised = await _notes.IsProjectGroupInstructor(userId, projectGroupId);

    if (!isAuthorised) return Forbid();

    await _notes.LockProjectGroupNotes(projectGroupId);

    return Ok();
  }

  /// <summary>
  /// Advance the stage of the note
  /// </summary>
  /// <param name="id">The id of the note to advance</param>
  /// <param name="setStage">The stage to advance to</param>
  /// <returns></returns>
  [HttpPost("{id}/AdvanceStage")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var userId = _users.GetUserId(User);
    if (userId is null) return Forbid();

    var isAuthorised = await _notes.IsNoteOwner(userId, id) ||
                       await _notes.IsProjectInstructor(userId, id);

    if (!isAuthorised) return Forbid();

    var nextStage = await _notes.AdvanceStage(id, setStage.StageName);
    if (nextStage is null)
    {
      return Conflict();
    }

    return Ok(nextStage);
  }


}
