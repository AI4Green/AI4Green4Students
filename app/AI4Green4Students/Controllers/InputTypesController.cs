namespace AI4Green4Students.Controllers;

using Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InputTypesController : ControllerBase
{
  private readonly InputTypeService _inputTypes;

  public InputTypesController(InputTypeService inputTypes) => _inputTypes = inputTypes;

  /// <summary>
  /// Lists all input types.
  /// </summary>
  /// <returns>Input types.</returns>
  [Authorize(nameof(AuthPolicies.CanEditProjectTypes))]
  [HttpGet]
  public async Task<ActionResult> List() => Ok(await _inputTypes.List());

  /// <summary>
  /// Get an input type by ID.
  /// </summary>
  /// <param name="id">Input type ID.</param>
  /// <returns>Input type.</returns>
  [Authorize(nameof(AuthPolicies.CanEditProjectTypes))]
  [HttpGet]
  public async Task<ActionResult> Get(int id)
  {
    try
    {
      return Ok(await _inputTypes.Get(id));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
