using AI4Green4Students.Auth;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Extensions;
using AI4Green4Students.Models.ProjectGroup;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
  /// List project groups based on user's role
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>Project group list</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet("project/{id}")]
  public async Task<ActionResult<List<ProjectGroupModel>>> List(int id)
  {
    var userId = _users.GetUserId(User);
    if (userId is null) return Forbid();

    var isInstructor = User.IsInRole(Roles.Instructor);
    return isInstructor
      ? await _projectGroups.ListByInstructor(id, userId)
      : await _projectGroups.ListByStudent(id,userId);
  }
  
  
  /// <summary>
  /// Delete project group
  /// </summary>
  /// <param name="id">Project group id to delete</param>
  /// <returns></returns>
  [Authorize(nameof(AuthPolicies.CanDeleteProjects))]
  [HttpDelete("{id}")]
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
  [Authorize(nameof(AuthPolicies.CanCreateProjects))]
  [HttpPost]
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
  [Authorize(nameof(AuthPolicies.CanEditProjects))]
  [HttpPut("{id}")]
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
  [Authorize (nameof(AuthPolicies.CanInviteUsers))]
  [Authorize(nameof(AuthPolicies.CanInviteStudents))]
  [HttpPut ("{id}/invite-students")]
  public async Task<ActionResult> InviteStudents(int id, InviteStudentModel model)
  {
    try
    {
      return Ok(await _projectGroups.InviteStudents(id, model, Request.GetUICulture().Name));
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
  [Authorize(nameof(AuthPolicies.CanEditProjects))]
  [HttpPut("{id}/remove-student")]
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
  
  /// <summary>
  /// Get project group by id.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <returns>Project group</returns>
  /// <remarks>User must be either project group member or one of the instructor of the project group.</remarks>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet("{id}")]
  public async Task<ActionResult<ProjectGroupModel>> Get(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();
      
      var isAuthorised = await _projectGroups.IsPgProjectInstructor(userId, id) || await _projectGroups.IsProjectGroupMember(userId, id);

      return isAuthorised ? await _projectGroups.Get(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
  
  /// <summary>
  /// Get project group activities section form, which includes section fields and its responses.
  /// </summary>
  /// <param name="id">Project group id to get the field responses for</param>
  /// <returns>Project group activities section form.</returns>
  [Authorize(nameof(AuthPolicies.CanViewProjectGroupExperiments))]
  [HttpGet("form/{id}")]
  public async Task<ActionResult<SectionFormModel>> GetSectionForm(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null) return Forbid();

      var isAuthorised = await _projectGroups.IsPgProjectInstructor(userId, id) || await _projectGroups.IsProjectGroupMember(userId, id);

      return isAuthorised ? await _projectGroups.GetSectionForm(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Save the field responses.
  /// </summary>
  /// <param name="model"> Section form payload model. </param>
  /// <returns> saved section form data.</returns>
  [Authorize(nameof(AuthPolicies.CanCreateExperiments))]
  [HttpPut("save-form")]
  [Consumes("multipart/form-data")]
  public async Task<ActionResult<SectionFormModel>> SaveSectionForm([FromForm] SectionFormPayloadModel model)
  {
    try
    {
      var userId = _users.GetUserId(User);
      var isAuthorised = userId is not null && await _projectGroups.IsProjectGroupMember(userId, model.RecordId);

      if (!isAuthorised) return Forbid();

      return await _projectGroups.SaveForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
