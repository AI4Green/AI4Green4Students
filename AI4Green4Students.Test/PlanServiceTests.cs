using AI4Green4Students.Constants;
using AI4Green4Students.Services;

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
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == PlanStages.Draft);

    //Act
    var planModel = await planService.AdvanceStage(plan.Id);
    var newStage = planModel?.Stage;

    //Assert
    Assert.Equal(PlanStages.InReview, newStage);
  }

  /// <summary>
  /// Test to see if a stage naturally moves onto the next default nextstage property
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_NextStage()
  {
    //Arrange
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == PlanStages.Draft);
    await planService.AdvanceStage(plan.Id, PlanStages.AwaitingChanges);

    //Act
    var planModel = await planService.AdvanceStage(plan.Id);
    var newStage = planModel?.Stage;

    //Assert
    Assert.Equal(PlanStages.InReview, newStage);
  }


  /// <summary>
  /// Try and advance with no next stage or sort order - should return a null stage
  /// </summary>
  [Fact]
  public async void TestAdvanceStage_NextStage_NoNextStage()
  {
    //Arrange
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == PlanStages.Draft);
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
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var plan = _databaseFixture.DbContext.Plans.First(x => x.Stage.DisplayName == PlanStages.Draft);

    //Act
    var planModel = await planService.AdvanceStage(plan.Id, PlanStages.AwaitingChanges);
    var newStage = planModel?.Stage;

    //Assert
    Assert.Equal(PlanStages.AwaitingChanges, newStage);
  }

  [Fact]
  public async void TestCreatePlan()
  {
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();
    var user = _databaseFixture.DbContext.Users.Single(x => x.FullName == StringConstants.StudentUserOne);
    var projectGroup = _databaseFixture.DbContext.ProjectGroups.Single(x => x.Name == StringConstants.FirstProjectGroup);

    //Act
    var plan = await planService.Create(user.Id, new Models.Plan.CreatePlanModel(projectGroup.Id));
    var planFieldResponses = await planService.GetPlanFieldResponses(plan.Id);

    //Assert
    //2 sections, one with 2 fields and other with just one, so defaults to 3 planFieldResponse
    Assert.Equal(3, planFieldResponses.Count);
  }
}
