using System.Globalization;
using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.ProjectGroup;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectGroupsController : ControllerBase
{
  private readonly ProjectGroupService _projectGroups;
  private readonly UserManager<ApplicationUser> _users;

  public ProjectGroupsController(ProjectGroupService projects, UserManager<ApplicationUser> users)
  {
    _projectGroups = projects;
    _users = users;
  }

  /// <summary>
  /// Get Project group list based on user permission
  /// </summary>
  /// <returns>Project group list</returns>
  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  public async Task<ActionResult<List<ProjectGroupModel>>> List()
  {
    if(User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllProjects))
      return await _projectGroups.ListAll();
    
    var userId = _users.GetUserId(User);
    return userId is not null ? await _projectGroups.ListByUser(userId) : Forbid();
  }
  
  
  /// <summary>
  /// Get project group based on project group id and user permission
  /// </summary>
  /// <param name="id">Project id to get</param>
  /// <returns>Project associated with the id</returns>
  [HttpGet("{id}")]
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  public async Task<ActionResult<ProjectGroupModel>> Get(int id)
  { 
    try
    {
      if (User.HasClaim(CustomClaimTypes.SitePermission, SitePermissionClaims.ViewAllProjects))
        return await _projectGroups.Get(id);
    
      var userId = _users.GetUserId(User);
      return userId is not null ? await _projectGroups.GetByUser(id, userId) : Forbid();      
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Delete project group
  /// </summary>
  /// <param name="id">Project group id to delete</param>
  /// <returns></returns>
  [HttpDelete("{id}")]
  [Authorize(nameof(AuthPolicies.CanDeleteProjects))]
  public async Task<ActionResult> Delete(int id)
  {
    try
    {
      await _projectGroups.Delete(id);
      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  
  /// <summary>
  /// Create project group or update if project group associate with the project already exist
  /// </summary>
  /// <param name="model">Project group data</param>
  /// <returns></returns>
  [HttpPost]
  [Authorize(nameof(AuthPolicies.CanCreateProjects))]
  public async Task<ActionResult> Create(CreateProjectGroupModel model)
  {
    try
    {
      return Ok(await _projectGroups.Create(model));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
    catch (InvalidOperationException)
    {
      return Conflict();
    }
  }
  
  
  /// <summary>
  /// Update project group
  /// </summary>
  /// <param name="id">Project group id to update</param>
  /// <param name="model">Project group update data</param>
  /// <returns></returns>
  [HttpPut("{id}")]
  [Authorize(nameof(AuthPolicies.CanEditProjects))]
  public async Task<ActionResult> Set(int id, CreateProjectGroupModel model)
  {
    try
    {
      return Ok(await _projectGroups.Set(id, model));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Bulk invite students to project group
  /// </summary>
  /// <param name="id">Project Group id</param>
  /// <param name="model">Bulk students invite data</param>
  /// <returns></returns>
  [HttpPut ("{id}/invite-students")]
  [Authorize (nameof(AuthPolicies.CanInviteUsers))]
  [Authorize(nameof(AuthPolicies.CanInviteStudents))]
  public async Task<ActionResult> InviteStudents(int id, InviteStudentModel model)
  {
    try
    {
      return Ok(await _projectGroups.InviteStudents(id, model, CultureInfo.CurrentUICulture.Name));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Remove student from project group
  /// </summary>
  /// <param name="id">Project group id to remove student from</param>
  /// <param name="model">Data for removing student</param>
  /// <returns></returns>
  [HttpPut("{id}/remove-student")]
  [Authorize(nameof(AuthPolicies.CanEditProjects))]
  public async Task<ActionResult> RemoveStudent(int id, RemoveStudentModel model)
  {
    try
    {
      return Ok(await _projectGroups.RemoveStudent(id, model));
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
