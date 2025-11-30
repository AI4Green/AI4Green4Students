namespace AI4Green4Students.Services;

using System.ComponentModel.DataAnnotations;
using Auth;
using Constants;
using Data;
using Data.Entities;
using Data.Entities.Identity;
using EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Emails;
using Models.Project;
using Models.User;

public class ProjectService
{
  private readonly ApplicationDbContext _db;
  private readonly LiteratureReviewService _literatureReviews;
  private readonly PlanService _plans;
  private readonly ReportService _reports;
  private readonly UserManager<ApplicationUser> _users;
  private readonly AccountEmailService _accountEmail;
  private readonly ProjectEmailService _projectEmail;
  private readonly TokenIssuingService _tokens;
  private readonly UserService _user;

  public ProjectService(
    ApplicationDbContext db,
    LiteratureReviewService literatureReviews,
    PlanService plans,
    ReportService reports,
    UserManager<ApplicationUser> users,
    AccountEmailService accountEmail,
    ProjectEmailService projectEmail,
    TokenIssuingService tokens,
    UserService user
  )
  {
    _db = db;
    _literatureReviews = literatureReviews;
    _plans = plans;
    _reports = reports;
    _users = users;
    _accountEmail = accountEmail;
    _projectEmail = projectEmail;
    _tokens = tokens;
    _user = user;
  }

  /// <summary>
  /// List projects.
  /// </summary>
  /// <returns>Projects.</returns>
  public async Task<List<ProjectModel>> List()
  {
    var projects = await _db.Projects.AsNoTracking()
      .Include(x => x.ProjectGroups)
      .Include(x => x.ProjectType)
      .ToListAsync();

    var list = new List<ProjectModel>();
    foreach (var project in projects)
    {
      list.Add(new ProjectModel(project)
      {
        Stage = await Status(project.Id)
      });
    }
    return list;
  }

  /// <summary>
  /// List instructor's projects.
  /// </summary>
  /// <param name="userId">Instructor Id.</param>
  /// <returns>Instructor projects.</returns>
  public async Task<List<ProjectModel>> ListByInstructor(string userId)
  {
    var projects = await _db.Projects.AsNoTracking()
      .Include(x => x.ProjectGroups)
      .Include(x => x.ProjectType)
      .Where(x => x.Instructors.Any(y => y.Id == userId))
      .ToListAsync();

    var list = new List<ProjectModel>();
    foreach (var project in projects)
    {
      list.Add(new ProjectModel(project)
      {
        Stage = await Status(project.Id)
      });
    }
    return list;
  }

  /// <summary>
  /// List student's projects
  /// </summary>
  /// <param name="userId">Student Id.</param>
  /// <returns>Student projects.</returns>
  public async Task<List<ProjectModel>> ListByStudent(string userId)
  {
    var projects = await _db.Projects.AsNoTracking()
      .Include(x => x.ProjectGroups).ThenInclude(y => y.Students)
      .Include(x => x.ProjectType)
      .Where(x => x.ProjectGroups.Any(y => y.Students.Any(z => z.Id == userId)))
      .AsSplitQuery()
      .ToListAsync();

    var list = new List<ProjectModel>();
    foreach (var project in projects)
    {
      list.Add(new ProjectModel(project)
      {
        ProjectGroups = project.ProjectGroups
          .Where(x => x.Students.Any(y => y.Id == userId))
          .Select(x => new ProjectGroupModel(x.Id, x.Name)).ToList(),
        Stage = await Status(project.Id, userId)
      });
    }

    return list;
  }

