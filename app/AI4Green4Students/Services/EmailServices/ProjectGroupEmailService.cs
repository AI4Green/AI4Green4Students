using AI4Green4Students.Data.Entities;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Services.Contracts;



namespace AI4Green4Students.Services.EmailServices;

public class ProjectGroupEmailService
{
  private readonly IEmailSender _emails;


  
  public ProjectGroupEmailService(IEmailSender emails)
  {
    _emails = emails;
  }
  
  public async Task SendProjectGroupAssignmentUpdate(EmailAddress to, string projectName, string projectGroupName)
    => await _emails.SendEmail(
      to,
      "Emails/StudentProjectGroupAssignment",
      new NotifyStudentOfProjectGroupModel(
        to.Name!,
        projectName,
        projectGroupName));
  
  public async Task SendProjectGroupRemovalUpdate(EmailAddress to, string projectName, string projectGroupName)
    => await _emails.SendEmail(
      to,
      "Emails/StudentProjectGroupRemoval",
      new NotifyStudentOfProjectGroupModel(
        to.Name!,
        projectName,
        projectGroupName));

  public async Task SendNoteFeedbackRequest(EmailAddress instructorEmail, string studentName, string projectName,
    string noteUrl, string instructorName, string planName)
  {
    await _emails.SendEmail(
      instructorEmail,
      "Emails/SendNoteFeedbackRequest",
      new SendNoteFeedBackRequestModel(
        projectName,
        studentName,
        noteUrl,
        instructorName,
        planName));
  }

  public async Task SendNoteFeedbackComplete(EmailAddress studentEmail, string studentName, string projectName,
    string noteUrl, string instructorName, string planName)
  {
    await _emails.SendEmail(
      studentEmail,
      "Emails/SendCompleteFeedback",
      new SendNoteFeedbackCompletionModel(
        projectName,
        studentName,
        noteUrl,
        instructorName,
        planName));
  }
}


