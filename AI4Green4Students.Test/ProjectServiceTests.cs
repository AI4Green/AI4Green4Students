using AI4Green4Students.Services;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Tests;

public class ProjectServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;

  public ProjectServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
  }

  /// <summary>
  /// Test to retrieve a project summary for a student.
  /// Test to see if the project and project group name comes through.
  /// Test to see if the plans come through including the stage name.
  /// </summary>
  [Fact]
  public async void GetStudentProjectSummary()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var literatureReviewService = new LiteratureReviewService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var projectService = new ProjectService(_databaseFixture.DbContext, literatureReviewService, planService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var user = await _databaseFixture.DbContext.Users.SingleAsync(x => x.FullName == StringConstants.StudentUserOne);
    var project = await _databaseFixture.DbContext.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);

    //Act
    var studentProjectSummary = await projectService.GetStudentProjectSummary(project.Id, user.Id);


    //Assert
    Assert.Equal(StringConstants.FirstProject, studentProjectSummary.ProjectName);
    Assert.Equal(StringConstants.FirstProjectGroup, studentProjectSummary.ProjectGroupName);
    Assert.Equal(2, studentProjectSummary.Plans.Count);
    Assert.Collection(studentProjectSummary.Plans,
      item => Assert.Equal(StringConstants.SecondPlanningStage, item.Stage),
      item => Assert.Equal(StringConstants.FirstPlanningStage, item.Stage));
  }
  
  /// <summary>
  /// Test to retrieve a project summary for a project group.
  /// Test to see if the project and project group name comes through.
  /// Test to see if the all project groups plans come through including their stage.
  /// </summary>
  [Fact]
  public async void GetProjectGroupProjectSummary()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var literatureReviewService = new LiteratureReviewService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var projectService = new ProjectService(_databaseFixture.DbContext, literatureReviewService, planService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();
    
    var projectGroup =
      await _databaseFixture.DbContext.ProjectGroups.SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);
    
    //Act
    var projectGroupProjectSummary = await projectService.GetProjectGroupProjectSummary(projectGroup.Id);
    
    //Assert
    Assert.Equal(StringConstants.FirstProject, projectGroupProjectSummary.ProjectName);
    Assert.Equal(StringConstants.FirstProjectGroup, projectGroupProjectSummary.ProjectGroupName);
    Assert.Equal(3, projectGroupProjectSummary.Plans.Count);
    Assert.Collection(projectGroupProjectSummary.Plans,
      item => Assert.Equal(StringConstants.SecondPlanningStage, item.Stage),
      item => Assert.Equal(StringConstants.FirstPlanningStage, item.Stage),
      item => Assert.Equal(StringConstants.FirstPlanningStage, item.Stage));
  }

  
}
