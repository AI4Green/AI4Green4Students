namespace AI4Green4Students.Tests;

using Constants;
using Data;
using Data.Entities.SectionTypeData;
using Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;

public class StageServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private readonly TestHostFixture _fixture;

  public StageServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();

  /// <summary>
  /// Test advancing to a valid specified stage.
  /// </summary>
  [Fact]
  public async Task Advance_WithStageSpecified_AdvancesToSpecifiedStage()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var plan = await db.Plans.FirstAsync(x => x.Stage.DisplayName == Stages.Draft);

    //Act
    var stageModel = await service.Advance<Plan>(plan.Id, Stages.AwaitingChanges);

    //Assert
    Assert.Equal(Stages.AwaitingChanges, stageModel?.DisplayName);
  }

  /// <summary>
  /// Test advancing without a specified stage. If there's a next stage, it should advance to that.
  /// </summary>
  [Fact]
  public async Task Advance_WithoutStageSpecified_AdvancesToNextStage()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var plan = await db.Plans.FirstAsync(x => x.Stage.DisplayName == Stages.InReview);

    //Act
    var stageModel = await service.Advance<Plan>(plan.Id);

    //Assert
    Assert.Equal(Stages.AwaitingChanges, stageModel?.DisplayName); // Awaiting is the next stage after InReview
  }

  /// <summary>
  /// Try and advance stage without a specified stage, where there's no natural next stage
  /// AwaitingChanges sort order is 5, while next stage sort order is 10
  /// </summary>
  [Fact]
  public async Task Advance_WithoutStageSpecifiedAndHaveNoNextStage_ReturnsNull()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var plan = await db.Plans.FirstAsync(x => x.Stage.DisplayName == Stages.AwaitingChanges);

    //Act
    var stageModel = await service.Advance<Plan>(plan.Id);
    var newStage = stageModel?.DisplayName;

    //Assert
    Assert.Null(newStage);
  }

  private async Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var stageService = _fixture.GetServiceProvider().GetRequiredService<StageService>();

    var dataSeeder = new DataSeeder(db);
    await dataSeeder.SeedDefaultTestExperiment();

    return new ContextModel(db, stageService);
  }

  private record ContextModel(
    ApplicationDbContext Db,
    StageService StageService
  );
}
