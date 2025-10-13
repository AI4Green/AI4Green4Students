namespace AI4Green4Students.Auth;

public class SitePermissionClaims
{
  public const string CreateProjectTypes = "CreateProjectTypes";
  public const string EditProjectTypes = "EditProjectTypes";
  public const string DeleteProjectTypes = "DeleteProjectTypes";
  public const string ViewProjectTypes = "ViewProjectTypes";

  public const string InviteStudents = "InviteStudents";
  public const string InviteInstructors = "InviteInstructors";
  public const string InviteUsers = "InviteUsers";
  public const string EditUsers = "EditUsers";
  public const string DeleteUsers = "DeleteUsers";
  public const string ViewAllUsers = "ViewAllUsers";

  public const string ViewRoles = "ViewRoles";

  public const string CreateRegistrationRules = "CreateRegistrationRules";
  public const string EditRegistrationRules = "EditRegistrationRules";
  public const string DeleteRegistrationRules = "DeleteRegistrationRules";
  public const string ViewRegistrationRules = "ViewRegistrationRules";

  public const string CreateProjects = "CreateProjects";
  public const string EditProjects = "EditProjects";
  public const string DeleteProjects = "DeleteProjects";
  public const string ViewProjects = "ViewProjects";

  public const string CreateProjectGroups = "CreateProjectGroups";
  public const string EditProjectGroups = "EditProjectGroups";
  public const string DeleteProjectGroups = "DeleteProjectGroups";
  public const string ViewProjectGroups = "ViewProjectGroups";

  // For now, using this for both plans and reports.
  // if required, it could be split further if needed. e.g. CreatePlan, CreateReport and so on.
  public const string CreateExperiments = "CreateExperiments";
  public const string DeleteExperiments = "DeleteExperiments";
  public const string ViewExperiments = "ViewExperiments";
  public const string ViewProjectGroupExperiments = "ViewProjectGroupExperiments";
  public const string ViewProjectExperiments = "ViewProjectExperiments";

  public const string MarkCommentsAsRead = "MarkCommentsAsRead";
  public const string AddComments = "AddComments";
  public const string EditComments = "EditComments";
  public const string DeleteComments = "DeleteComments";
  public const string ApproveFieldResponses = "ApproveFieldResponses";
  public const string LockProjectGroupNotes = "LockProjectGroupNotes";

  public const string AdvanceStage = "AdvanceStage";

}


