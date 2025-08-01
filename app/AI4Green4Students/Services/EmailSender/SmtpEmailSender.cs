using AI4Green4Students.Config;
using AI4Green4Students.Models.Emails;
using AI4Green4Students.Services.Contracts;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace AI4Green4Students.Services.EmailServices;

public class SmtpEmailSender : IEmailSender
{
  private readonly SmtpOptions _config;
  private readonly RazorViewService _emailViews;
  private readonly ILogger<SmtpEmailSender> _logger;

  public SmtpEmailSender(
    IOptions<SmtpOptions> options,
    RazorViewService emailViews,
    ILogger<SmtpEmailSender> logger)
  {
    _config = options.Value;
    _emailViews = emailViews;
    _logger = logger;

    if (!IsSmtpOptionsAvailable()) LogError("SMTP values missing or incomplete");
  }

  public async Task SendEmail<TModel>(List<EmailAddress> toAddresses, string viewName, TModel model, List<EmailAddress>? ccAddresses = null)
    where TModel : class
  {
    toAddresses = toAddresses.Where(x=> !_config.ExcludedEmailAddresses.Contains(x.Address)).ToList(); // filter out blocked email addresses
    
    if (toAddresses.Count == 0) return;

    var (body, viewContext) = await _emailViews.RenderToString(viewName, model);

    var message = new MimeMessage();

    foreach (var address in toAddresses)
      message.To.Add(!string.IsNullOrEmpty(address.Name)
        ? new MailboxAddress(address.Name, address.Address)
        : MailboxAddress.Parse(address.Address));

    message.From.Add(new MailboxAddress(_config.FromName, _config.FromAddress));
    message.ReplyTo.Add(MailboxAddress.Parse(_config.ReplyToAddress));
    message.Subject = (string)viewContext.ViewBag.Subject ?? string.Empty;

    message.Body = new TextPart(TextFormat.Html)
    {
      Text = body
    };

    using var smtpClient = await ConnectAndAuthenticate(); // connect and authenticate 
    await SendEmailMessage(message, smtpClient); // send email
  }

  public async Task SendEmail<TModel>(EmailAddress toAddress, string viewName, TModel model, EmailAddress? ccAddress = null)
    where TModel : class
    => await SendEmail(new List<EmailAddress> { toAddress }, viewName, model);

  private bool IsSmtpOptionsAvailable()
  {
    // checking if the correct numeric values (port no. and secure socket enum) are supplied
    if (_config.SmtpPort == 0 || _config.SmtpSecureSocketEnum is > 4 or < 1) return false;

    // if okay, then check string values (hostname, username & password) 
    string[] smtpStringOptions = { _config.SmtpHost, _config.SmtpUsername, _config.SmtpPassword };
    return !smtpStringOptions.Any(string.IsNullOrEmpty);
  }

  private async Task<SmtpClient> ConnectAndAuthenticate()
  {
    var smtpClient = new SmtpClient();

    // Anything above 4 or less than 1, use 1(Auto)
    var secureSocketKey = _config.SmtpSecureSocketEnum is > 4 or < 1 ? 1 : _config.SmtpSecureSocketEnum;

    var secureSocketOption = new Dictionary<int, SecureSocketOptions>
    {
      { 1, SecureSocketOptions.Auto },
      { 2, SecureSocketOptions.SslOnConnect },
      { 3, SecureSocketOptions.StartTls },
      { 4, SecureSocketOptions.StartTlsWhenAvailable }
    };

    try
    {
      await smtpClient.ConnectAsync(_config.SmtpHost, _config.SmtpPort, secureSocketOption[secureSocketKey]);
      await smtpClient.AuthenticateAsync(_config.SmtpUsername, _config.SmtpPassword);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message); // log exception message
      LogError("Couldn't connect or authenticate"); // log custom error message
    }

    return smtpClient;
  }
  
  private async Task SendEmailMessage(MimeMessage message, SmtpClient smtpClient)
  {
    try { await smtpClient.SendAsync(message); }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message); // log exception message
      LogError("Couldn't send a message");// log custom error message
    }
    finally
    {
      await smtpClient.DisconnectAsync(true);
      smtpClient.Dispose();
    }
  }

  private void LogError(string customErrorMsg)
  {
    _logger.LogError(customErrorMsg);
    throw new InvalidOperationException(customErrorMsg); // then throw exception
  }
}
