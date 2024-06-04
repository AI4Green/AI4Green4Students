using AI4Green4Students.Models.Emails;
using AI4Green4Students.Services.Contracts;

namespace AI4Green4Students.Services.EmailServices;

public class StageEmailService
{
  private readonly IServiceProvider _serviceProvider;

  public StageEmailService(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  private IEmailSender EmailSender => _serviceProvider.GetRequiredService<IEmailSender>();

  public async Task SendNewSubmissionNotification(EmailAddress to, StageAdvancementEmailModel model)
    => await EmailSender.SendEmail(
      to,
      "Emails/StageSubmit",
      model);
  
  public async Task SendReSubmissionNotification(EmailAddress to, StageAdvancementEmailModel model)
    => await EmailSender.SendEmail(
      to,
      "Emails/StageSubmit",
      model);
  
  public async Task SendRequestChangeNotification(EmailAddress to, StageAdvancementEmailModel model)
    => await EmailSender.SendEmail(
      to,
      "Emails/StageRequestChange",
      model);
  
  public async Task SendApproveNotification(EmailAddress to, StageAdvancementEmailModel model)
    => await EmailSender.SendEmail(
      to,
      "Emails/StageApprove",
      model);
}
