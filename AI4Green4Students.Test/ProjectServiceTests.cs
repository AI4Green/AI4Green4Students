using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Services;
using AI4Green4Students.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Tests;

public class ProjectServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;

  public ProjectServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
  }
  
  private ApplicationDbContext CreateNewDbContext()
  {
    return _databaseFixture.CreateNewContext();
  }
  
  private static async Task SeedDefaultTestExperiment(ApplicationDbContext dbContext)
  {
    var dataSeeder = new DataSeeder(dbContext);
    await dataSeeder.SeedDefaultTestExperiment();
  }
  
  private static ProjectService GetProjectService(ApplicationDbContext dbContext)
  {
    var sectionFormServiceFixture = new SectionFormServiceFixture(dbContext);
    var stageServiceFixture = new StageServiceFixture(dbContext);
    
    var sectionTypeService = new SectionTypeService(dbContext);
    var planService = new PlanService(dbContext, sectionTypeService, stageServiceFixture.Service, sectionFormServiceFixture.Service);
    var literatureReviewService = new LiteratureReviewService(dbContext, sectionTypeService, stageServiceFixture.Service, sectionFormServiceFixture.Service);
    var reportService = new ReportService(dbContext, stageServiceFixture.Service, sectionFormServiceFixture.Service);
    return new ProjectService(dbContext, literatureReviewService, planService, reportService);
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
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var user = await dbContext.Users.SingleAsync(x => x.FullName == StringConstants.StudentUserOne);
    var project = await dbContext.Projects.SingleAsync(x => x.Name == StringConstants.FirstProject);
    var projectService = GetProjectService(dbContext);

    //Act
    var studentProjectSummary = await projectService.GetStudentProjectSummary(project.Id, user.Id);


    //Assert
    Assert.Equal(StringConstants.FirstProject, studentProjectSummary.ProjectName);
    Assert.Equal(StringConstants.FirstProjectGroup, studentProjectSummary.ProjectGroupName);
    Assert.Equal(3, studentProjectSummary.Plans.Count);
    Assert.Collection(studentProjectSummary.Plans,
      item => Assert.Equal(PlanStages.AwaitingChanges, item.Stage),
      item => Assert.Equal(PlanStages.Draft, item.Stage),
      item => Assert.Equal(PlanStages.InReview, item.Stage));
  }
  
  /// <summary>
  /// Test to retrieve a project summary for a project group.
  /// Test to see if the project and project group name comes through.
  /// Test to see if the all project groups plans (excludes drafts) come through including their stage.
  /// </summary>
  [Fact]
  public async void GetProjectGroupProjectSummary()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var projectGroup = await dbContext.ProjectGroups.SingleAsync(x => x.Name == StringConstants.FirstProjectGroup);
    var projectService = GetProjectService(dbContext);
    
    //Act
    var projectGroupProjectSummary = await projectService.GetProjectGroupProjectSummary(projectGroup.Id);
    
    //Assert
    Assert.Equal(StringConstants.FirstProject, projectGroupProjectSummary.ProjectName);
    Assert.Equal(StringConstants.FirstProjectGroup, projectGroupProjectSummary.ProjectGroupName);
    Assert.Equal(2, projectGroupProjectSummary.Plans.Count);
    Assert.Collection(projectGroupProjectSummary.Plans,
      item => Assert.Equal(PlanStages.AwaitingChanges, item.Stage),
      item => Assert.Equal(PlanStages.InReview, item.Stage));
  }

  
}
