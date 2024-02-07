namespace AI4Green4Students.Data.Entities;

public class LiteratureReviewFieldResponse
{
  public int LiteratureReviewId { get; set; }
  public LiteratureReview LiteratureReview { get; set; } = null!;
  public int FieldResponseId { get; set; }
  public FieldResponse FieldResponse { get; set; } = null!;
}
