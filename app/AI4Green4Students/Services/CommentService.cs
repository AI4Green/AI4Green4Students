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
    var fieldResponseEntity = _db.FieldResponses.Single(x => x.Id == model.FieldResponseId) ?? throw new KeyNotFoundException("Field response Id not found");
    var commentEntity = new Comment
    {
      Value = model.Value,
      Owner = model.User,
      CommentDate = DateTime.Now,
      Read = false
    };

   fieldResponseEntity.Conversation.Add(commentEntity);

   //need to check the role of the user - if it is an invigilator then we need to mark the fieldresponse valid as false
   if(model.IsInstructor)
      fieldResponseEntity.Approved = false;
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

    _db.Comments.Update(entity);
    await _db.SaveChangesAsync();
    return await Get(id);
  }

  public async Task Delete(int id)
  {
    var entity = await _db.Comments
                   .FirstOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException();

    _db.Comments.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
