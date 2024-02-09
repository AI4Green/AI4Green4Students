using AI4Green4Students.Data;
using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Comment;
using Microsoft.EntityFrameworkCore;

namespace AI4Green4Students.Services;

public class CommentService
{
  private readonly ApplicationDbContext _db;

  public CommentService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<CommentModel> Create(CreateCommentModel model)
  {
    var fieldResponseEntity = _db.FieldResponses
      .Include(x => x.FieldResponseValues)
      .Single(x => x.Id == model.FieldResponseId) 
      ?? throw new KeyNotFoundException("Field response Id not found");

    var commentEntity = new Comment
    {
      Value = model.Value,
      Owner = model.User,
      CommentDate = DateTime.Now,
      Read = false
    };

    fieldResponseEntity.Conversation.Add(commentEntity);

    //need to check the role of the user - if it is an invigilator then we need to mark the fieldresponse valid as false
    // when a field response is set to false, add a new response value with the same value as the previous one - this will
    // be the new edited value in the future, while the previous entry will remain as a way to see previous answers
    if (model.IsInstructor)
    {
      fieldResponseEntity.Approved = false;
      var latestFieldResponseValue = fieldResponseEntity.FieldResponseValues.OrderByDescending(x => x.ResponseDate).FirstOrDefault();
      var newFieldResponseValue = new FieldResponseValue { Value = latestFieldResponseValue.Value, ResponseDate = DateTime.Now };

      fieldResponseEntity.FieldResponseValues.Add(newFieldResponseValue);
    }
    else
      fieldResponseEntity.Approved = true;


    _db.Update(fieldResponseEntity);
    await _db.SaveChangesAsync();

    return await Get(commentEntity.Id);
  }

  public async Task<CommentModel> Get(int id)
  {
    var result = await _db.Comments
                   .AsNoTracking()
                   .Include(x => x.Owner)
                   .Where(x => x.Id == id)
                   .SingleOrDefaultAsync()
                 ?? throw new KeyNotFoundException();

    return new CommentModel(result);
  }

  public async Task<CommentModel> Set(int id, CreateCommentModel model)
  {
    var entity = await _db.Comments
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if project does not exist

    entity.Value = model.Value;
    entity.Read = false;
    entity.CommentDate = DateTime.Now;

    _db.Comments.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  public async Task MarkCommentAsRead(int id)
  {
    var entity = await _db.Comments
                   .Where(x => x.Id == id)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if project does not exist

    entity.Read = true;

    _db.Comments.Update(entity);
    await _db.SaveChangesAsync();
    await Get(id);
  }

  public async Task ApproveFieldResponse(int fieldResponseId, bool isApproved)
  {
    var entity = await _db.FieldResponses
                   .Where(x => x.Id == fieldResponseId)
                   .FirstOrDefaultAsync()
                 ?? throw new KeyNotFoundException(); // if project does not exist

    entity.Approved = isApproved;

    _db.FieldResponses.Update(entity);
    await _db.SaveChangesAsync();
  }

  public async Task Delete(int id)
  {
    var entity = await _db.Comments
                   .FirstOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    _db.Comments.Remove(entity);
    await _db.SaveChangesAsync();
  }

  public async Task<List<CommentModel>> GetByFieldResponse(int fieldResponse)
  {
    var fr = await _db.FieldResponses
               .Include(x => x.Conversation)
               .ThenInclude(x => x.Owner)
               .SingleOrDefaultAsync(x => x.Id == fieldResponse)
             ?? throw new KeyNotFoundException();
    return fr.Conversation.Select(x => new CommentModel(x)).ToList();
  }
}
