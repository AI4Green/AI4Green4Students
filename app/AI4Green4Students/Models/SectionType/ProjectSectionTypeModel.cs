using AI4Green4Students.Constants;

namespace AI4Green4Students.Models.SectionType;

public class ProjectSectionTypeModel
{
  public ProjectSectionTypeModel(List<SectionTypeModel> projectSectionTypes)
  {
    LiteratureReviewSectionTypeId = projectSectionTypes.FirstOrDefault(x => x.Name == SectionTypes.LiteratureReview)?.Id;
    PlanSectionTypeId = projectSectionTypes.FirstOrDefault(x => x.Name == SectionTypes.Plan)?.Id;
    ReportSectionTypeId = projectSectionTypes.FirstOrDefault(x => x.Name == SectionTypes.Report)?.Id;
    ProjectGroupSectionTypeId = projectSectionTypes.FirstOrDefault(x => x.Name == SectionTypes.ProjectGroup)?.Id;
  }
  public int? LiteratureReviewSectionTypeId { get; set; }
  public int? PlanSectionTypeId { get; set; }
  public int? ReportSectionTypeId { get; set; }
  public int? ProjectGroupSectionTypeId { get; set; }
}
