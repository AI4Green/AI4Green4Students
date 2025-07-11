namespace AI4Green4Students.Services.EmailServices;

using Contracts;
using Models.Emails;

public class StageEmailService
{
  private readonly IEmailSender _emails;

  public StageEmailService(IEmailSender emails)
    => _emails = emails;

  public async Task SendNewSubmissionNotification(EmailAddress to, AdvanceStageEmailModel model)
    => await _emails.SendEmail(to, "Emails/StageSubmit", model);

  public async Task SendReSubmissionNotification(EmailAddress to, AdvanceStageEmailModel model)
    => await _emails.SendEmail(to, "Emails/StageSubmit", model);

  public async Task SendRequestChangeNotification(EmailAddress to, AdvanceStageEmailModel model)
    => await _emails.SendEmail(to, "Emails/StageRequestChange", model);

  public async Task SendApproveNotification(EmailAddress to, AdvanceStageEmailModel model)
    => await _emails.SendEmail(to, "Emails/StageApprove", model);
}
