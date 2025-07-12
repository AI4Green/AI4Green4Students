namespace AI4Green4Students.Services.EmailServices;

using Contracts;
using Models.Emails;

public class ProjectGroupEmailService
{
  private readonly IEmailSender _emails;

  public ProjectGroupEmailService(IEmailSender emails)
    => _emails = emails;

  public async Task AssignProjectGroup(ProjectGroupEmailModel model)
    => await _emails.SendEmail(model.Recipient, "Emails/ProjectGroupAssign", model);

  public async Task RemoveProjectGroup(ProjectGroupEmailModel model)
    => await _emails.SendEmail(model.Recipient, "Emails/ProjectGroupRemove", model);
}
