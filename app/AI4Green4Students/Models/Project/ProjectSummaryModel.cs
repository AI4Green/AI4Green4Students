using AI4Green4Students.Models.LiteratureReview;
using AI4Green4Students.Models.Plan;
using AI4Green4Students.Models.Report;

namespace AI4Green4Students.Models.Project;

public record ProjectSummaryModel(
  List<LiteratureReviewModel> LiteratureReviews,
  List<PlanModel> Plans,
  List<ReportModel> Reports,
  ProjectSummaryProjectModel Project,
  ProjectSummaryProjectGroupModel ProjectGroup,
  ProjectSummaryAuthorModel? Author
);

public record ProjectSummaryProjectModel(int Id, string Name);
public record ProjectSummaryProjectGroupModel(int Id, string Name);
public record ProjectSummaryAuthorModel(string Id, string Name);

