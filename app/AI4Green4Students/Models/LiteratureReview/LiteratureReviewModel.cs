namespace AI4Green4Students.Models.LiteratureReview;

public class LiteratureReviewModel
{
  public LiteratureReviewModel(Data.Entities.LiteratureReview entity)
  {
    Id = entity.Id;
    Deadline = entity.Deadline;
    OwnerId = entity.Owner.Id;
    OwnerName = entity.Owner.FullName;
    Stage = entity.Stage.DisplayName;
  }

  public int Id { get; set; }
  public string OwnerId { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public DateTimeOffset Deadline { get; set; }
  public string Stage { get; set; }
  public List<string> Permissions { get; set; } = new();
}

