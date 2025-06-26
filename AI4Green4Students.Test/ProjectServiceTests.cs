namespace AI4Green4Students.Tests;

using Constants;
using Data;
using Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;

public class ProjectServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private readonly TestHostFixture _fixture;

  public ProjectServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();

  /// <summary>
  /// Test to retrieve a project summary for a student.
  /// Test to see if the project and project group name comes through.
  /// Test to see if the plans come through including the stage name.
  /// </summary>
  [Fact]
  public async Task GetStudentProjectSummary_ShouldReturnProjectSummary()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var user = await db.Users.SingleAsync(x => x.FullName == StringConstants.StudentUserOne);
    var project = await db.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);

    //Act
    var studentProjectSummary = await service.GetStudentProjectSummary(project.Id, user.Id);

    //Assert
    Assert.NotNull(studentProjectSummary);
    Assert.NotNull(studentProjectSummary.Plans);
    Assert.Equal(3, studentProjectSummary.Plans.Count);

    var planOne = studentProjectSummary.Plans.Single(x => x.Title == StringConstants.PlanOne);
    var planTwo = studentProjectSummary.Plans.Single(x => x.Title == StringConstants.PlanTwo);
    var planThree = studentProjectSummary.Plans.Single(x => x.Title == StringConstants.PlanThree);

    Assert.Equal(Stages.InReview, planOne.Stage);
    Assert.Equal(Stages.Draft, planTwo.Stage);
    Assert.Equal(Stages.AwaitingChanges, planThree.Stage);

    Assert.Equal(StringConstants.PlanOne, planOne.Title);
  }

  private async Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var projectService = _fixture.GetServiceProvider().GetRequiredService<ProjectService>();

    var dataSeeder = new DataSeeder(db);
    await dataSeeder.SeedDefaultTestExperiment();

    return new ContextModel(db, projectService);
  }

  private record ContextModel(
    ApplicationDbContext Db,
    ProjectService ProjectService
  );
}
