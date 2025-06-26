namespace AI4Green4Students.Services;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Auth;
using Constants;
using Data;
using Data.Entities.Identity;
using Data.Entities.SectionTypeData;
using EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Emails;
using Models.ProjectGroup;
using Models.Section;

public class ProjectGroupService
{
  private readonly AccountEmailService _accountEmail;
  private readonly ApplicationDbContext _db;
  private readonly FieldResponseService _fieldResponses;
  private readonly ProjectGroupEmailService _projectGroupEmail;
  private readonly SectionFormService _sectionForm;
  private readonly TokenIssuingService _tokens;
  private readonly UserManager<ApplicationUser> _users;

  public ProjectGroupService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> users,
    TokenIssuingService tokens,
    AccountEmailService accountEmail,
    ProjectGroupEmailService projectGroupEmail,
    SectionFormService sectionForm,
    FieldResponseService fieldResponses)
  {
    _db = db;
    _users = users;
    _tokens = tokens;
    _accountEmail = accountEmail;
    _projectGroupEmail = projectGroupEmail;
    _sectionForm = sectionForm;
    _fieldResponses = fieldResponses;
  }

  /// <summary>
  /// List instructor's project groups.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Instructor id.</param>
  /// <returns>Project groups list.</returns>
  public async Task<List<ProjectGroupModel>> ListByInstructor(int id, string userId)
  {
    var list = await QueryWithStudents().AsNoTracking()
      .Where(x => x.Project.Id == id && x.Project.Instructors.Any(y => y.Id == userId))
      .ToListAsync();

    return list.Select(x => new ProjectGroupModel(x)).ToList();
  }

  /// <summary>
  /// List student's project groups.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Student id.</param>
  /// <returns>Project groups list.</returns>
  public async Task<List<ProjectGroupModel>> ListByStudent(int id, string userId)
  {
    var list = await QueryWithStudents().AsNoTracking()
      .Where(x => x.Students.Any(y => y.Id == userId) && x.Project.Id == id).ToListAsync();

    return list.Select(x => new ProjectGroupModel(x)).ToList();
  }

  /// <summary>
  /// Get project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <returns>Project group.</returns>
  public async Task<ProjectGroupModel> Get(int id)
  {
    var result = await QueryWithStudents().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new ProjectGroupModel(result);
  }

  /// <summary>
  /// Delete project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  public async Task Delete(int id)
  {
    var entity = await Query().AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    _db.ProjectGroups.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Create project group.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Newly created project group.</returns>
  public async Task<ProjectGroupModel> Create(CreateProjectGroupModel model)
  {
    var existingProject = await _db.Projects
                            .Where(x => x.Id == model.ProjectId)
                            .Include(x => x.ProjectGroups)
                            .FirstOrDefaultAsync()
                          ?? throw new KeyNotFoundException();

    var existingProjectGroup = existingProject.ProjectGroups
      .FirstOrDefault(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));

    if (existingProjectGroup is not null)
    {
      return await Set(existingProjectGroup.Id, model);
    }

    var entity = new ProjectGroup
    {
      Name = model.Name,
      Project = existingProject,
      StartDate = ParseDateOrDefault(model.StartDate),
      PlanningDeadline = ParseDateOrDefault(model.PlanningDeadline),
      ExperimentDeadline = ParseDateOrDefault(model.ExperimentDeadline)
    };

    await _db.ProjectGroups.AddAsync(entity);

    entity.FieldResponses = await _fieldResponses.CreateResponses<ProjectGroup>(entity.Id, existingProject.Id);

    await _db.SaveChangesAsync();
    return await Get(entity.Id);
  }

  /// <summary>
  /// Update project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Updated project group model.</returns>
  public async Task<ProjectGroupModel> Set(int id, CreateProjectGroupModel model)
  {
    var entity = await Query().Where(x => x.Id == id).SingleOrDefaultAsync() ?? throw new KeyNotFoundException();

    entity.Name = model.Name;
    entity.StartDate = ParseDateOrDefault(model.StartDate);
    entity.PlanningDeadline = ParseDateOrDefault(model.PlanningDeadline);
    entity.ExperimentDeadline = ParseDateOrDefault(model.ExperimentDeadline);

    _db.ProjectGroups.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Bulk invite students to a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Invite model.</param>
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
        .ThenInclude(y => y.Project)
        .Where(x => x.Email == email)
        .FirstOrDefaultAsync();

      if (existingStudent is null)
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
          await _users.AddToRoleAsync(newUser, Roles.Student);
          await _accountEmail.SendUserInvite(
            new EmailAddress(newUser.Email)
            {
              Name = newUser.FullName
            },
            await _tokens.GenerateAccountActivationLink(newUser));

          inviteResult = await AssignProjectGroup(true, newUser, model.ProjectId, id);
        }
      }
      else
      {
        inviteResult = await AssignProjectGroup(false, existingStudent, model.ProjectId, id);
      }

      if (!string.IsNullOrEmpty(inviteResult.Warning))
      {
        warnings.Add(inviteResult.Warning);
      }

      if (!string.IsNullOrEmpty(inviteResult.Error))
      {
        errors.Add(inviteResult.Error);
      }
    }

    return new BulkInviteStudentResult
    {
      ProjectGroup = await Get(id),
      Warnings = warnings,
      Errors = errors
    };
  }

  /// <summary>
  /// Assign a student to a project group.
  /// </summary>
  /// <param name="isNewStudent">Is the student new?</param>
  /// <param name="student">Student to assign.</param>
  /// <param name="projectId">Project id.</param>
  /// <param name="projectGroupId">Project Group id.</param>
  /// <returns>Invite result.</returns>
  public async Task<InviteStudentResult> AssignProjectGroup(
    bool isNewStudent,
    ApplicationUser student,
    int projectId,
    int projectGroupId
  )
  {
    var warning = string.Empty;
    var error = string.Empty;

    if (!isNewStudent)
    {
      var isStudentInProject = student.ProjectGroups.Any(x => x.Project.Id == projectId);
      var isStudentInProjectGroup = student.ProjectGroups.Any(x => x.Id == projectGroupId);

      if (isStudentInProjectGroup)
      {
        return new InviteStudentResult
        {
          Warning = $"User {student.Email} is already in the project group"
        };
      }

      if (isStudentInProject && !isStudentInProjectGroup)
      {
        var studentExistingProjectGroup = student.ProjectGroups.FirstOrDefault(x => x.Project.Id == projectId);
        studentExistingProjectGroup?.Students.Remove(student);
        warning = $"User {student.Email} removed from their current project group {studentExistingProjectGroup?.Name}";
      }
    }

    try
    {
      var entity = await QueryWithStudents().Where(x => x.Id == projectGroupId).SingleOrDefaultAsync()
                   ?? throw new KeyNotFoundException();

      entity.Students.Add(student); // add student to ProjectGroup

      _db.ProjectGroups.Update(entity);
      await _db.SaveChangesAsync();

      // notify student of project group assignment
      await _projectGroupEmail.SendProjectGroupAssignmentUpdate(new EmailAddress(student.Email)
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

  /// <summary>
  /// Remove a student from a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="model">Remove model.</param>
  /// <returns>Updated project group.</returns>
  public async Task<ProjectGroupModel> RemoveStudent(int id, RemoveStudentModel model)
  {
    var entity = await QueryWithStudents().Where(x => x.Id == id).SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();
    var student = await _users.FindByIdAsync(model.StudentId) ?? throw new KeyNotFoundException();
    entity.Students.Remove(student);
    await _db.SaveChangesAsync();

    await _projectGroupEmail.SendProjectGroupRemovalUpdate(new EmailAddress(student.Email)
    {
      Name = student.FullName
    }, entity.Project.Name, entity.Name);
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
      .AnyAsync(x => x.Id == projectGroupId && x.Students.Any(y => y.Id == userId));

  /// <summary>
  /// Check if a given user is the instructor of a given project group's project.
  /// </summary>
  /// <param name="userId">Instructor id to check.</param>
  /// <param name="projectGroupId">Project group id.</param>
  /// <returns>True if the user is the instructor, false otherwise.</returns>
  public async Task<bool> IsPgProjectInstructor(string userId, int projectGroupId)
    => await _db.ProjectGroups
      .AsNoTracking()
      .AnyAsync(x => x.Id == projectGroupId && x.Project.Instructors.Any(y => y.Id == userId));

  /// <summary>
  /// Get a project group section including its fields and field responses.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <returns>Section form.</returns>
  public async Task<SectionFormModel> GetSectionForm(int id)
  {
    var pg = await Get(id);
    var pgSection = await _db.Sections.AsNoTracking()
                      .Where(x => x.SectionType.Name == SectionTypes.ProjectGroup && x.Project.Id == pg.ProjectId)
                      .FirstAsync()
                    ?? throw new KeyNotFoundException();
    return await _sectionForm.GetSectionForm<ProjectGroup>(id, pgSection.Id);
  }

  /// <summary>
  /// Save project group section form.
  /// </summary>
  /// <param name="model">Section form payload.</param>
  /// <returns>Saved data.</returns>
  public async Task<SectionFormModel> SaveForm(SectionFormPayloadModel model)
  {
    var submission = new SectionFormSubmissionModel
    {
      SectionId = model.SectionId,
      RecordId = model.RecordId,
      FieldResponses = await _fieldResponses.CreateFieldResponseModels(
        model.FieldResponses,
        model.Files,
        model.FileFieldResponses
      ),
      NewFieldResponses = await _fieldResponses.CreateFieldResponseModels(
        model.NewFieldResponses,
        model.NewFiles,
        model.NewFileFieldResponses,
        true
      )
    };

    var pg = await Get(model.RecordId);
    var fieldResponses = await _fieldResponses.ListBySection<ProjectGroup>(submission.RecordId, submission.SectionId);
    var updatedValues = _fieldResponses.UpdateDraft(submission.FieldResponses, fieldResponses);

    foreach (var updatedValue in updatedValues)
    {
      _db.Update(updatedValue);
    }

    await _db.SaveChangesAsync();

    if (submission.NewFieldResponses.Count == 0)
    {
      return await _sectionForm.GetSectionForm<ProjectGroup>(submission.RecordId, submission.SectionId);
    }

    var entity = await _db.ProjectGroups.FindAsync(submission.RecordId) ?? throw new KeyNotFoundException();
    var newFieldResponses = await _fieldResponses.CreateResponses<ProjectGroup>(
      pg.Id,
      pg.ProjectId,
      submission.NewFieldResponses
    );

    entity.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await _sectionForm.GetSectionForm<ProjectGroup>(submission.RecordId, submission.SectionId);
  }

  private IQueryable<ProjectGroup> QueryWithStudents()
    => _db.ProjectGroups.AsQueryable().Include(x => x.Project).Include(x => x.Students);

  private IQueryable<ProjectGroup> Query()
    => _db.ProjectGroups.AsQueryable().Include(x => x.Project);

  /// <summary>
  /// Parse date string to DateTimeOffset.
  /// </summary>
  /// <param name="dateString">Date string to parse.</param>
  /// <returns>DateTimeOffset.</returns>
  private static DateTimeOffset ParseDateOrDefault(string dateString)
    => DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
      ? new DateTimeOffset(date, TimeSpan.Zero)
      : DateTimeOffset.MaxValue;
}
