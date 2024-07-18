using System.ComponentModel.DataAnnotations;
using AI4Green4Students.Auth;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Models.ProjectGroup;
using AI4Green4Students.Models.Section;
using AI4Green4Students.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class ProjectGroupService
{
  private readonly ApplicationDbContext _db;
  private readonly UserManager<ApplicationUser> _users;
  private readonly TokenIssuingService _tokens;
  private readonly ProjectGroupEmailService _projectGroupEmail;
  private readonly SectionFormService _sectionForm;

  public ProjectGroupService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> users,
    TokenIssuingService tokens,
    ProjectGroupEmailService projectGroupEmail,
    SectionFormService sectionForm)
  {
    _db = db;
    _users = users;
    _tokens = tokens;
    _projectGroupEmail = projectGroupEmail;
    _sectionForm = sectionForm;
  }

  /// <summary>
  /// List all project's project groups including project group students.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>Project groups list.</returns>
  public async Task<List<ProjectGroupModel>> ListByProject(int id)
  {
    var list = await ProjectGroupsQuery(true).AsNoTracking().Where(x => x.Project.Id == id).ToListAsync();

    return list.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x));
  }
  
  /// <summary>
  /// List user's project groups including project group students based on project id.
  /// </summary>
  /// <param name="userId">User id.</param>
  /// <param name="id">Project id.</param>
  /// <returns>Users project groups list.</returns>
  public async Task<List<ProjectGroupModel>> ListByUser(int id, string userId)
  {
    var list = await ProjectGroupsQuery(true).AsNoTracking()
      .Where(x => x.Students.Any(y => y.Id == userId) && x.Project.Id == id).ToListAsync();

    return list.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x));
  }
  
  /// <summary>
  /// Get project group by project group id.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <returns>Project group.</returns>
  public async Task<ProjectGroupModel> Get(int id)
  {
    var result = await ProjectGroupsQuery(true).AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ProjectGroupModel(result);
  }

  /// <summary>
  /// Delete project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  public async Task Delete(int id)
  {
    var entity = await ProjectGroupsQuery().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    _db.ProjectGroups.Remove(entity);
    await _db.SaveChangesAsync();
  }
  
  /// <summary>
  /// Create project group based on model, including default field responses for the project group section (activities).
  /// </summary>
  /// <param name="model">Model for creating project group.</param>
  /// <returns>Newly created project group model.</returns>
  public async Task<ProjectGroupModel> Create(CreateProjectGroupModel model)
  {
    var existingProject = await _db.Projects
                    .Where(x=>x.Id == model.ProjectId)
                    .Include(x=>x.ProjectGroups)
                    .FirstOrDefaultAsync()
                  ?? throw new KeyNotFoundException();
    
    var existingProjectGroup = existingProject.ProjectGroups
      .FirstOrDefault(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));
    
    if (existingProjectGroup is not null) 
      await Set(existingProjectGroup.Id, model); // update existing ProjectGroup (for now just the name)
    
    var entity = new ProjectGroup { Name = model.Name, Project = existingProject}; // create new ProjectGroup
    
    await _db.ProjectGroups.AddAsync(entity); // add ProjectGroup to db
    
    // create field responses for the new ProjectGroup
    entity.FieldResponses = await _sectionForm.CreateFieldResponses<ProjectGroup>(entity.Id, existingProject.Id, SectionTypes.ProjectGroup,null); 
    
    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }
  
  /// <summary>
  /// Update project group based on model.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Model for updating project group.</param>
  /// <returns>Updated project group model.</returns>
  public async Task<ProjectGroupModel> Set (int id, CreateProjectGroupModel model)
  {
    var entity = await ProjectGroupsQuery().Where(x=>x.Id == id).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    entity.Name = model.Name;
    
    _db.ProjectGroups.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
  
  /// <summary>
  /// Bulk Invite students to a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Model for inviting students.</param>
  /// <param name="uiCulture">User interface culture.</param>
  /// <returns>Bulk invite student result.</returns>
  public async Task<BulkInviteStudentResult> InviteStudents(int id, InviteStudentModel model, string uiCulture)
  {
    var errors = new List<string>();
    var warnings = new List<string>();
    
    foreach (var email in model.Emails)
    {
      var inviteResult = new InviteStudentResult();
      var isEmailValid = new EmailAddressAttribute().IsValid(email);
      
      if (!isEmailValid)
      {
        errors.Add($"Email {email} is not valid");
        continue; // skip to next email
      }

      var existingStudent = await _users.Users
        .Include(x => x.ProjectGroups)
        .ThenInclude(y=>y.Project)
        .Where(x => x.Email == email)
        .FirstOrDefaultAsync();
      
      if (existingStudent is null)
      {
        var newUser = new ApplicationUser { UserName = email, Email = email, UICulture = uiCulture };
        var result = await _users.CreateAsync(newUser);
        if (result.Succeeded)
        {
          await _users.AddToRoleAsync(newUser, Roles.Student); // assign student role to the user
          await _tokens.SendUserInvite(newUser);
          inviteResult = await AssignProjectGroup(true, newUser, model.ProjectId, id);
        }
      }
      else inviteResult = await AssignProjectGroup(false, existingStudent, model.ProjectId, id);

      if (!string.IsNullOrEmpty(inviteResult.Warning)) warnings.Add(inviteResult.Warning);
      if (!string.IsNullOrEmpty(inviteResult.Error)) errors.Add(inviteResult.Error);
    }

    return new BulkInviteStudentResult { ProjectGroup = await Get(id), Warnings = warnings, Errors = errors };
  }

  /// <summary>
  /// Assign a student to a project group.
  /// </summary>
  /// <param name="isNewStudent">Is the student new?</param>
  /// <param name="student">Student to assign.</param>
  /// <param name="projectId">Project id.</param>
  /// <param name="projectGroupId">Project Group id.</param>
  /// <returns>Invite student result.</returns>
  public async Task<InviteStudentResult> AssignProjectGroup (bool isNewStudent, ApplicationUser student, int projectId, int projectGroupId)
  {
    var warning = string.Empty;
    var error = string.Empty;
    
    if (!isNewStudent)
    {
      var isStudentInProject = student.ProjectGroups.Any(x => x.Project.Id == projectId); // is student in the proposed Project?
      var isStudentInProjectGroup = student.ProjectGroups.Any(x => x.Id == projectGroupId); // is student in the proposes Project Group?
    
      if (isStudentInProjectGroup)
        return new InviteStudentResult { Warning = $"User {student.Email} is already in the project group" };

      if (isStudentInProject && !isStudentInProjectGroup)
      {
        // remove the student from their current ProjectGroup matching the Project
        var studentExistingProjectGroup = student.ProjectGroups.FirstOrDefault(x => x.Project.Id == projectId);
        studentExistingProjectGroup?.Students.Remove(student);
        warning = $"User {student.Email} removed from their current project group {studentExistingProjectGroup?.Name}";
      }
    }

    try
    {
      var entity = await ProjectGroupsQuery(true).Where(x => x.Id == projectGroupId).SingleOrDefaultAsync()
                   ?? throw new KeyNotFoundException();
      entity.Students.Add(student); // add student to ProjectGroup
      
      _db.ProjectGroups.Update(entity);
      await _db.SaveChangesAsync();
      
      // notify student of project group assignment
      await _projectGroupEmail.SendProjectGroupAssignmentUpdate(new EmailAddress(student.Email) { Name = student.FullName }, entity.Project.Name, entity.Name);
    }
    catch (KeyNotFoundException)
    {
      error = "Project group does not exist";
    }

    return new InviteStudentResult { Warning = warning, Error = error };
  }
  
  /// <summary>
  /// Remove a student from a project group and notify the student.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Model for removing student.</param>
  /// <returns>Updated project group.</returns>
  public async Task<ProjectGroupModel> RemoveStudent(int id, RemoveStudentModel model)
  {
    var entity = await ProjectGroupsQuery(true).Where(x => x.Id == id).SingleOrDefaultAsync() 
                 ?? throw new KeyNotFoundException();
    var student = await _users.FindByIdAsync(model.StudentId) ?? throw new KeyNotFoundException();
    entity.Students.Remove(student);
    await _db.SaveChangesAsync();
    
    await _projectGroupEmail.SendProjectGroupRemovalUpdate(new EmailAddress(student.Email) { Name = student.FullName }, entity.Project.Name, entity.Name);
    return await Get(id);
  }

  /// <summary>
  /// Check if a given user is the member of a given project group.
  /// </summary>
  /// <param name="userId">Id of the user to check.</param>
  /// <param name="projectGroupId">Id of the project group to check the user against.</param>
  /// <returns>True if the user is the member of the project group, false otherwise.</returns>
  public async Task<bool> IsProjectGroupMember(string userId, int projectGroupId)
    => await _db.ProjectGroups
      .AsNoTracking()
      .AnyAsync(x => x.Id == projectGroupId && x.Students.Any(y=>y.Id == userId));

  /// <summary>
  /// Get a project group section including its fields and field responses.
  /// </summary>
  /// <param name="projectGroupId">Id of the project group to get the field responses for</param>
  /// <returns>Project group section with its fields, fields response and more.</returns>
  public async Task<SectionFormModel> GetSectionForm(int projectGroupId)
  {
    var pg = await Get(projectGroupId);
    var pgSection = await _db.Sections
                      .AsNoTracking()
                      .Where(x => x.SectionType.Name == SectionTypes.ProjectGroup && x.Project.Id == pg.ProjectId)
                      .FirstAsync()
                    ?? throw new KeyNotFoundException(); // since project group only has one section
    
    var fieldsResponses = await _sectionForm.ListBySection<ProjectGroup>(projectGroupId, pgSection.Id);
    return await _sectionForm.GetFormModel(pgSection.Id, fieldsResponses);
  }
  
  /// <summary>
  /// Save project group section form. Also creates new field responses if they don't exist.
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public async Task<SectionFormModel> SaveForm(SectionFormPayloadModel model)
  {
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _sectionForm.GenerateFieldResponses(model.FieldResponses, model.Files, model.FileFieldResponses),
      NewFieldResponses = await _sectionForm.GenerateFieldResponses(model.NewFieldResponses, model.NewFiles, model.NewFileFieldResponses)
    };
    
    var pg = await Get(model.RecordId);
    
    var fieldResponses = await _sectionForm.ListBySection<ProjectGroup>(submission.RecordId, submission.SectionId);

    var updatedValues = _sectionForm.UpdateDraftFieldResponses(submission.FieldResponses, fieldResponses);
    
    foreach (var updatedValue in updatedValues) _db.Update(updatedValue);
    await _db.SaveChangesAsync();
    
    if (submission.NewFieldResponses.Count == 0) return await GetSectionForm(submission.RecordId);
    
    var entity = await _db.ProjectGroups.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    var newFieldResponses = await _sectionForm.CreateFieldResponses<ProjectGroup>(pg.Id, pg.ProjectId, SectionTypes.ProjectGroup, submission.NewFieldResponses);
    entity.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await GetSectionForm(model.RecordId);
  }
  
  /// <summary>
  /// Construct a query to fetch Project groups along with its related entities.
  /// </summary>
  /// <returns>An IQueryable of Project Group entities.</returns>
  private IQueryable<ProjectGroup> ProjectGroupsQuery(bool includeStudents = false)
  {
    var query = _db.ProjectGroups.AsQueryable();
    query = query.Include(x => x.Project);

    return includeStudents ? query.Include(x => x.Students) : query;
  }
}
