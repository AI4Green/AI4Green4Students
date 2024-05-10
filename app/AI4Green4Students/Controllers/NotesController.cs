using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Note;
using AI4Green4Students.Models.Section;
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
  /// Get note. Only the owner or instructor can view the note.
  /// </summary>
  /// <param name="noteId">Id of the note.</param>
  /// <returns>Plan</returns>
  [HttpGet]
  public async Task<ActionResult<NoteModel>> Get(int noteId)
  {
    try
    {
      var userId = _users.GetUserId(User);

      if (userId is not null && (
            await _notes.IsNoteOwner(userId, noteId) ||
            User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)))
      {
        var note = await _notes.Get(noteId);
        return note.Plan.Stage == PlanStages.Approved ? note : Forbid();
      }
      return Unauthorized();
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
      var isAuthorised = User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments) ||
                         (userId is not null &&
                          User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments) &&
                          await _notes.IsNoteOwner(userId, noteId));

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
}
