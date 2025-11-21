namespace AI4Green4Students.Tests;

using Auth;
using Data;
using Data.Entities.SectionTypeData;
using Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Project;
using Models.ProjectGroup;
using Services;

public class ProjectGroupServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private const string _newProjectGroup = "New Project Group";
  private const string _secondProjectGroup = "Second Project Group";
  private const string _startDate = "2025-11-18";
  private const string _newStartDate = "2025-11-25";
  private const string _planningDeadline = "2025-12-17";
  private const string _experimentDeadline = "2026-01-18";
  private const string _newStudentOneEmail = "newstudent1@test.com";
  private const string _newStudentTwoEmail = "newstudent2@test.com";
  private const string _invalidEmail = "invalid-email";
  private readonly TestHostFixture _fixture;

  public ProjectGroupServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();

  /// <summary>
  /// List instructor's project groups.
  /// </summary>
  [Fact]
  public async Task ListByInstructor_ShouldReturnProjectGroups()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var instructor = await db.Users.SingleAsync(x => x.FullName == StringConstants.InstructorUser);
    var project = await db.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);
    project.Instructors.Add(instructor);
    await db.SaveChangesAsync();

    // Act
    var result = await service.ListByInstructor(project.Id, instructor.Id);

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal(StringConstants.FirstProjectGroup, result.First().Name);
    Assert.Equal(project.Id, result.First().Project.Id);
    Assert.Equal(2, result.First().Students.Count);
  }

  /// <summary>
  /// List student's project groups.
  /// </summary>
  [Fact]
  public async Task ListByStudent_ShouldReturnProjectGroups()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var student = await db.Users.SingleAsync(x => x.FullName == StringConstants.StudentUserOne);
    var project = await db.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);

    // Act
    var result = await service.ListByStudent(project.Id, student.Id);

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal(StringConstants.FirstProjectGroup, result.First().Name);
    Assert.Equal(project.Id, result.First().Project.Id);
  }

  /// <summary>
  /// Get project group by ID.
  /// </summary>
  [Fact]
  public async Task Get_ShouldReturnProjectGroup()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var projectGroup = await db.ProjectGroups.SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);

    // Act
    var result = await service.Get(projectGroup.Id);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(projectGroup.Id, result.Id);
    Assert.Equal(StringConstants.FirstProjectGroup, result.Name);
    Assert.Equal(2, result.Students.Count);
  }

  /// <summary>
  /// Delete project group.
  /// </summary>
  [Fact]
  public async Task Delete_ShouldRemoveProjectGroup()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var projectGroup = await db.ProjectGroups.SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);
    var projectGroupId = projectGroup.Id;

    // Act
    await service.Delete(projectGroupId);

    // Assert
    var entity = await db.ProjectGroups.FindAsync(projectGroupId);
    Assert.Null(entity);
  }

  /// <summary>
  /// Create a new project group.
  /// </summary>
  [Fact]
  public async Task Create_ShouldCreateProjectGroup()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var project = await db.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);
    var model = new CreateProjectGroupModel(
      _newProjectGroup,
      project.Id,
      _startDate,
      _planningDeadline,
      _experimentDeadline
    );

    // Act
    var result = await service.Create(model);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(_newProjectGroup, result.Name);
    Assert.Equal(project.Id, result.Project.Id);
    Assert.Equal(_startDate, result.StartDate);
    Assert.Equal(_planningDeadline, result.PlanningDeadline);
    Assert.Equal(_experimentDeadline, result.ExperimentDeadline);
  }

  /// <summary>
  /// Create with existing name updates instead of creating.
  /// </summary>
  [Fact]
  public async Task Create_WithExistingName_ShouldUpdateInstead()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var project = await db.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);
    var existingGroup = await db.ProjectGroups.SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);

    var model = new CreateProjectGroupModel(
      existingGroup.Name,
      project.Id,
      _newStartDate,
      _planningDeadline,
      _experimentDeadline
    );

    // Act
    var result = await service.Create(model);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(existingGroup.Id, result.Id);
    Assert.Equal(_newStartDate, result.StartDate);
  }

  /// <summary>
  /// Update project group.
  /// </summary>
  [Fact]
  public async Task Set_ShouldUpdateProjectGroup()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var projectGroup = await db.ProjectGroups.SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);
    var project = await db.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);

    var model = new CreateProjectGroupModel(
      _newProjectGroup,
      project.Id,
      _startDate,
      _planningDeadline,
      _experimentDeadline
    );

    // Act
    var result = await service.Set(projectGroup.Id, model);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(projectGroup.Id, result.Id);
    Assert.Equal(_newProjectGroup, result.Name);
  }

  /// <summary>
  /// Invite students.
  /// </summary>
  [Fact]
  public async Task InviteStudents_ShouldCreateNewStudentsAndAssignToGroup()
  {
    // Arrange
    var (db, service) = await GetContextModel();

    var projectGroup = await db.ProjectGroups.AsNoTracking()
      .SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);

    var newStudentEmails = new List<string>
    {
      _newStudentOneEmail, _newStudentTwoEmail
    };

    var model = new InviteModel(newStudentEmails);

    // Act
    await service.InviteStudents(projectGroup.Id, model.Emails, "Test");

    // Assert
    var studentRole = await db.Roles.SingleAsync(x => x.Name != null && EF.Functions.ILike(x.Name, Roles.Student));

    var users = await db.Users
      .Where(x => newStudentEmails.Contains(x.Email!))
      .ToListAsync();

    var students = await db.UserRoles
      .Where(x => users.Select(y => y.Id).Contains(x.UserId))
      .ToListAsync();

    projectGroup = await db.ProjectGroups
      .Include(x => x.Students)
      .SingleAsync(x => x.Id == projectGroup.Id);

    Assert.Equal(2, users.Count);
    Assert.Contains(users, x => x.Email == _newStudentOneEmail);
    Assert.Contains(users, x => x.Email == _newStudentTwoEmail);
    Assert.Equal(2, students.Count);

    foreach (var student in students)
    {
      Assert.Equal(studentRole.Id, student.RoleId);
    }

    foreach (var email in newStudentEmails)
    {
      Assert.Contains(projectGroup.Students, x => x.Email == email);
    }
  }

  /// <summary>
  /// Test InviteStudents skips invalid email addresses.
  /// </summary>
  [Fact]
  public async Task InviteStudents_ShouldSkipInvalidEmails()
  {
    // Arrange
    var (db, service) = await GetContextModel();

    var projectGroup =
      await db.ProjectGroups.AsNoTracking().SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);

    var newStudentEmails = new List<string>
    {
      _newStudentOneEmail, _invalidEmail
    };

    var model = new InviteModel(newStudentEmails);

    // Act
    await service.InviteStudents(projectGroup.Id, model.Emails, "Test");

    // Assert
    var users = await db.Users
      .Where(x => newStudentEmails.Contains(x.Email!))
      .ToListAsync();

    Assert.Single(users);
    Assert.Contains(users, x => x.Email == _newStudentOneEmail);
    Assert.DoesNotContain(users, x => x.Email == _invalidEmail);
  }

  /// <summary>
  /// Invite student. Should remove student from other groups in same project.
  /// </summary>
  [Fact]
  public async Task InviteStudents_ShouldRemoveStudentFromOtherGroupsInSameProject()
  {
    // Arrange
    var (db, service) = await GetContextModel();
    var project = await db.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);

    var secondGroup = new ProjectGroup
    {
      Name = _secondProjectGroup, Project = project
    };
    await db.ProjectGroups.AddAsync(secondGroup);
    await db.SaveChangesAsync();

    var projectGroups = await db.ProjectGroups
      .Include(x => x.Students)
      .ToListAsync();

    // remove student from first group and add to second group
    var student = await db.Users.SingleAsync(x => x.FullName == StringConstants.StudentUserOne);

    var firstGroup = projectGroups.Single(x => x.Name == StringConstants.FirstProjectGroup);
    firstGroup.Students.Remove(student);

    secondGroup = projectGroups.Single(x => x.Name == _secondProjectGroup);
    secondGroup.Students.Add(student);

    await db.SaveChangesAsync();

    var model = new InviteModel([student.Email!]);

    // Act
    await service.InviteStudents(firstGroup.Id, model.Emails, "Test");

    // Assert
    projectGroups = await db.ProjectGroups
      .Include(x => x.Students)
      .ToListAsync();

    firstGroup = projectGroups.Single(x => x.Name == StringConstants.FirstProjectGroup);
    secondGroup = projectGroups.Single(x => x.Name == _secondProjectGroup);

    Assert.DoesNotContain(secondGroup.Students, x => x.Id == student.Id);
    Assert.Contains(firstGroup.Students, x => x.Id == student.Id);
  }

  /// <summary>
  /// Remove a student from project group.
  /// </summary>
  [Fact]
  public async Task RemoveStudent_ShouldRemoveStudentFromGroup()
  {
    // Arrange
    var (db, service) = await GetContextModel();

    var projectGroup = await db.ProjectGroups.AsNoTracking()
      .Include(x => x.Students)
      .SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);

    var student = await db.Users.SingleAsync(x => x.FullName == StringConstants.StudentUserOne);
    var initialCount = projectGroup.Students.Count;

    var model = new RemoveModel(student.Id);

    // Act
    await service.RemoveStudent(projectGroup.Id, model.Id);

    // Assert
    projectGroup = await db.ProjectGroups.AsNoTracking()
      .Include(x => x.Students)
      .SingleAsync(x => x.Id == projectGroup.Id);

    Assert.Equal(initialCount - 1, projectGroup.Students.Count);
    Assert.DoesNotContain(projectGroup.Students, s => s.Id == student.Id);
  }

  private async Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var projectGroupService = _fixture.GetServiceProvider().GetRequiredService<ProjectGroupService>();

    var dataSeeder = new DataSeeder(db);
    await dataSeeder.SeedDefaultTestExperiment();

    return new ContextModel(db, projectGroupService);
  }

  private sealed record ContextModel(
    ApplicationDbContext Db,
    ProjectGroupService ProjectGroupService
  );
}
