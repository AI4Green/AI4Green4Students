namespace AI4Green4Students.Auth;

public class SitePermissionClaims
{
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
  public const string ViewOwnProjects = "ViewOwnProjects";

  // For now, using this for both plans and reports.
  // if required, it could be split further if needed. e.g. CreatePlan, CreateReport and so on.
  public const string CreateExperiments = "CreateExperiments";
  public const string EditOwnExperiments = "EditOwnExperiments";
  public const string DeleteOwnExperiments = "DeleteOwnExperiments";
  public const string ViewOwnExperiments = "ViewOwnExperiments";
  public const string ViewProjectGroupExperiments = "ViewProjectGroupExperiments";
  public const string ViewProjectExperiments = "ViewProjectExperiments";

  public const string MarkCommentsAsRead = "MarkCommentsAsRead";
  public const string MakeComments = "MakeComments";
  public const string EditOwnComments = "EditOwnComments";
  public const string DeleteOwnComments = "DeleteOwnComments";
  public const string ApproveFieldResponses = "ApproveFieldResponses";
  public const string ViewAllCommentsForFieldResponse = "ViewAllCommentsForFieldResponse";
  public const string LockProjectGroupNotes = "LockProjectGroupNotes";


  public const string AdvanceStage = "AdvanceStage";

}


