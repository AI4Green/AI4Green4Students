using AI4Green4Students.Models.Experiment;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExperimentTypesController : ControllerBase
{
  private readonly ExperimentTypeService _experimentTypes;
  
  public ExperimentTypesController (ExperimentTypeService experimentTypes)
  {
    _experimentTypes = experimentTypes;
  }
  
  /// <summary>
  /// Get experiment types list
  /// </summary>
  /// <returns>Experiment types list</returns>
  [HttpGet]
  public async Task<ActionResult<List<ExperimentTypeModel>>> List()
  => await _experimentTypes.List();
}
