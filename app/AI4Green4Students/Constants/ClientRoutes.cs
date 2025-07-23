namespace AI4Green4Students.Constants;

public static class ClientRoutes
{
  public const string ResetPassword = "/account/password";

  public const string ResendResetPassword = "/account/password/resend";

  public const string ConfirmAccount = "/account/confirm";

  public const string ResendConfirm = "/account/confirm/resend";

  public const string ConfirmEmailChange = "/account/ConfirmEmailChange";

  public const string ConfirmAccountActivation = "/account/activate";

  public static string Overview(string type, int projectId, int projectGroupId, int id)
  {
    var sectionType = type switch
    {
      SectionTypes.Note => "notes",
      SectionTypes.Plan => "plans",
      SectionTypes.Report => "reports",
      SectionTypes.LiteratureReview => "literature-reviews",
      _ => string.Empty
    };

    return $"/projects/{projectId}/project-groups/{projectGroupId}/{sectionType}/{id}/overview";
  }
}
