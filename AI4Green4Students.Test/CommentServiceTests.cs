using AI4Green4Students.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI4Green4Students.Tests;
public class CommentServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _databaseFixture;

  public CommentServiceTests(DatabaseFixture databaseFixture)
  {
    _databaseFixture = databaseFixture;
  }

  [Fact]
  public async void TestStudentComment()
  {
    //Arrange
    var commentService = new CommentService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var user = _databaseFixture.DbContext.Users.Single(x => x.FullName == StringConstants.StudentUser);

    var field = _databaseFixture.DbContext.Fields.Single(x => x.Name == StringConstants.FirstField);
    var fieldResponse = _databaseFixture.DbContext.FieldResponses.Single(x => x.Field.Id == field.Id);

    //Act
    var comment = await commentService.Create(new Models.Comment.CreateCommentModel
    {
      IsInstructor = false,
      Value = StringConstants.StudentComment,
      User = user,
      FieldResponseId = fieldResponse.Id
    });

    var commentedField = _databaseFixture.DbContext.FieldResponses.Single(x => x.Id == fieldResponse.Id);

    //Assert
    Assert.True(commentedField.Approved == true);

  }

  [Fact]
  public async void TestInstructorComment()
  {
    //Arrange
    var commentService = new CommentService(_databaseFixture.DbContext);

    var dataSeeder = new DataSeeder(_databaseFixture.DbContext);
    await dataSeeder.SeedDefaultTestExperiment();

    var user = _databaseFixture.DbContext.Users.Single(x => x.FullName == StringConstants.InstructorUser);

    var field = _databaseFixture.DbContext.Fields.Single(x => x.Name == StringConstants.FirstField);
    var fieldResponse = _databaseFixture.DbContext.FieldResponses.Single(x => x.Field.Id == field.Id);

    //Act
    var comment = await commentService.Create(new Models.Comment.CreateCommentModel
    {
      IsInstructor = true,
      Value = StringConstants.InstructorComment,
      User = user,
      FieldResponseId = fieldResponse.Id
    });

    var commentedField = _databaseFixture.DbContext.FieldResponses.Single(x => x.Id == fieldResponse.Id);

    //Assert
    Assert.True(commentedField.Approved == false);
  }

}
