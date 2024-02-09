namespace AI4Green4Students.Models.Comment;

public class CommentModel
{
  public CommentModel(Data.Entities.Comment entity)
  {
    Id = entity.Id;
    Value = entity.Value;
    Read = entity.Read;
    Owner = entity.Owner.FullName;
    CommentDate = entity.CommentDate;
  }

  public int Id { get; set; } 
  public string Value { get; set; }
  public bool Read { get; set; }
  public string Owner { get; set; }
  public DateTimeOffset CommentDate { get; set; }
}
