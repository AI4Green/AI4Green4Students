using AI4Green4Students.Models.Field;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FieldsController : ControllerBase
{
  private readonly FieldService _fields;

  public FieldsController(FieldService fields)
  {
    _fields = fields;
  }

  [HttpGet]
  public async Task<ActionResult<FieldModel>> Get(int fieldId)
  {
    try
    {
      return await _fields.Get(fieldId);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a field by name.
  /// Project Id and Section Type are required to ensure the field is unique.
  /// </summary>
  /// <param name="projectId">Project Id</param>
  /// <param name="sectionType">Section type name (e.g Plan, Note)</param>
  /// <param name="fieldName">Field Name</param>
  /// <returns></returns>
  [HttpGet("{projectId}/{sectionType}/{fieldName}")]
  public async Task<ActionResult<FieldModel>> GetByName(int projectId, string sectionType, string fieldName)
  {
    try
    {
      return await _fields.GetByName(projectId, sectionType, fieldName);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
