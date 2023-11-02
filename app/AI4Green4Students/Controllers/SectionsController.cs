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
  private readonly UserManager<ApplicationUser> _users;

  public SectionsController(SectionService sectionService, UserManager<ApplicationUser> users)
  {
    _sectionService = sectionService;
    _users = users;
  }

  [HttpGet("{projectId}/{experimentId}")]
  public async Task<ActionResult<List<SectionModel>>> List(int projectId, int experimentId)
  {
    try 
    {
      var userId = _users.GetUserId(User);
      return await _sectionService.List(projectId, userId);
    }
    catch(KeyNotFoundException) 
    {
      return NotFound();
    }
  }
}
