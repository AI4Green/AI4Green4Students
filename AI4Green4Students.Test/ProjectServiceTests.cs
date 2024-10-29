using AI4Green4Students.Config;
using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Services;
using AI4Green4Students.Tests.Fixtures;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace AI4Green4Students.Tests;

public class ProjectServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;
  private readonly Mock<AzureStorageService> _mockAZExperimentStorageService;

  public ProjectServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
    _mockAZExperimentStorageService = new Mock<AzureStorageService>(new Mock<BlobServiceClient>().Object, Options.Create(new AzureStorageOptions()));
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
  
  private ProjectService GetProjectService(ApplicationDbContext dbContext)
  {
    var sectionFormServiceFixture = new SectionFormServiceFixture(dbContext);
    var stageServiceFixture = new StageServiceFixture(dbContext);
    var exportServiceFixture = new ExportServiceFixture(dbContext);
    var fieldResponseServiceFixture = new FieldResponseServiceFixture(dbContext);
    
    var planService = new PlanService(dbContext, stageServiceFixture.Service, sectionFormServiceFixture.Service, fieldResponseServiceFixture.Service);
    var literatureReviewService = new LiteratureReviewService(dbContext, stageServiceFixture.Service, sectionFormServiceFixture.Service, fieldResponseServiceFixture.Service);
    var reportService = new ReportService(dbContext, stageServiceFixture.Service, sectionFormServiceFixture.Service, fieldResponseServiceFixture.Service, exportServiceFixture.Service);
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
    Assert.Equal(3, studentProjectSummary.Plans.Count);
    Assert.Collection(studentProjectSummary.Plans,
      item => Assert.Equal(PlanStages.AwaitingChanges, item.Stage),
      item => Assert.Equal(PlanStages.Draft, item.Stage),
      item => Assert.Equal(PlanStages.InReview, item.Stage));
  }
}
