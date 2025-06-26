namespace AI4Green4Students.Services;

using Data;
using Data.Entities;
using Data.Entities.SectionTypeData;
using Microsoft.EntityFrameworkCore;
using Models.Comment;

public class CommentService
{
  private readonly ApplicationDbContext _db;

  public CommentService(ApplicationDbContext db) => _db = db;

  /// <summary>
  /// Create a comment.
  /// </summary>
  /// <param name="model">Create model.</param>
  /// <returns>Result.</returns>
  public async Task<CommentModel> Create(CreateCommentModel model)
  {
    var fieldResponse = await _db.FieldResponses
                          .Include(x => x.FieldResponseValues)
                          .FirstAsync(x => x.Id == model.FieldResponseId)
                        ?? throw new KeyNotFoundException();

    var comment = new Comment
    {
      Value = model.Value,
      Owner = model.User!,
      CommentDate = DateTimeOffset.UtcNow,
      Read = false,
    };

    fieldResponse.Conversation.Add(comment);

    // add a new response value with the same value as the previous one - this will
    // be the new edited value in the future, while the previous entry will remain as a way to see previous answers
    fieldResponse.Approved = false;
    var latestFieldResponseValue = fieldResponse
      .FieldResponseValues.OrderByDescending(x => x.ResponseDate)
      .FirstOrDefault();

    var newFieldResponseValue = new FieldResponseValue
    {
      FieldResponse = fieldResponse,
      Value = latestFieldResponseValue?.Value ?? string.Empty,
      ResponseDate = DateTimeOffset.UtcNow,
    };
    fieldResponse.FieldResponseValues.Add(newFieldResponseValue);

    _db.Update(fieldResponse);
    await _db.SaveChangesAsync();

    return await Get(comment.Id);
  }

  /// <summary>
  /// Get a comment.
  /// </summary>
  /// <param name="id">Comment id.</param>
  /// <returns>Comment.</returns>
  private async Task<CommentModel> Get(int id)
  {
    var result = await _db.Comments.AsNoTracking()
                   .Include(x => x.Owner)
                   .Where(x => x.Id == id)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new CommentModel(result);
  }

  /// <summary>
  /// Update a comment.
  /// </summary>
  /// <param name="id">Comment id.</param>
  /// <param name="model">Update model.</param>
  /// <returns>Result.</returns>
  public async Task<CommentModel> Set(int id, CreateCommentModel model)
  {
    var entity = await _db.Comments.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Value = model.Value;
    entity.Read = false;
    entity.CommentDate = DateTime.UtcNow;

    _db.Comments.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  /// <summary>
  /// Mark a comment as read.
  /// </summary>
  /// <param name="id">Id.</param>
  public async Task MarkCommentAsRead(int id)
  {
    var entity = await _db.Comments.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Read = true;

    _db.Comments.Update(entity);
    await _db.SaveChangesAsync();
    await Get(id);
  }

  /// <summary>
  /// Approve a field response.
  /// </summary>
  /// <param name="id">Field response id.</param>
  /// <param name="isApproved">Whether the field response is approved.</param>
  public async Task ApproveFieldResponse(int id, bool isApproved)
  {
    var entity = await _db.FieldResponses.Where(x => x.Id == id).FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    entity.Approved = isApproved;

    _db.FieldResponses.Update(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Delete a comment.
  /// </summary>
  /// <param name="id">Comment id.</param>
  public async Task Delete(int id)
  {
    var entity = await _db.Comments.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();

    _db.Comments.Remove(entity);
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// List comments by field response.
  /// </summary>
  /// <param name="id">Field response id.</param>
  /// <returns>List.</returns>
  public async Task<List<CommentModel>> ListByFieldResponse(int id)
  {
    var fr = await _db.FieldResponses
               .Include(x => x.Conversation).ThenInclude(x => x.Owner)
               .SingleOrDefaultAsync(x => x.Id == id)
             ?? throw new KeyNotFoundException();

    return fr.Conversation.Select(x => new CommentModel(x)).ToList();
  }

  /// <summary>
  /// Count comments by section type. E.g. plan, literature review, etc.
  /// </summary>
  /// <typeparam name="T">Section type. E.g. plan, literature review, etc.</typeparam>
  /// <param name="id">Entity id.</param>
  /// <returns>Count.</returns>
  public async Task<int> Count<T>(int id) where T : BaseSectionTypeData
    => await _db.Set<T>()
      .Where(x => x.Id == id)
      .SelectMany(x => x.FieldResponses)
      .SelectMany(x => x.Conversation)
      .CountAsync(x => !x.Read);
}
