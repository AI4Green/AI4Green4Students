namespace AI4Green4Students.Controllers;

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
