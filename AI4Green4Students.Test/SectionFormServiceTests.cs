using AI4Green4Students.Constants;
using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Data.Entities.SectionTypeData;
using AI4Green4Students.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Tests;

public class SectionFormServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;
  
  public SectionFormServiceTests(DatabaseFixture databaseFixture)
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
  
  private static Task<Plan> GetPlan(ApplicationDbContext db, string title, string ownerName)
  {
    return db.Plans
      .Where(x => x.Title == title && x.Owner.FullName == ownerName)
      .Include(x => x.Stage)
      .SingleAsync();
  }
  
  private static Task<Section> GetSection (ApplicationDbContext db, string name, string sectionTypeName)
  {
    return db.Sections
      .Where(x => x.Name == name && x.SectionType.Name == sectionTypeName)
      .SingleAsync();
  }
  
  /// <summary>
  /// Test the retrieval of a section model with 2 answers to a response, only retrieve the newest response.
  /// Test to see if the section type comes through and is correct.
  /// </summary>
  [Fact]
  public async void GetSectionForm()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var plan = await GetPlan(dbContext, StringConstants.PlanOne, StringConstants.StudentUserOne);
    var section = await GetSection(dbContext, StringConstants.PlanFirstSection, SectionTypes.Plan);
    var sectionFormService = new SectionFormServiceFixture(dbContext).Service;
    var sectionFieldResponses = await sectionFormService.ListBySection<Plan>(plan.Id, section.Id);
    
    //Act
    var sectionForm = await sectionFormService.GetFormModel(section.Id, sectionFieldResponses);
    var thirdFieldResponse = sectionForm.FieldResponses.
      SingleOrDefault(x => x.Name == StringConstants.ThirdField)?
      .FieldResponse.ToString();

    //Assert
    Assert.Equal(StringConstants.PlanFirstSection, sectionForm.Name);
    Assert.True(thirdFieldResponse == StringConstants.ApprovedResponse); // third field has two responses, check if the newest one comes through
  }
  
  /// <summary>
  /// Test to retrieve 2 default section summaries, one with comments, the other is approved.
  /// Test to see if the section type comes through and is correct.
  /// </summary>
  [Fact]
  public async Task TestSummaryList()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var plan = await GetPlan(dbContext, StringConstants.PlanOne, StringConstants.StudentUserOne);
    var stageService = new StageServiceFixture(dbContext).Service;
    var stagePermissions = await stageService.GetStagePermissions(plan.Stage, StageTypes.Plan);
    var sectionFormService = new SectionFormServiceFixture(dbContext).Service;
    var fieldResponses = await sectionFormService.ListBySectionType<Plan>(plan.Id);
    
    //Act
    var summary = await sectionFormService.GetSummaryModel(plan.Project.Id, SectionTypes.Plan, fieldResponses, stagePermissions, plan.Stage.DisplayName);

    //Assert
    Assert.Collection(summary,
      item => Assert.Contains(StringConstants.PlanFirstSection, item.Name),
      item => Assert.Contains(StringConstants.PlanSecondSection, item.Name));

    Assert.Collection(summary, 
      item => Assert.True(item.Comments == 3), // first section has 3 comments
      item => Assert.True(item.Approved)); // second section is approved
    
    Assert.All(summary, item => Assert.Equal(SectionTypes.Plan, item.SectionType.Name));
  }
  
}
