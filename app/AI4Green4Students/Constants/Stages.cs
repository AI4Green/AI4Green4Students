namespace AI4Green4Students.Constants;

public class Stages
{
  public const string Draft = "Draft";
  public const string InReview = "In Review";
  public const string AwaitingChanges = "Awaiting Changes";
  public const string Approved = "Approved";
  public const string Submitted = "Submitted";
  public const string Locked = "Locked";
  public const string OnGoing = "On Going";
  public const string Completed = "Completed";
}

public class PlanStages
{
  public const string Draft = Stages.Draft;
  public const string InReview = Stages.InReview;
  public const string AwaitingChanges = Stages.AwaitingChanges;
  public const string Approved = Stages.Approved;
}

public class NoteStages
{
  public const string Draft = Stages.Draft;
  public const string Locked = Stages.Locked;
}

public class LiteratureReviewStages
{
  public const string Draft = Stages.Draft;
  public const string InReview = Stages.InReview;
  public const string AwaitingChanges = Stages.AwaitingChanges;
  public const string Approved = Stages.Approved;
}

public class ReportStages
{
  public const string Draft = Stages.Draft;
  public const string Submitted = Stages.Submitted;
}

/// <summary>
/// Project stages.
/// <remarks>These are not the same as the stages for plans, literature reviews, and reports.</remarks>
/// </summary>
public class ProjectStages
{
  public const string OnGoing = Stages.OnGoing;
  public const string Completed = Stages.Completed;
}
