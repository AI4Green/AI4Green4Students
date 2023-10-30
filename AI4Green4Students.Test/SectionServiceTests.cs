
using AI4Green4Students.Data.Entities;
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
  /// Test to retrieve 2 default sections, one with comments, the other is approved
  /// </summary>
  [Fact]
  public async void TestDefaultExperimentSetup()
  {
    //Arrange
    var sectionService = new SectionService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    //Act
    var sections = await sectionService.List(1, "TestUser");

    //Assert
    //Check the collection twice - first for names, then to see if comments and approval is coming
    //through from child entities
    Assert.Collection(sections, item => Assert.Contains("First Section", item.Name),
                                               item => Assert.Contains("Second Section", item.Name));

    Assert.Collection(sections, item => Assert.True(item.Comments == 2),
                                           item => Assert.True(item.Approved));
  }
}