  /// <summary>
  /// Get a project.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <returns>Project.</returns>
  public async Task<ProjectModel> Get(int id)
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectGroups)
                    .Include(x => x.ProjectType)
                    .SingleOrDefaultAsync(x => x.Id == id)
                  ?? throw new KeyNotFoundException();

    return new ProjectModel(project)
    {
      Stage = await Status(project.Id)
    };
  }

  /// <summary>
  /// Get instructor's project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Instructor user id.</param>
  /// <returns>Project.</returns>
  public async Task<ProjectModel> GetByInstructor(int id, string userId)
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectGroups)
                    .Include(x => x.ProjectType)
                    .Where(x => x.Id == id && x.Instructors.Any(y => y.Id == userId))
                    .SingleOrDefaultAsync()
                  ?? throw new KeyNotFoundException();

    return new ProjectModel(project)
    {
      Stage = await Status(project.Id)
    };
  }

  /// <summary>
  /// Get student's project.
  /// </summary>
  /// <param name="id">Project Id.</param>
  /// <param name="userId">Student Id.</param>
  /// <returns>Project.</returns>
  public async Task<ProjectModel> GetByStudent(int id, string userId)
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectGroups
                      .Where(y => y.Students.Any(z => z.Id == userId)))
                    .Include(x => x.ProjectType)
                    .SingleOrDefaultAsync(x => x.Id == id)
                  ?? throw new KeyNotFoundException();

    return new ProjectModel(project)
    {
      Stage = await Status(project.Id)
    };
  }

  /// <summary>
  /// Delete the project.
  /// </summary>
  /// <param name="id">Project Id.</param>
  public async Task Delete(int id)
  {
    var hasRelatedRecords = await _db.Projects
      .Where(x => x.Id == id)
      .Select(x =>
        x.ProjectGroups.Count != 0 ||
        x.Plans.Count != 0 ||
        x.Reports.Count != 0 ||
        _db.LiteratureReviews.Any(y => y.Project.Id == id)
      )
      .FirstOrDefaultAsync();

    if (hasRelatedRecords)
    {
      throw new InvalidOperationException("Cannot delete a project as it has related records.");
    }

    var entity = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();

    _db.Projects.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Create project.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <param name="userId">User Id.</param>
  /// <returns>Project.</returns>
  public async Task Create(CreateProjectModel model, string userId)
  {
    var isExistingValue = await _db.Projects
      .Where(x => EF.Functions.ILike(x.Name, model.Name))
      .FirstOrDefaultAsync();

    if (isExistingValue is not null)
    {
      await Set(isExistingValue.Id, model);
    }

    var projectType = await _db.ProjectTypes.Where(x => x.Id == model.ProjectTypeId).FirstOrDefaultAsync()
                      ?? throw new KeyNotFoundException("Project type not found");

    var instructor = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync()
                     ?? throw new KeyNotFoundException("Instructor not found");

    var entity = new Project
    {
      ProjectType = projectType,
      Name = model.Name,
      Instructors = new List<ApplicationUser>
      {
        instructor
      }
    };

    await _db.Projects.AddAsync(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Update project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Updated Project.</returns>
  public async Task Set(int id, CreateProjectModel model)
  {
    var entity = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();

    entity.Name = model.Name;

    _db.Projects.Update(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Get student's project summary.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="userId">Student id.</param>
  /// <param name="isOwner">Is an entity owner?</param>
  /// <param name="isInstructor">Is instructor?</param>
  /// <returns>Project summary.</returns>
  public async Task<ProjectSummaryModel> GetStudentProjectSummary(
    int id,
    string userId,
    bool isOwner = false,
    bool isInstructor = false
  )
  {
    var project = await _db.Projects.AsNoTracking()
                    .Include(x => x.ProjectGroups
                      .Where(y => y.Students.Any(z => z.Id == userId)))
                    .ThenInclude(x => x.Students)
                    .SingleOrDefaultAsync(x => x.Id == id)
                  ?? throw new KeyNotFoundException();

    var projectGroup = project.ProjectGroups.First();
    var literatureReviews = await _literatureReviews.ListByUser(id, userId);
    var plans = await _plans.ListByUser(id, userId);
    var reports = isOwner || isInstructor ? await _reports.ListByUser(id, userId) : [];

    var owner = projectGroup.Students.First(x => x.Id == userId);

    return new ProjectSummaryModel(
      isInstructor ? literatureReviews.Where(x => x.Stage != Stages.Draft).ToList() : literatureReviews,
      isInstructor ? plans.Where(x => x.Stage != Stages.Draft).ToList() : plans,
      reports,
      new ProjectSummaryProjectModel(project.Id, project.Name),
      new ProjectSummaryProjectGroupModel(projectGroup.Id, projectGroup.Name),
      new ProjectSummaryAuthorModel(owner.Id, owner.FullName)
    );
  }

  /// <summary>
  /// Check if given users belong to the same project group.
  /// </summary>
  /// <param name="viewerId">User id.</param>
  /// <param name="targetUserId">User id.</param>
  /// <param name="projectId">Project id.</param>
  /// <returns>Result.</returns>
  public async Task<bool> IsInSameProjectGroup(string viewerId, string targetUserId, int projectId)
    => await _db.ProjectGroups.AsNoTracking()
      .Where(x => x.Project.Id == projectId && x.Students.Any(y => y.Id == viewerId))
      .AnyAsync(x => x.Students.Any(y => y.Id == targetUserId));

  /// <summary>
  /// Check if a given user is the instructor of a given project.
  /// </summary>
  /// <param name="userId">Instructor id.</param>
  /// <param name="projectId">Project id.</param>
  /// <returns>Result.</returns>
  public async Task<bool> IsProjectInstructor(string userId, int projectId)
    => await _db.Projects.AsNoTracking()
      .AnyAsync(x => x.Id == projectId && x.Instructors.Any(y => y.Id == userId));

  /// <summary>
  /// List project instructors.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <returns>List of instructors.</returns>
  public async Task<List<UserModel>> ListInstructors(int id)
  {
    var project = await _db.Projects.AsNoTracking()
      .Include(x => x.Instructors)
      .FirstOrDefaultAsync(x => x.Id == id)
      ?? throw new KeyNotFoundException();

    var list = new List<UserModel>();
    foreach (var instructor in project.Instructors)
    {
      var roles = await _users.GetRolesAsync(instructor);
      list.Add(new UserModel(
        instructor.Id,
        instructor.Email!,
        instructor.FullName,
        instructor.EmailConfirmed,
        instructor.UICulture,
        roles.ToList()
      ));
    }

    return list;
  }

  /// <summary>
  /// Bulk invite instructors to a project.
  /// </summary>
  /// <param name="id">Project group id.</param>
  /// <param name="emails">Invite emails.</param>
  /// <param name="uiCulture">User interface culture.</param>
  public async Task InviteInstructors(int id, List<string> emails, string uiCulture)
  {
    var normalizedEmails = emails.Select(x => x.ToUpperInvariant()).ToList();
    var existingUsers = await _users.Users.AsNoTracking()
      .Where(x => normalizedEmails.Contains(x.NormalizedEmail!))
      .ToListAsync();

    var instructors = new List<ApplicationUser>();
    foreach (var email in emails)
    {
      var isEmailValid = new EmailAddressAttribute().IsValid(email);

      if (!isEmailValid)
      {
        continue;
      }

      var user = existingUsers.FirstOrDefault(x => x.Email!.Equals(email, StringComparison.OrdinalIgnoreCase));
      if (user is not null)
      {
        var isStudent = await _users.IsInRoleAsync(user, Roles.Student);
        var isInstructor = await _users.IsInRoleAsync(user, Roles.Instructor);
        if (isStudent)
        {
          continue;
        }

        if (!isInstructor)
        {
          await _users.AddToRoleAsync(user, Roles.Instructor);
        }

        if (!await _user.CanRegister(email))
        {
          continue;
        }

        instructors.Add(user);
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
        await _users.AddToRoleAsync(newUser, Roles.Instructor);
        instructors.Add(newUser);
      }
    }

    // send invites to unconfirmed instructors
    foreach (var instructor in instructors.Where(x => !x.EmailConfirmed))
    {
      await _accountEmail.SendUserInvite(
        new EmailAddress(instructor.Email!),
        await _tokens.GenerateAccountActivationLink(instructor)
      );
    }

    await AssignProject(id, instructors.Select(x => x.Email!).ToList());
  }

  /// <summary>
  /// Remove a instructor from a project.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="instructorId">Instructor id.</param>
  public async Task RemoveInstructor(int id, string instructorId)
  {
    var entity = await _db.Projects
                   .Include(x => x.Instructors)
                   .FirstOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    var instructor = await _users.FindByIdAsync(instructorId) ?? throw new KeyNotFoundException();

    entity.Instructors.Remove(instructor);
    await _db.SaveChangesAsync();

    var emailModel = new ProjectEmailModel(
      new EmailAddress(instructor.Email!)
      {
        Name = instructor.FullName
      },
      entity.Name
    );

    await _projectEmail.RemoveProject(emailModel);
  }

  /// <summary>
  /// Assign project to instructors.
  /// </summary>
  /// <param name="id">Project id.</param>
  /// <param name="emails">Instructor emails.</param>
  private async Task AssignProject(int id, List<string> emails)
  {
    var project = await _db.Projects
                    .Include(x => x.Instructors)
                    .FirstOrDefaultAsync(x => x.Id == id)
                  ?? throw new KeyNotFoundException();

    var normalizedEmails = emails.Select(x => x.ToUpperInvariant()).ToList();
    var users = await _users.Users
      .Where(x => normalizedEmails.Contains(x.NormalizedEmail!))
      .ToListAsync();

    var emailModel = new List<ProjectEmailModel>();

    foreach (var user in users.Where(x => project.Instructors.All(y => y.Id != x.Id)))
    {
      project.Instructors.Add(user);

      emailModel.Add(new ProjectEmailModel(
        new EmailAddress(user.Email!)
        {
          Name = user.FullName
        },
        project.Name
      ));
    }

    await _db.SaveChangesAsync();

    foreach (var model in emailModel)
    {
      await _projectEmail.AssignProject(model);
    }
  }

  /// <summary>
  /// Get project status.
  /// </summary>
  /// <param name="projectId">Project id.</param>
  /// <param name="userId">User id. If provided, status is based on user's submission.</param>
  /// <returns>Project status.</returns>
  private async Task<string> Status(int projectId, string? userId = null)
  {
    if (userId is not null)
    {
      return await _reports.HasStudentSubmitted(projectId, userId)
        ? Stages.Completed
        : Stages.OnGoing;
    }

    return await _reports.HasEveryStudentSubmitted(projectId)
      ? Stages.Completed
      : Stages.OnGoing;
  }
}
