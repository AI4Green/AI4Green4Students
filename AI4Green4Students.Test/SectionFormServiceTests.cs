namespace AI4Green4Students.Tests;

using Constants;
using Data;
using Data.Entities.SectionTypeData;
using Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;

public class SectionFormServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private readonly TestHostFixture _fixture;

  public SectionFormServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();

  /// <summary>
  /// Test the retrieval of a section model with 2 answers to a response, only retrieve the newest response.
  /// Test to see if the section type comes through and is correct.
  /// </summary>
  [Fact]
  public async Task GetSectionForm_ShouldReturnSectionForm()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var plan = await db.Plans.SingleAsync(x
      => x.Title == StringConstants.PlanOne && x.Owner.FullName == StringConstants.StudentUserOne);
    var section = await db.Sections.SingleAsync(x
      => x.Name == StringConstants.PlanFirstSection && x.SectionType.Name == SectionTypes.Plan);

    //Act
    var sectionForm = await service.GetSectionForm<Plan>(plan.Id, section.Id);

    //Assert
    var response = sectionForm.FieldResponses.SingleOrDefault(x => x.Name == StringConstants.ThirdField)?.FieldResponse.ToString();
    Assert.Equal(StringConstants.ApprovedResponse, response); // third field has two responses, check if the newest one comes through
  }

  /// <summary>
  /// Test to retrieve 2 default section summaries, one with comments, the other is approved.
  /// Test to see if the section type comes through and is correct.
  /// </summary>
  [Fact]
  public async Task ListSummary_ShouldReturnSummaries()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var plan = await db.Plans.SingleAsync(x
      => x.Title == StringConstants.PlanOne && x.Owner.FullName == StringConstants.StudentUserOne);

    //Act
    var summary = await service.ListSummary<Plan>(plan.Id);

    //Assert
    Assert.NotNull(summary);
    Assert.Equal(2, summary.Count);

    // Test each section individually
    var firstSection = summary.Single(s => s.Name == StringConstants.PlanFirstSection);
    Assert.Equal(3, firstSection.Comments);
    Assert.False(firstSection.Approved);
    Assert.Equal(SectionTypes.Plan, firstSection.SectionType.Name);

    var secondSection = summary.Single(s => s.Name == StringConstants.PlanSecondSection);
    Assert.Equal(2, secondSection.Comments);
    Assert.True(secondSection.Approved);
    Assert.Equal(SectionTypes.Plan, secondSection.SectionType.Name);
  }

  private async Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var sectionFormService = _fixture.GetServiceProvider().GetRequiredService<SectionFormService>();

    var dataSeeder = new DataSeeder(db);
    await dataSeeder.SeedDefaultTestExperiment();

    return new ContextModel(db, sectionFormService);
  }

  private record ContextModel(
    ApplicationDbContext Db,
    SectionFormService SectionFormService
  );
}
