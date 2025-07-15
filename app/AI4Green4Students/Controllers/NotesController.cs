namespace AI4Green4Students.Controllers;

using Auth;
using Constants;
using Data.Entities.Identity;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Note;
using Models.Section;
using Models.Stage;
using Services;

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
  /// Get user's notes for a given project.
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
  /// Get a note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <returns>Note.</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<NoteModel>> Get(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _notes.IsOwner(userId, id) ||
                         await _notes.IsProjectInstructor(userId, id) ||
                         await _notes.IsInSameProjectGroup(userId, id);

      if (!isAuthorised)
      {
        return Forbid();
      }

      var note = await _notes.Get(id);
      return note.Plan.Stage == Stages.Approved ? note : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <returns>Note Feedback.</returns>
  [HttpGet("{id}/feedback")]
  public async Task<ActionResult<NoteFeedbackModel>> GetFeeback(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _notes.IsOwner(userId, id) || await _notes.IsProjectInstructor(userId, id);

      if (!isAuthorised)
      {
        return Forbid();
      }

      return await _notes.GetFeedback(id);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
    catch(InvalidOperationException ex)
    {
      return Conflict(ex.Message);
    }
  }

  /// <summary>
  /// Get field response for a field from a note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <param name="fieldId">Field id.</param>
  /// <returns>Field response.</returns>
  [HttpGet("{id}/field-response/{fieldId}")]
  public async Task<ActionResult<FieldResponseModel>> GetFieldResponse(int id, int fieldId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _notes.IsOwner(userId, id) ||
                         await _notes.IsProjectInstructor(userId, id) ||
                         await _notes.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _notes.GetFieldResponse(id, fieldId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a section form.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <param name="sectionId">Section id.</param>
  /// <returns>Section form.</returns>
  [HttpGet("{id}/form/{sectionId}")]
  public async Task<ActionResult<SectionFormModel>> GetSectionForm(int id, int sectionId)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _notes.IsOwner(userId, id) ||
                         await _notes.IsProjectInstructor(userId, id) ||
                         await _notes.IsInSameProjectGroup(userId, id);

      return isAuthorised ? await _notes.GetSectionForm(id, sectionId) : Forbid();
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
                         await _notes.IsOwner(userId, model.RecordId);

      if (!isAuthorised)
      {
        return Forbid();
      }

      return await _notes.SaveSectionForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Lock all notes for a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  [Authorize(nameof(AuthPolicies.CanLockProjectGroupNotes))]
  [HttpPost("lock-notes/{id}")]
  public async Task<ActionResult> LockProjectGroupNotes(int id)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isAuthorised = await _notes.IsProjectInstructor(userId, id);
    if (!isAuthorised)
    {
      return Forbid();
    }

    await _notes.LockProjectGroupNotes(id);

    return NoContent();
  }

  /// <summary>
  /// Advance the stage of the note
  /// </summary>
  /// <param name="id">The id of the note to advance</param>
  /// <param name="setStage">The stage to advance to</param>
  /// <returns></returns>
  [HttpPost("{id}/advance")]
  public async Task<ActionResult> AdvanceStage(int id, SetStageModel setStage)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isAuthorised = await _notes.IsOwner(userId, id) ||
                       await _notes.IsProjectInstructor(userId, id);

    if (!isAuthorised)
    {
      return Forbid();
    }

    try
    {
      await _notes.AdvanceStage(id, setStage.StageName);
      return NoContent();
    }
    catch (KeyNotFoundException e)
    {
      return NotFound(e.Message);
    }
    catch (InvalidOperationException e)
    {
      return Conflict(e.Message);
    }
  }

  /// <summary>
  /// Request feedback for a specific note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <returns>Action result indicating the outcome of the operation.</returns>
  [HttpPost("{id}/request-feedback")]
  public async Task<ActionResult> RequestFeedback(int id)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isAuthorised = await _notes.IsOwner(userId, id);
    if (!isAuthorised)
    {
      return Forbid();
    }

    try
    {
      await _notes.RequestFeedback(id, new RequestContextModel(Request));
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      return Conflict(ex.Message);
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
  }

  /// <summary>
  /// Complete feedback for a specific note.
  /// </summary>
  /// <param name="id">Note id.</param>
  /// <param name="model">Create note's feedback model.</param>
  /// <returns>Action result indicating the outcome of the operation.</returns>
  [HttpPost("{id}/complete-feedback")]
  public async Task<ActionResult> CompleteFeedback(int id, CreateNoteFeedbackModel model)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isAuthorised = await _notes.IsProjectInstructor(userId, id);
    if (!isAuthorised)
    {
      return Forbid();
    }

    try
    {
      await _notes.CompleteFeedback(id, userId, model, new RequestContextModel(Request));
      return NoContent();
    }
    catch (InvalidOperationException ex)
    {
      return Conflict(ex.Message);
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
  }
}
