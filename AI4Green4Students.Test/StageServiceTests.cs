using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Tests;

public class StageServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;
  
  public StageServiceTests(DatabaseFixture databaseFixture)
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
  
  /// <summary>
  /// Advance to a specified stage.
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_FixedStage()
  {
    //Arrange
    var dbContext = CreateNewDbContext();  // new instance for each test to avoid side effects
    await SeedDefaultTestExperiment(dbContext);
    var stageService = new StageServiceFixture(dbContext).Service;
    var plan = await dbContext.Plans.FirstAsync(x => x.Stage.DisplayName == PlanStages.Draft);

    //Act
    var planModel = await stageService.AdvanceStage<Plan>(plan.Id, StageTypes.Plan, PlanStages.AwaitingChanges);
    var newStage = planModel?.Stage.DisplayName;

    //Assert
    Assert.Equal(PlanStages.AwaitingChanges, newStage);
  }
  
  /// <summary>
  /// Test to see if a stage naturally moves onto the next default nextstage property.
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_NextStage()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var stageService = new StageServiceFixture(dbContext).Service;
    var plan = await dbContext.Plans.FirstAsync(x => x.Stage.DisplayName == PlanStages.InReview);

    //Act
    var planModel = await stageService.AdvanceStage<Plan>(plan.Id, StageTypes.Plan);
    var newStage = planModel?.Stage.DisplayName;

    //Assert
    Assert.Equal(PlanStages.AwaitingChanges, newStage); // Awaiting is the next stage after InReview
  }
  
  /// <summary>
  /// Advance the stage of the given plan, using the default sort order
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_SortOrder()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var stageService = new StageServiceFixture(dbContext).Service;
    var plan = await dbContext.Plans.FirstAsync(x => x.Stage.DisplayName == PlanStages.Draft);
  
    //Act
    var planModel = await stageService.AdvanceStage<Plan>(plan.Id, StageTypes.Plan);
    var newStage = planModel?.Stage.DisplayName;
  
    //Assert
    Assert.Equal(PlanStages.InReview, newStage); // Draft sort order is 1, InReview sort order is 2
  }
  
  /// <summary>
  /// Try and advance with no next stage or sort order - should return a null stage
  /// AwaitingChanges has no next stage and next sort order is 10, while sort order of AwaitingChanges is 5
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_NextStage_NoNextStage()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var stageService = new StageServiceFixture(dbContext).Service;
    var plan = dbContext.Plans.First(x => x.Stage.DisplayName == PlanStages.AwaitingChanges);

    //Act
    var planModel = await stageService.AdvanceStage<Plan>(plan.Id, StageTypes.Plan);
    var newStage = planModel?.Stage.DisplayName;

    //Assert
    Assert.Null(newStage); 
  }
}
