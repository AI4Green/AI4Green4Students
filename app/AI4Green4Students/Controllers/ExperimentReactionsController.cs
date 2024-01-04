using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Experiment;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExperimentReactionsController : ControllerBase
{
  private readonly ExperimentReactionService _experimentReaction;
  private readonly UserManager<ApplicationUser> _users;
  
  public ExperimentReactionsController (ExperimentReactionService experimentReaction, UserManager<ApplicationUser> users)
  {
    _experimentReaction = experimentReaction;
    _users = users;
  }
  
  /// <summary>
  /// Get a list of experiment reactions for a specific experiment.
  /// </summary>
  /// <param name="experimentId">The id of the experiment to list reactions for.</param>
  /// <returns>List of experiment reactions matching the experiment Id.</returns>
  [HttpGet]
  public async Task<ActionResult<List<ExperimentReactionModel>>> List(int experimentId)
  { 
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)) // if user can view all
        return await _experimentReaction.ListByExperiment(experimentId);
      
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experimentReaction.ListByUser(userId, experimentId): Forbid();     
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get an experiment reaction by its id.
  /// </summary>
  /// <param name="id">The id of an experiment reaction to retrieve.</param>
  /// <returns>Experiment reaction matching the provided id.</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<ExperimentReactionModel>> Get(int id)
  {
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)) // if user can view all
        return await _experimentReaction.Get(id);
      
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experimentReaction.GetByUser(userId, id) : Forbid();      
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Delete an experiment reaction by its id.
  /// </summary>
  /// <param name="id">The id of an experiment reaction to delete.</param>
  /// <returns>If the deletion is successful then no content</returns>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();
      
      await _experimentReaction.Delete(id, userId);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Create a new experiment reaction.
  /// </summary>
  /// <param name="model">Experiment reaction data to create.</param>
  /// <returns>Newly created experiment reaction.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPost]
  public async Task<ActionResult<ExperimentReactionModel>> Create(CreateExperimentReactionModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experimentReaction.Create(userId, model) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Update an experiment reaction by its id.
  /// </summary>
  /// <param name="id">The id of the experiment reaction to update.</param>
  /// <param name="model">Experiment reaction data for updating experiment reaction.</param>
  /// <returns>Updated experiment reaction.</returns>
  [Authorize(nameof(AuthPolicies.CanEditOwnExperiments))]
  [HttpPut("{id}")]
  public async Task<ActionResult<ExperimentReactionModel>> Set(int id, CreateExperimentReactionModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experimentReaction.Set(userId, id, model) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
