namespace AI4Green4Students.Services.EmailServices;

using Contracts;
using Models.Emails;

public class ProjectEmailService
{
  private readonly IEmailSender _emails;

  public ProjectEmailService(IEmailSender emails)
    => _emails = emails;

  public async Task AssignProject(ProjectEmailModel model)
    => await _emails.SendEmail(model.Recipient, "Emails/ProjectAssign", model);

  public async Task RemoveProject(ProjectEmailModel model)
    => await _emails.SendEmail(model.Recipient, "Emails/ProjectRemove", model);
}
