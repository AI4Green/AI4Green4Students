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
using Models.Section.Form;
using ProjectGroupModel=Models.ProjectGroup.ProjectGroupModel;

public class ProjectGroupService
{
  private readonly AccountEmailService _accountEmail;
  private readonly ApplicationDbContext _db;
  private readonly FieldResponseService _fieldResponses;
  private readonly ProjectGroupEmailService _projectGroupEmail;
  private readonly SectionFormService _sectionForm;
  private readonly TokenIssuingService _tokens;
  private readonly UserManager<ApplicationUser> _users;
  private readonly UserService _user;

  public ProjectGroupService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> users,
    TokenIssuingService tokens,
    AccountEmailService accountEmail,
    ProjectGroupEmailService projectGroupEmail,
    SectionFormService sectionForm,
    FieldResponseService fieldResponses,
    UserService user
  )
  {
    _db = db;
    _users = users;
    _tokens = tokens;
    _accountEmail = accountEmail;
    _projectGroupEmail = projectGroupEmail;
    _sectionForm = sectionForm;
    _fieldResponses = fieldResponses;
    _user = user;
  }

  /// <summary>
  /// List instructor's project groups.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Instructor id.</param>
  /// <returns>Project groups list.</returns>
  public async Task<List<ProjectGroupModel>> ListByInstructor(int id, string userId)
    => await _db.ProjectGroups.AsNoTracking()
      .Include(x => x.Project)
      .Include(x => x.Students)
      .Where(x => x.Project.Id == id && x.Project.Instructors.Any(y => y.Id == userId))
      .Select(x => new ProjectGroupModel(x))
      .ToListAsync();

  /// <summary>
  /// List student's project groups.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Student id.</param>
  /// <returns>Project groups list.</returns>
  public async Task<List<ProjectGroupModel>> ListByStudent(int id, string userId)
    => await _db.ProjectGroups.AsNoTracking()
      .Include(x => x.Project)
      .Include(x => x.Students)
      .Where(x => x.Students.Any(y => y.Id == userId) && x.Project.Id == id)
      .Select(x => new ProjectGroupModel(x))
      .ToListAsync();

  /// <summary>
  /// Get project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <returns>Project group.</returns>
  public async Task<ProjectGroupModel> Get(int id)
  {
    var entity = await _db.ProjectGroups.AsNoTracking()
                   .Include(x => x.Project)
                   .Include(x => x.Students)
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    return new ProjectGroupModel(entity);
  }

  /// <summary>
  /// Delete project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  public async Task Delete(int id)
  {
    var entity = await _db.ProjectGroups.FindAsync(id) ?? throw new KeyNotFoundException();

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
                            .Include(x => x.ProjectGroups)
                            .SingleOrDefaultAsync(x => x.Id == model.ProjectId)
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
    var entity = await _db.ProjectGroups
                   .Include(x => x.Project)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

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
  /// <param name="emails">Invite emails.</param>
  /// <param name="uiCulture">User interface culture.</param>
  public async Task InviteStudents(int id, List<string> emails, string uiCulture)
  {
    var normalizedEmails = emails.Select(x => x.ToUpperInvariant()).ToList();
    var existingUsers = await _users.Users.AsNoTracking()
      .Where(x => normalizedEmails.Contains(x.NormalizedEmail!))
      .ToListAsync();

    var students = new List<ApplicationUser>();
    foreach (var email in emails)
    {
      var isEmailValid = new EmailAddressAttribute().IsValid(email);

      if (!isEmailValid)
      {
        continue; // skip to next email
      }

      var user = existingUsers.FirstOrDefault(x => x.Email!.Equals(email, StringComparison.OrdinalIgnoreCase));
      if (user is not null)
      {
        var isStudent = await _users.IsInRoleAsync(user, Roles.Student);

        if (!await _user.CanRegister(email))
        {
          continue;
        }

        if (isStudent)
        {
          students.Add(user);
        }

        continue;
      }

      if (!await _user.CanRegister(email))
      {
        continue;
      }

      var newUser = new ApplicationUser
      {
        UserName = email, Email = email, UICulture = uiCulture
      };

      var result = await _users.CreateAsync(newUser);
      if (result.Succeeded)
      {
        await _users.AddToRoleAsync(newUser, Roles.Student);
        students.Add(newUser);
      }
    }

    // send invites to unconfirmed students
    foreach (var student in students.Where(x => !x.EmailConfirmed))
    {
      await _accountEmail.SendUserInvite(
        new EmailAddress(student.Email!),
        await _tokens.GenerateAccountActivationLink(student)
      );
    }

    await AssignProjectGroup(id, students.Select(x => x.Email!).ToList());
  }

  /// <summary>
  /// Remove a student from a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="studentId">Student id.</param>
  public async Task RemoveStudent(int id, string studentId)
  {
    var entity = await _db.ProjectGroups
                   .Include(x => x.Project)
                   .Include(x => x.Students)
                   .FirstOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    var student = await _users.FindByIdAsync(studentId) ?? throw new KeyNotFoundException();

    entity.Students.Remove(student);
    await _db.SaveChangesAsync();

    var emailModel = new ProjectGroupEmailModel(
      new EmailAddress(student.Email!)
      {
        Name = student.FullName
      },
      entity.Project.Name,
      entity.Name
    );

    await _projectGroupEmail.RemoveProjectGroup(emailModel);
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
    var pg = await _db.ProjectGroups.AsNoTracking()
      .Where(x => x.Id == id)
      .Include(x => x.Project)
      .ThenInclude(x => x.ProjectType)
      .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Project Group not found.");

    var pgSection = await _db.Sections.AsNoTracking()
                      .Where(x =>
                        x.SectionType.Name == SectionTypes.ProjectGroup &&
                        x.ProjectType.Id == pg.Project.ProjectType.Id)
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
      pg.Project.Id,
      submission.NewFieldResponses
    );

    entity.FieldResponses.AddRange(newFieldResponses);
    await _db.SaveChangesAsync();

    return await _sectionForm.GetSectionForm<ProjectGroup>(submission.RecordId, submission.SectionId);
  }

  /// <summary>
  /// Assign students to a project group.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="emails">Student email list.</param>
  private async Task AssignProjectGroup(int id, List<string> emails)
  {
    var projectGroup = await _db.ProjectGroups
                         .Include(x => x.Project)
                         .Include(x => x.Students)
                         .FirstOrDefaultAsync(x => x.Id == id)
                       ?? throw new KeyNotFoundException();

    var normalizedEmails = emails.Select(x => x.ToUpperInvariant()).ToList();
    var students = await _users.Users
      .Include(x => x.ProjectGroups)
      .ThenInclude(y => y.Project)
      .Where(x => normalizedEmails.Contains(x.NormalizedEmail!))
      .ToListAsync();

    var studentsToBeAssigned = students.Where(x => x.ProjectGroups.All(y => y.Id != id)).ToList();
    var studentsToClearProjectGroups = studentsToBeAssigned
      .Where(x => x.ProjectGroups.Any(y => y.Project.Id == projectGroup.Project.Id && y.Id != id))
      .Select(x => x.Id)
      .ToList();

    await RemoveProjectGroups(studentsToClearProjectGroups);

    var emailModel = new List<ProjectGroupEmailModel>();

    foreach (var student in studentsToBeAssigned)
    {
      projectGroup.Students.Add(student);

      emailModel.Add(new ProjectGroupEmailModel(
        new EmailAddress(student.Email!)
        {
          Name = student.FullName
        },
        projectGroup.Project.Name,
        projectGroup.Name
      ));
    }

    await _db.SaveChangesAsync();

    foreach (var model in emailModel)
    {
      await _projectGroupEmail.AssignProjectGroup(model);
    }
  }

  /// <summary>
  /// Remove any project group assigned to the students.
  /// </summary>
  /// <param name="ids">Student ids.</param>
  private async Task RemoveProjectGroups(List<string> ids)
  {
    var students = await _users.Users
      .Include(x => x.ProjectGroups)
      .Where(x => ids.Contains(x.Id))
      .ToListAsync();

    var emailModel = new List<ProjectGroupEmailModel>();
    foreach (var student in students)
    {
      var projectGroups = student.ProjectGroups.ToList();
      foreach (var projectGroup in projectGroups)
      {
        student.ProjectGroups.Remove(projectGroup);

        emailModel.Add(new ProjectGroupEmailModel(
          new EmailAddress(student.Email!)
          {
            Name = student.FullName
          },
          projectGroup.Project.Name,
          projectGroup.Name
        ));
      }
    }

    await _db.SaveChangesAsync();

    foreach (var model in emailModel)
    {
      await _projectGroupEmail.RemoveProjectGroup(model);
    }
  }

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
