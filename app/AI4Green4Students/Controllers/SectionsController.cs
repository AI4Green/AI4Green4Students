using AI4Green4Students.Auth;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SectionsController : ControllerBase
{
  private readonly SectionService _sections;

  public SectionsController(SectionService sections)
  {
    _sections = sections;
  }

  [HttpGet("ListSectionSummaries")]
  public async Task<ActionResult<List<SectionSummaryModel>>> List(int experimentId)
  {
    try
    {
      return await _sections.List(experimentId);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  [HttpGet("GetSectionForm")]
  public async Task<ActionResult<SectionFormModel>> Get(int sectionId, int experimentId)
  {
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewOwnExperiments))
        return await _sections.GetFormModel(sectionId, experimentId);
      else
        return Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

}
