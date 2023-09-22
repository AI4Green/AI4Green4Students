using AI4Green4Students.Auth;
using AI4Green4Students.Models;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationRulesController : ControllerBase
{
  private readonly RegistrationRuleService _registrationRules;
  private readonly UserService _user;
  
  public RegistrationRulesController(
    RegistrationRuleService registrationRules,
    UserService user
    )
  {
    _registrationRules = registrationRules;
    _user = user;
  }
  
  /// <summary>
  /// Get Registration rules list
  /// </summary>
  /// <returns>Registration rules list</returns>
  [Authorize(nameof(AuthPolicies.CanViewRegistrationRules))]
  [HttpGet]
  public async Task<List<RegistrationRuleModel>> List() 
    => await _registrationRules.List();


  /// <summary>
  /// Get registration rule based on rule id
  /// </summary>
  /// <param name="id">Rule id to get</param>
  /// <returns>Registration rules associated with the value</returns>
  [Authorize(nameof(AuthPolicies.CanViewRegistrationRules))]
  [HttpGet("{id}")]
  public async Task<RegistrationRuleModel> Get(int id)
  => await _registrationRules.Get(id);
  
  
  /// <summary>
  /// Delete registration rule
  /// </summary>
  /// <param name="id">Rule id to delete</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanDeleteRegistrationRules))]
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      await _registrationRules.Delete(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Create registration rule or update if value already exists
  /// </summary>
  /// <param name="model">Rule data</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanCreateRegistrationRules))]
  [HttpPost]
  public async Task<ActionResult> Create(CreateRegistrationRuleModel model)
  {
    return Ok(await _registrationRules.Create(model));
  }
  
  
  /// <summary>
  /// Update registration rule
  /// </summary>
  /// <param name="id">Rule id to update</param>
  /// <param name="model">Rule update data</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanEditRegistrationRules))]
  [HttpPut("{id}")]
  public async Task<ActionResult> Set(int id, [FromBody] CreateRegistrationRuleModel model)
  {
    try
    {
      return Ok(await _registrationRules.Set(id, model));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Validate email
  /// </summary>
  /// <returns>Return validation outcome. If valid True or else False</returns>
  [HttpPost("validate")]
  public async Task<IActionResult> ValidateEmail([FromBody] string email)
  {
    return Ok(new EmailValidationResult(await _user.CanRegister(email)));
  }
}
