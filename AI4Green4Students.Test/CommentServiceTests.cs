namespace AI4Green4Students.Tests;

using Data;
using Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Comment;
using Services;

public class CommentServiceTests : IClassFixture<TestHostFixture>, IAsyncLifetime
{
  private readonly TestHostFixture _fixture;

  public CommentServiceTests(TestHostFixture fixture) => _fixture = fixture;
  public async Task InitializeAsync() => await _fixture.InitializeServices();
  public async Task DisposeAsync() => await _fixture.DropTestDatabase();

  [Fact]
  public async Task Create_AsStudentForFieldResponse_ShouldCreateComment()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var user = await db.Users.SingleAsync(x => x.FullName == StringConstants.StudentUserOne);
    var field = await db.Fields.FirstAsync(x => x.Name == StringConstants.FirstField);
    var fieldResponse = await db.FieldResponses.SingleAsync(x => x.Field.Id == field.Id);

    //Act
    await service.Create(new CreateCommentModel
    {
      IsInstructor = false, Value = StringConstants.StudentComment, User = user, FieldResponseId = fieldResponse.Id
    });


    //Assert
    var comments = await db.FieldResponses
      .Include(x => x.Conversation.OrderByDescending(y => y.CommentDate))
      .ThenInclude(y => y.Owner)
      .Where(x => x.Id == fieldResponse.Id)
      .SelectMany(x => x.Conversation.OrderByDescending(y => y.CommentDate))
      .ToListAsync();

    Assert.Equal(StringConstants.StudentComment, comments.First().Value);
    Assert.Equal(StringConstants.StudentUserOne, comments.First().Owner.FullName);
  }

  [Fact]
  public async Task Create_AsInstructorForFieldResponse_ShouldCreateComment()
  {
    //Arrange
    var (db, service) = await GetContextModel();
    var user = await db.Users.SingleAsync(x => x.FullName == StringConstants.InstructorUser);

    var field = await db.Fields.FirstAsync(x => x.Name == StringConstants.FirstField);
    var fieldResponse = await db.FieldResponses.SingleAsync(x => x.Field.Id == field.Id);

    //Act
    await service.Create(new CreateCommentModel
    {
      IsInstructor = true, Value = StringConstants.InstructorComment, User = user, FieldResponseId = fieldResponse.Id
    });

    var comments = await db.FieldResponses
      .Include(x => x.Conversation).ThenInclude(y => y.Owner)
      .Where(x => x.Id == fieldResponse.Id)
      .SelectMany(x => x.Conversation.OrderByDescending(y => y.CommentDate))
      .ToListAsync();

    //Assert
    Assert.Equal(StringConstants.InstructorComment, comments.First().Value);
    Assert.Equal(StringConstants.InstructorUser, comments.First().Owner.FullName);
  }

  private async Task<ContextModel> GetContextModel()
  {
    var db = _fixture.GetServiceProvider().GetRequiredService<ApplicationDbContext>();
    var commentService = _fixture.GetServiceProvider().GetRequiredService<CommentService>();

    var dataSeeder = new DataSeeder(db);
    await dataSeeder.SeedDefaultTestExperiment();

    return new ContextModel(db, commentService);
  }

  private sealed record ContextModel(
    ApplicationDbContext Db,
    CommentService CommentService
  );
}
