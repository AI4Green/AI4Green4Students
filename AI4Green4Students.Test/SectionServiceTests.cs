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
  /// Test to retrieve 2 default section summaries, one with comments, the other is approved
  /// </summary>
  [Fact]
  public async void TestListSectionSummary()
  {
    //Arrange
    var sectionService = new SectionService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    //Act
    var sections = await sectionService.List(1);

    //Assert
    //Check the collection twice - first for names, then to see if comments and approval is coming
    //through from child entities
    Assert.Collection(sections, item => Assert.Contains(StringConstants.FirstSection, item.Name),
                                               item => Assert.Contains(StringConstants.SecondSection, item.Name));

    Assert.Collection(sections, item => Assert.True(item.Comments == 2),
                                           item => Assert.True(item.Approved));
  }

  /// <summary>
  /// Test the retrieval of a section model with 2 answers to a response, only retrieve the newest response.
  /// </summary>
  [Fact]
  public async void TestGetSectionModel()
  {
    //Arrange
    var sectionService = new SectionService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    //Act
    var section = await sectionService.GetFormModel(1, 1);

    //Assert
    Assert.Equal(StringConstants.FirstSection, section.Name);
    Assert.Collection(section.FieldResponses, item => Assert.True(item.FieldResponse == StringConstants.FirstResponse));

  }
}
