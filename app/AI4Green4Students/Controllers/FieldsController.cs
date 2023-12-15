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
}
