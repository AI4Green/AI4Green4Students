using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Note;
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
      return userId is not null && (
        await _notes.IsNoteOwner(userId, noteId) ||
        User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)
      )
        ? await _notes.Get(noteId)
        : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
