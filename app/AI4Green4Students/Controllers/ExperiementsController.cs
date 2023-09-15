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
  /// Get experiment list
  /// </summary>
  /// <returns>Experiment list</returns>
  [HttpGet]
  public async Task<ActionResult<List<ExperimentModel>>> List()
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experiments.ListByUser(userId): Forbid();     
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get experiment list by user
  /// </summary>
  /// <returns>Experiment list</returns>
  [HttpGet("/user/{userId}")]
  public async Task<ActionResult<List<ExperimentModel>>> ListByUser(string userId)
  {
    try
    {
      return await _experiments.ListByUser(userId);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Get experiment by experiment id
  /// </summary>
  /// <param name="id">Experiment id to get</param>
  /// <returns>Experiment matching the id</returns>
  [HttpGet("{id}")]
  public async Task<ActionResult<ExperimentModel>> Get(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      return userId is not null ? await _experiments.GetByUser(id, userId) : Forbid();      
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get user's experiment by experiment id
  /// </summary>
  /// <param name="id">Experiment id to get</param>
  /// <param name="userId">User id of the experiment owner</param>
  /// <returns>Experiment matching the id</returns>
  [HttpGet("{id}/user/{userId}")]
  public async Task<ActionResult<ExperimentModel>> Get(int id, string userId)
  {
    try
    {
      var user = await _users.FindByIdAsync(userId);
      return user is not null ? await _experiments.GetByUser(id, userId) : Forbid();      
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  
  /// <summary>
  /// Delete experiment
  /// </summary>
  /// <param name="id">Experiment id to delete</param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      await _experiments.Delete(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Create experiment
  /// </summary>
  /// <param name="model">Experiment data</param>
  /// <returns></returns>
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
  
}
