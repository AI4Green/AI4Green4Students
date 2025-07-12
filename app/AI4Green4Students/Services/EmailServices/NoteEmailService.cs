namespace AI4Green4Students.Services.EmailServices;

using Contracts;
using Models.Emails;

public class NoteEmailService
{
  private readonly IEmailSender _emails;

  public NoteEmailService(IEmailSender emails)
    => _emails = emails;

  public async Task SendNoteFeedbackRequest(EmailAddress email, NoteFeedBackEmailModel model)
    => await _emails.SendEmail(email, "Emails/NoteRequestFeedback", model);

  public async Task SendNoteFeedbackComplete(EmailAddress email, NoteFeedBackEmailModel model)
    => await _emails.SendEmail(email, "Emails/NoteCompleteFeedback", model);
}
