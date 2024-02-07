using AI4Green4Students.Models.Section;
using AI4Green4Students.Services;

namespace AI4Green4Students.Tests;
public class SectionServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;
  
  public SectionServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
  }
  
  /// <summary>
  /// Test to retrieve 2 default section summaries, one with comments, the other is approved.
  /// Test to see if the section type comes through and is correct.
  /// </summary>
  [Fact]
  public async void TestListSectionSummaryByPlan()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var literatureReviewService = new LiteratureReviewService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, literatureReviewService, planService);
    var sectionTypeService = new SectionTypeService(_databaseFixture.DbContext);
    
    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var sectionType = await sectionTypeService.List();
    var planSectionType = sectionType.First(x=>x.Name == StringConstants.SectionTypePlan);

    //Act
    var sections = await sectionService.ListSummariesByPlan(1, planSectionType.Id);

    //Assert
    //Check the collection twice - first for names, then to see if comments and approval is coming
    //through from child entities
    Assert.Collection(sections, item => Assert.Contains(StringConstants.FirstSection, item.Name),
                                               item => Assert.Contains(StringConstants.SecondSection, item.Name));

    Assert.Collection(sections, item => Assert.True(item.Comments == 2),
                                           item => Assert.True(item.Approved));
    Assert.All(sections, item => Assert.Equal(StringConstants.SectionTypePlan, item.SectionType.Name));

  }

  /// <summary>
  /// Test the retrieval of a section model with 2 answers to a response, only retrieve the newest response.
  /// Test to see if the section type comes through and is correct.
  /// </summary>
  [Fact]
  public async void TestGetPlanSectionModel()
  {
    //Arrange
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var literatureReviewService = new LiteratureReviewService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, literatureReviewService, planService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    //Get the experiment Id and then the section Id
    var planId = _databaseFixture.DbContext.Plans.First().Id;
    var firstSection = _databaseFixture.DbContext.Sections.First(s => s.Name == StringConstants.FirstSection);

    //Act

    var section = await sectionService.GetPlanFormModel(firstSection.Id, planId);

    //Assert
    Assert.Equal(StringConstants.FirstSection, section.Name);
    Assert.Equal(firstSection.Name, section.Name);
    Assert.Collection(section.FieldResponses, item => Assert.True(item.FieldResponse == StringConstants.FirstResponse));

  }
}
