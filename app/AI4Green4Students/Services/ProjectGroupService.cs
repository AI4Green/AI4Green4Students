using System.ComponentModel.DataAnnotations;
using AI4Green4Students.Auth;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.Identity;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Models.ProjectGroup;
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

  public ProjectGroupService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> users,
    TokenIssuingService tokens,
    ProjectGroupEmailService projectGroupEmail)
  {
    _db = db;
    _users = users;
    _tokens = tokens;
    _projectGroupEmail = projectGroupEmail;
  }

  public async Task<List<ProjectGroupModel>> List()
  {
    var list = await _db.ProjectGroups
      .AsNoTracking()
      .Include(x => x.Project)
      .Include(y=>y.Students)
      .ToListAsync();

    return list.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x));
  }
  
  public async Task<List<ProjectGroupModel>> ListEligible(string userId)
  {
    var list = await _db.ProjectGroups
      .AsNoTracking()
      .Where(x=> x.Students.Any(y=>y.Id==userId))
      .Include(x => x.Project)
      .ToListAsync();

    return list.ConvertAll<ProjectGroupModel>(x => new ProjectGroupModel(x));
  }
  
  public async Task<ProjectGroupModel> Get(int id)
  {
    var result = await _db.ProjectGroups
                   .AsNoTracking()
                   .Where(x => x.Id == id)
                   .Include(x => x.Project)
                   .Include(x=>x.Students)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ProjectGroupModel(result);
  }
  
  public async Task<ProjectGroupModel> GetEligible(int id, string userId)
  {
    var result = await _db.ProjectGroups
                   .AsNoTracking()
                   .Where(x => x.Id == id && 
                               x.Students.Any(y=>y.Id==userId))
                   .Include(x => x.Project)
                   .Include(x=>x.Students)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ProjectGroupModel(result);
  }

  public async Task Delete(int id)
  {
    var entity = await _db.ProjectGroups
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x=>x.Id == id)
                 ?? throw new KeyNotFoundException();
    
    _db.ProjectGroups.Remove(entity);
    await _db.SaveChangesAsync();
  }
  
  public async Task<ProjectGroupModel> Create(CreateProjectGroupModel model)
  {
    var existingProject = await _db.Projects
                    .Where(x=>x.Id == model.ProjectId)
                    .FirstOrDefaultAsync()
                  ?? throw new KeyNotFoundException();
    
    var existingProjectGroup = existingProject.ProjectGroups
      .FirstOrDefault(y => EF.Functions.ILike(y.Name, model.Name));

    if (existingProjectGroup is not null) 
      throw new InvalidOperationException("Project group name already exist");

    var newProjectGroup = new ProjectGroup { Name = model.Name, Project = existingProject}; // create new ProjectGroup
    
    await _db.ProjectGroups.AddAsync(newProjectGroup); // add ProjectGroup to db
    await _db.SaveChangesAsync();
    
    return await Get(newProjectGroup.Id);
  }
  
  public async Task<ProjectGroupModel> Set (int id, CreateProjectGroupModel model)
  {
    var entity = await _db.ProjectGroups
                   .Where(x=>x.Id == id && x.Project.Id == model.ProjectId)
                   .Include(y => y.Project)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    
    entity.Name = model.Name;
    
    _db.ProjectGroups.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }
  
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

      var studentEmail = await _users.Users
        .Include(x => x.ProjectGroups)
        .ThenInclude(y=>y.Project)
        .Where(x => x.Email == email)
        .FirstOrDefaultAsync();
      
      if (studentEmail is null)
      {
        var newUser = new ApplicationUser
        {
          UserName = email,
          Email = email,
          UICulture = uiCulture
        };
        var result = await _users.CreateAsync(newUser);
        if (result.Succeeded)
        {
          await _users.AddToRoleAsync(newUser, Roles.Student); // assign student role to the user
          await _tokens.SendUserInvite(newUser);
          inviteResult = await AssignProjectGroup(true, newUser, model.ProjectId, id);
        }
      }
      else inviteResult = await AssignProjectGroup(false, studentEmail, model.ProjectId, id);

      if (!string.IsNullOrEmpty(inviteResult.Warning)) warnings.Add(inviteResult.Warning);
      if (!string.IsNullOrEmpty(inviteResult.Error)) errors.Add(inviteResult.Error);
    }
    return new BulkInviteStudentResult
    {
      ProjectGroup = await Get(id),
      Warnings = warnings,
      Errors = errors
    };
  }

  public async Task<ProjectGroup> GetTrackedEntity (int projectGroupId)
  {
    return await _db.ProjectGroups
      .Where(x => x.Id == projectGroupId)
      .Include(y => y.Project)
      .Include(z=>z.Students)
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException();
  }
  
  public async Task<InviteStudentResult> AssignProjectGroup (bool isNewStudent, ApplicationUser student, int projectId, int projectGroupId)
  {
    var warning = string.Empty;
    var error = string.Empty;
    
    if (!isNewStudent)
    {
      var isStudentInProject = student.ProjectGroups.Any(x=>x.Project.Id == projectId); // is student in the proposed Project?
      var isStudentInProjectGroup = student.ProjectGroups.Any(x=>x.Id == projectGroupId); // is student in the propose ProjectGroup?
    
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
      var entity = await GetTrackedEntity(projectGroupId);
      entity.Students.Add(student); // add student to ProjectGroup
      
      _db.ProjectGroups.Update(entity);
      await _db.SaveChangesAsync();
      
      // notify student of project group assignment
      await _projectGroupEmail.SendProjectGroupAssignmentUpdate(
        new EmailAddress(student.Email)
        {
          Name = student.FullName
        }, entity.Project.Name, entity.Name);
    }
    catch (KeyNotFoundException)
    {
      error = "Project group does not exist";
    }

    return new InviteStudentResult
    {
      Warning = warning,
      Error = error
    };
  }
  
  public async Task<ProjectGroupModel> RemoveStudent(int id, RemoveStudentModel model)
  {
    var entity = await GetTrackedEntity(id);
    var student = await _users.FindByIdAsync(model.StudentId) 
                  ?? throw new KeyNotFoundException();
    entity.Students.Remove(student);
    await _db.SaveChangesAsync();
    
    await _projectGroupEmail.SendProjectGroupRemovalUpdate(
      new EmailAddress(student.Email)
      {
        Name = student.FullName
      }, entity.Project.Name, entity.Name);
    return await Get(id);
  }
}
