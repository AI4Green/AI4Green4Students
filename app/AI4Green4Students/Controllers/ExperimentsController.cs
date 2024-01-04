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
public class ExperimentsController : ControllerBase
{
  private readonly ExperimentService _experiments;
  private readonly UserManager<ApplicationUser> _users;

  public ExperimentsController(ExperimentService experiments, UserManager<ApplicationUser> users)
  {
    _experiments = experiments;
    _users = users;
  }
  
  /// <summary>
  /// Get a list of experiments for a project.
  /// </summary>
  /// <param name="projectId">The id of the project to list experiments for.</param>
  /// <returns>List of experiments matching the project id.</returns>
  [HttpGet]
  public async Task<ActionResult<List<ExperimentModel>>> List(int projectId)
  {
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)) // if user can view all
        return await _experiments.ListByProject(projectId);
      
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experiments.ListByUser(userId, projectId): Forbid();     
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get an experiment by its id.
  /// </summary>
  /// <param name="id">The id of an experiment to retrieve.</param>
  /// <returns>Experiment matching the provided id.</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<ExperimentModel>> Get(int id)
  {
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllExperiments)) // if user can view all
        return await _experiments.Get(id);
      
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experiments.GetByUser(id, userId) : Forbid();      
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Delete an experiment.
  /// </summary>
  /// <param name="id">The id of an experiment to delete.</param>
  /// <returns>If the deletion is successful then no content</returns>
  [Authorize(nameof(AuthPolicies.CanDeleteOwnExperiments))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();
      
      await _experiments.Delete(id, userId);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Create a new experiment.
  /// </summary>
  /// <param name="model">Experiment data to create.</param>
  /// <returns>Newly created experiment.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPost]
  public async Task<ActionResult<ExperimentModel>> Create(CreateExperimentModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experiments.Create(model, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Update an experiment by its id.
  /// </summary>
  /// <param name="id">The id of an experiment to update.</param>
  /// <param name="model">Experiment data for updating experiment.</param>
  /// <returns>Updated experiment.</returns>
  [Authorize(nameof(AuthPolicies.CanEditOwnExperiments))]
  [HttpPut("{id}")]
  public async Task<ActionResult<ExperimentModel>> Set(int id, CreateExperimentModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experiments.Set(id, model, userId) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
