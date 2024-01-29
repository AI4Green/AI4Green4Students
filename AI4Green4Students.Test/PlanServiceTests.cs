using AI4Green4Students.Services;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Tests;

public class PlanServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;

  public PlanServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
  }

  /// <summary>
  /// Advance the stage of the given plan, using the default sort order
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_SortOrder()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == StringConstants.FirstPlanningStage);

    //Act
    var planModel = await planService.AdvanceStage(plan.Id);
    var newStage = planModel?.Stage;

    //Assert
    Assert.Equal(StringConstants.SecondPlanningStage, newStage);
  }

  /// <summary>
  /// Test to see if a stage naturally moves onto the next default nextstage property
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_NextStage()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == StringConstants.FirstPlanningStage);
    await planService.AdvanceStage(plan.Id, StringConstants.ThirdPlanningStage);

    //Act
    var planModel = await planService.AdvanceStage(plan.Id);
    var newStage = planModel?.Stage;

    //Assert
    Assert.Equal(StringConstants.SecondPlanningStage, newStage);
  }


  /// <summary>
  /// Try and advance with no next stage or sort order - should return a null stage
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_NextStage_NoNextStage()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == StringConstants.FirstPlanningStage);
    await planService.AdvanceStage(plan.Id);

    //Act
    var planModel = await planService.AdvanceStage(plan.Id);
    var newStage = planModel?.Stage;

    //Assert
    Assert.Null(newStage);
  }

  /// <summary>
  /// Advance to a specified stage, as opposed to the default next one.
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_FixedStage()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == StringConstants.FirstPlanningStage);

    //Act
    var planModel = await planService.AdvanceStage(plan.Id, StringConstants.ThirdPlanningStage);
    var newStage = planModel?.Stage;

    //Assert
    Assert.Equal(StringConstants.ThirdPlanningStage, newStage);
  }
}
