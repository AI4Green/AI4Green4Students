namespace AI4Green4Students.Controllers;

using Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Field;
using Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FieldsController : ControllerBase
{
  private readonly FieldService _fields;

  public FieldsController(FieldService fields) => _fields = fields;

  /// <summary>
  /// List fields by project type.
  /// </summary>
  /// <param name="id">Project type ID.</param>
  /// <returns>Fields.</returns>
  [HttpGet("projectType/{id}")]
  public async Task<IActionResult> ListByProjectType(int id)
    => Ok(await _fields.ListByProjectType(id));

  /// <summary>
  /// List fields by section.
  /// </summary>
  /// <param name="id">Section ID.</param>
  /// <returns>Fields.</returns>
  [HttpGet("section/{id}")]
  public async Task<IActionResult> ListBySection(int id)
    => Ok(await _fields.ListBySection(id));

  /// <summary>
  /// Get a field.
  /// </summary>
  /// <param name="id">Field ID.</param>
  /// <returns>Field.</returns>
  [HttpGet]
  public async Task<ActionResult<FieldModel>> Get(int id)
  {
    try
    {
      return await _fields.Get(id);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Save section fields.
  /// </summary>
  /// <param name="id">Section ID.</param>
  /// <param name="model">Fields model.</param>
  [Authorize(nameof(AuthPolicies.CanEditProjectTypes))]
  [HttpPost("{id}/save")]
  public async Task<IActionResult> Save(int id, [FromBody] List<CreateSectionFieldModel> model)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }
    try
    {
      await _fields.SaveSectionFields(id, model);
      return NoContent();
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
  }

  /// <summary>
  /// Get a field by name.
  /// </summary>
  /// <param name="projectId">Project ID.</param>
  /// <param name="sectionType">Section type name (e.g. Plan, Note).</param>
  /// <param name="name">Field Name</param>
  /// <returns></returns>
  [HttpGet("{projectId}/{sectionType}/{name}")]
  public async Task<ActionResult<FieldModel>> GetByName(int projectId, string sectionType, string name)
  {
    try
    {
      return await _fields.GetByName(projectId, sectionType, name);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
