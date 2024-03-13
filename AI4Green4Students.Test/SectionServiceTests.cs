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
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);
    var sectionTypeService = new SectionTypeService(_databaseFixture.DbContext);
    
    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var sectionType = await sectionTypeService.List();
    var planSectionType = sectionType.First(x=>x.Name == StringConstants.SectionTypePlan);

    //Act
    var sections = await planService.ListSummariesByPlan(1, planSectionType.Id);

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
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    //Get the experiment Id and then the section Id
    var planId = _databaseFixture.DbContext.Plans.First().Id;
    var firstSection = _databaseFixture.DbContext.Sections.First(s => s.Name == StringConstants.FirstSection);

    //Act

    var section = await planService.GetPlanFormModel(firstSection.Id, planId);

    //Assert
    Assert.Equal(StringConstants.FirstSection, section.Name);
    Assert.Equal(firstSection.Name, section.Name);
    Assert.True(section.FieldResponses[1].FieldResponse?.GetString() == StringConstants.FirstResponse); // first field has two responses, check if the newest one comes through

  }

  /// <summary>
  /// During a draft plan, the first time a section is saved, it needs to create new fieldvalues.
  /// We expect to find only single values in the response collections at the end of this process.
  /// </summary>
  [Fact]
  public async void TestSectionService_DraftPlan_CreateFields()
  {
    //Arrange
    var reportService = new ReportService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext));
    var sectionService = new SectionService(_databaseFixture.DbContext, reportService);
    var planService = new PlanService(_databaseFixture.DbContext, new StageService(_databaseFixture.DbContext), sectionService);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    
    //Act


    //Assert


  }

  /// <summary>
  /// During a draft plan, when a section is saved subsequently (has been saved once before), we still only expect
  /// to find single values in the response collections, but they have now been altered at the end of the process.
  /// </summary>
  [Fact]
  public async void TestSectionService_DraftPlan_EditFields()
  {

  }

  /// <summary>
  /// During a plan having changes requested, when a field is altered for the first time since the plan's state change,
  /// add a new field value to the collection. So for any changed field responses, we're expecting 2 different responses:
  /// the original, and the newest draft. This only applies to fields which comments - other fields should not be able to have 
  /// edited responses.
  /// </summary>
  [Fact]
  public async void TestSectionService_ChangesRequestedPlan_EditFieldsInitial()
  {

  }

  /// <summary>
  /// During a plan having changes requested, when a field is altered for a subsequent time since the plan's state change, 
  /// edit the latest field value response in that fields collection. No new field responses should be added to commented fields.
  /// </summary>
  [Fact]
  public async void TestSectionService_ChangesRequestedPlan_EditFieldsSubsequent()
  {

  }
}
