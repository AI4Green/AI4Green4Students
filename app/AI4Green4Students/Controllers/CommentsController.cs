using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Comment;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]

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
    model.IsInstructor =  User.IsInRole(Roles.Instructor);

    if (model.User  == null) 
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
  [HttpPut]
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
  /// Get all comments for a given field response
  /// </summary>
  /// <param name="fieldResponse">Field Response which all the comments belong to</param>
  /// <returns></returns>

  [HttpGet]
  public async Task<ActionResult> GetByFieldResponse(int fieldResponse)
  {
    try
    {
      return Ok(await _comments.GetByFieldResponse(fieldResponse));
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
  [HttpPut("read")]
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
}

