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
public class SectionController : ControllerBase
{
  private readonly SectionService _sectionService;
  private readonly UserManager<ApplicationUser> _users;

  public SectionController(SectionService sectionService, UserManager<ApplicationUser> users)
  {
    _sectionService = sectionService;
    _users = users;
  }

  [HttpGet]
  public async Task<ActionResult<List<SectionModel>>> List(int projectId)
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
