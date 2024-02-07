using AI4Green4Students.Data.Entities.Identity;

namespace AI4Green4Students.Data.Entities;

public class LiteratureReview
{
  public int Id { get; set; }
  public Project Project { get; set; } = null!;
  public ApplicationUser Owner { get; set; } = null!;
  public DateTimeOffset Deadline { get; set; }
  public Stage Stage { get; set; }
  public List<LiteratureReviewFieldResponse> LiteratureReviewFieldResponses { get; set; } = null!;
}
