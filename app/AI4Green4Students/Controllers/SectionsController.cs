using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SectionsController : ControllerBase
{
  private readonly SectionService _sectionService;

  public SectionsController(SectionService sectionService, UserManager<ApplicationUser> users)
  {
    _sectionService = sectionService;
  }

  [HttpGet]
  public async Task<ActionResult<List<SectionSummaryModel>>> List(int projectId, int experimentId)
  {
    try 
    {
      return await _sectionService.List(projectId, experimentId);
    }
    catch(KeyNotFoundException) 
    {
      return NotFound();
    }
  }
}
