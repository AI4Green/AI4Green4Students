namespace AI4Green4Students.Controllers;

using Auth;
using Data.Entities.Identity;
using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.ProjectGroup;
using Models.Section;
using Services;

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
  /// List project groups based on user's role.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>Project group list</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet("project/{id}")]
  public async Task<ActionResult<List<ProjectGroupModel>>> List(int id)
  {
    var userId = _users.GetUserId(User);
    if (userId is null)
    {
      return Forbid();
    }

    var isInstructor = User.IsInRole(Roles.Instructor);
    return isInstructor
      ? await _projectGroups.ListByInstructor(id, userId)
      : await _projectGroups.ListByStudent(id, userId);
  }


  /// <summary>
  /// Delete project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
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
  /// Create a project group.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Project group.</returns>
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
  /// Update a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Project group.</returns>
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
  /// Bulk invite students to a project group.
  /// </summary>
  /// <param name="id">Project Group id</param>
  /// <param name="model">Invite model.</param>
  /// <returns>Invite results.</returns>
  [Authorize(nameof(AuthPolicies.CanInviteUsers))]
  [Authorize(nameof(AuthPolicies.CanInviteStudents))]
  [HttpPut("{id}/invite-students")]
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
  /// Remove a student from a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Remove model.</param>
  /// <returns>Result.</returns>
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
  /// Get a project group by id.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <returns>Project group.</returns>
  [Authorize(nameof(AuthPolicies.CanViewOwnProjects))]
  [HttpGet("{id}")]
  public async Task<ActionResult<ProjectGroupModel>> Get(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _projectGroups.IsPgProjectInstructor(userId, id) ||
                         await _projectGroups.IsProjectGroupMember(userId, id);

      return isAuthorised ? await _projectGroups.Get(id) : Forbid();
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }

  /// <summary>
  /// Get a project group activities section form.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <returns>Project group activities section form.</returns>
  [HttpGet("{id}/form")]
  public async Task<ActionResult<SectionFormModel>> GetSectionForm(int id)
  {
    try
    {
      var userId = _users.GetUserId(User);
      if (userId is null)
      {
        return Forbid();
      }

      var isAuthorised = await _projectGroups.IsPgProjectInstructor(userId, id) ||
                         await _projectGroups.IsProjectGroupMember(userId, id);

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

      if (!isAuthorised)
      {
        return Forbid();
      }

      return await _projectGroups.SaveForm(model);
    }
    catch (KeyNotFoundException)
    {
      return NotFound();
    }
  }
}
