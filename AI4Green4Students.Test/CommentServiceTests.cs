using AI4Green4Students.Data;
using AI4Green4Students.Services;
using AI4Green4Students.Tests.Fixtures;

namespace AI4Green4Students.Tests;
public class CommentServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;

  public CommentServiceTests(DatabaseFixture databaseFixture)
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
  
  [Fact]
  public async void TestStudentComment()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var commentService = new CommentService(dbContext);

    var user = dbContext.Users.Single(x => x.FullName == StringConstants.StudentUserOne);

    var field = dbContext.Fields.First(x => x.Name == StringConstants.FirstField);
    var fieldResponse = dbContext.FieldResponses.Single(x => x.Field.Id == field.Id);

    //Act
    var comment = await commentService.Create(new Models.Comment.CreateCommentModel
    {
      IsInstructor = false,
      Value = StringConstants.StudentComment,
      User = user,
      FieldResponseId = fieldResponse.Id
    });

    var commentedField = dbContext.FieldResponses.Single(x => x.Id == fieldResponse.Id);

    //Assert
    Assert.True(commentedField.Approved == true);

  }

  [Fact]
  public async void TestInstructorComment()
  {
    //Arrange
    var dbContext = CreateNewDbContext();
    await SeedDefaultTestExperiment(dbContext);
    var commentService = new CommentService(dbContext);

    var user = dbContext.Users.Single(x => x.FullName == StringConstants.InstructorUser);

    var field = dbContext.Fields.First(x => x.Name == StringConstants.FirstField);
    var fieldResponse = dbContext.FieldResponses.Single(x => x.Field.Id == field.Id);

    //Act
    var comment = await commentService.Create(new Models.Comment.CreateCommentModel
    {
      IsInstructor = true,
      Value = StringConstants.InstructorComment,
      User = user,
      FieldResponseId = fieldResponse.Id
    });

    var commentedField = dbContext.FieldResponses.Single(x => x.Id == fieldResponse.Id);

    //Assert
    Assert.True(commentedField.Approved == false);
  }

}
