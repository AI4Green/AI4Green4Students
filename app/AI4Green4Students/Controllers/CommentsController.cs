using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Comment;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
  private readonly CommentService _comments;
  private readonly UserManager<ApplicationUser> _users;

  public CommentsController(CommentService comments, UserManager<ApplicationUser> users)
  {
    _comments = comments;
    _users = users;
  }

  [HttpPost]
  [Authorize(nameof(AuthPolicies.CanMakeComments))]
  public async Task<ActionResult> Create(CreateCommentModel model)
  {
    model.User = await _users.GetUserAsync(User);
    model.IsInstructor = User.IsInRole(Roles.Instructor);

    if (model.User == null)
    {
      return Unauthorized();
    }

    return Ok(await _comments.Create(model));
  }

  /// <summary>
  /// Update comment
  /// </summary>
  /// <param name="id">Comment id to update</param>
  /// <param name="model">Comment update data</param>
  /// <returns></returns>
  [HttpPut("{id}")]
  [Authorize(nameof(AuthPolicies.CanEditOwnComments))]
  public async Task<ActionResult> Set(int id, [FromBody] CreateCommentModel model)
  {
    try
    {
      return Ok(await _comments.Set(id, model));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Delete comment
  /// </summary>
  /// <param name="id">Comment id to delete</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnComments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      await _comments.Delete(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// List all comments for a given field response.
  /// </summary>
  /// <param name="id">Field Response id which all the comments belong to.</param>
  /// <returns></returns>
  [HttpGet("field-response/{id}")]
  public async Task<ActionResult> ListByFieldResponse(int id)
  {
    try
    {
      return Ok(await _comments.ListByFieldResponse(id));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Mark comment as read
  /// </summary>
  /// <param name="id">Comment id</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanMarkCommentsAsRead))]
  [HttpPut("{id}/read")]
  public async Task<ActionResult> MarkCommentAsRead(int id)
  {
    try
    {
      await _comments.MarkCommentAsRead(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  ///  Set the approval status of a field response
  /// </summary>
  /// <param name="id">Field response id</param>
  /// <param name="isApproved"> whether the field response is approved or not</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanApproveFieldResponses))]
  [HttpPut("field-response/{id}")]
  public async Task<ActionResult> ApproveFieldResponse(int id, bool isApproved)
  {
    try
    {
      await _comments.ApproveFieldResponse(id, isApproved);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
